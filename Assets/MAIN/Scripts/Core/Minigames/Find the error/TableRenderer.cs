using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using EXERCISE.Model;
using UnityEngine.Networking;
using System.Collections;

namespace EXERCISE.UI
{
    public class TableRenderer : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private RectTransform gridRoot; // Content
        [SerializeField] private TokenView tokenPrefab;
        [SerializeField] private TableImageView imagePrefab;

        [Header("Grid metrics (dynamic)")]
        [SerializeField] private float rowHeightFactor = 0.40f;
        [SerializeField] private float cellPadding = 6f;

        [Header("Safety")]
        [SerializeField] private float minColWidth = 60f;
        [SerializeField] private float minViewportWidth = 200f;

        private readonly Dictionary<string, TokenView> views = new();

        private float _colWidth;
        private float _rowHeight;
        private float _contentWidth;

        // offsets
        private int _minRow;
        private int _minCol;

        public void Render(List<TableTokenData> tokens, List<TableImageData> images, string exerciseFolder, Action<TokenData> onClick)
        {
            Clear();
            if (tokens == null || tokens.Count == 0) return;

            // Content (gridRoot) top-left
            gridRoot.anchorMin = new Vector2(0f, 1f);
            gridRoot.anchorMax = new Vector2(0f, 1f);
            gridRoot.pivot     = new Vector2(0f, 1f);
            gridRoot.anchoredPosition = Vector2.zero;

            int minRow = tokens.Min(t => t.r0);
            int minCol = tokens.Min(t => t.c0);
            int maxRowAbs = tokens.Max(t => t.r1);
            int maxColAbs = tokens.Max(t => t.c1);

            if (images != null && images.Count > 0)
            {
                minRow = Mathf.Min(minRow, images.Min(i => i.r0));
                minCol = Mathf.Min(minCol, images.Min(i => i.c0));
                maxRowAbs = Mathf.Max(maxRowAbs, images.Max(i => i.r1));
                maxColAbs = Mathf.Max(maxColAbs, images.Max(i => i.c1));
            }

            _minRow = minRow;
            _minCol = minCol;

            int maxRow = maxRowAbs - _minRow;
            int maxCol = maxColAbs - _minCol;

            RectTransform viewport = gridRoot.parent as RectTransform; // Viewport
            if (viewport == null)
                throw new Exception("gridRoot debe ser el Content y su parent debe ser el Viewport (RectTransform).");

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(viewport);

            float viewportWidth = viewport.rect.width;
            var scroll = viewport.GetComponentInParent<ScrollRect>();

            if (scroll != null && scroll.verticalScrollbar != null)
            {
                var sbRT = scroll.verticalScrollbar.GetComponent<RectTransform>();
                viewportWidth -= sbRT.rect.width;
            }

            if (viewportWidth < minViewportWidth)
            {
                if (scroll != null)
                {
                    var scrollRT = scroll.GetComponent<RectTransform>();
                    viewportWidth = scrollRT.rect.width;
                }
                if (viewportWidth < minViewportWidth)
                {
                    var canvas = gridRoot.GetComponentInParent<Canvas>();
                    if (canvas != null)
                    {
                        var canvasRT = canvas.GetComponent<RectTransform>();
                        viewportWidth = canvasRT.rect.width * 0.9f;
                    }
                }
            }

            _contentWidth = Mathf.Max(1f, viewportWidth);

            _colWidth  = Mathf.Max(minColWidth, _contentWidth / Mathf.Max(1, (maxCol + 1)));
            _rowHeight = _colWidth * rowHeightFactor;

            float contentHeight = (maxRow + 1) * _rowHeight;
            gridRoot.sizeDelta = new Vector2(_contentWidth, contentHeight);

            if (images != null && images.Count > 0 && imagePrefab != null)
            {
                // orden por z para controlar apilado
                foreach (var im in images.OrderBy(i => i.z))
                {
                    var imgView = Instantiate(imagePrefab, gridRoot);

                    var rt = (RectTransform)imgView.transform;
                    rt.anchorMin = new Vector2(0f, 1f);
                    rt.anchorMax = new Vector2(0f, 1f);
                    rt.pivot     = new Vector2(0f, 1f);

                    // normaliza offsets como tokens
                    Rect rect = GetRect(
                        im.r0 - _minRow, im.c0 - _minCol,
                        im.r1 - _minRow, im.c1 - _minCol
                    );

                    rt.anchoredPosition = new Vector2(rect.x, rect.y);
                    rt.sizeDelta = new Vector2(rect.width, rect.height);

                    // no bloquear clicks
                    imgView.SetRaycast(false);

                    // carga sprite
                    StartCoroutine(LoadSpriteFromStreamingAssets(exerciseFolder, im.file, imgView));
                }
            }

            foreach (var token in tokens)
            {
                var view = Instantiate(tokenPrefab, gridRoot);
                view.Bind(token, onClick);

                var rt = (RectTransform)view.transform;
                rt.anchorMin = new Vector2(0f, 1f);
                rt.anchorMax = new Vector2(0f, 1f);
                rt.pivot     = new Vector2(0f, 1f);

                // Normaliza offsets
                Rect rect = GetRect(
                    token.r0 - _minRow, token.c0 - _minCol,
                    token.r1 - _minRow, token.c1 - _minCol
                );

                rt.anchoredPosition = new Vector2(rect.x, rect.y);
                rt.sizeDelta = new Vector2(rect.width, rect.height);

                views[token.id] = view;
            }

            if (scroll != null)
            {
                Canvas.ForceUpdateCanvases();
                scroll.verticalNormalizedPosition = 1f;
                Canvas.ForceUpdateCanvases();
            }
        }

        private IEnumerator LoadSpriteFromStreamingAssets(string exerciseFolder, string relativeFile, TableImageView target)
        {
            string fullPath = System.IO.Path.Combine(FilePaths.streamingAssets_findError, exerciseFolder, relativeFile);

            // en Android streamingAssetsPath es jar:file://..., se debe usar UnityWebRequest
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(fullPath))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning($"[TableImage] Failed load: {fullPath} -> {uwr.error}");
                    yield break;
                }

                var tex = DownloadHandlerTexture.GetContent(uwr);
                if (tex == null) yield break;

                var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
                target.SetSprite(sprite);
            }
        }

        private Rect GetRect(int r0, int c0, int r1, int c1)
        {
            float x = c0 * _colWidth + cellPadding;
            float y = -(r0 * _rowHeight + cellPadding);

            float w = (c1 - c0 + 1) * _colWidth;
            float h = (r1 - r0 + 1) * _rowHeight;

            w = Mathf.Max(1f, w);
            h = Mathf.Max(1f, h);

            return new Rect(x, y, w, h);
        }

        public void RefreshToken(string tokenId)
        {
            if (views.TryGetValue(tokenId, out var view))
                view.ApplyVisual();
        }

        public void RefreshAll()
        {
            foreach (var v in views.Values)
                v.ApplyVisual();
        }

        private void Clear()
        {
            foreach (Transform child in gridRoot)
                Destroy(child.gameObject);

            views.Clear();
        }
    }
}