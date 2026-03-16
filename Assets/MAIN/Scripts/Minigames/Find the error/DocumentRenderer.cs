using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EXERCISE.Loaders;
using EXERCISE.Model;

namespace EXERCISE.UI
{
    public class DocumentRenderer : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private RectTransform contentRoot;
        [SerializeField] private ParagraphView paragraphPrefab;

        private readonly List<ParagraphView> _views = new();

        // Estado externo (controller)
        private Dictionary<string, TokenData> _regionTokens;
        private Dictionary<string, List<RegionSpan>> _regionSpansByPid;

        // Cache para refrescos
        private List<ParagraphJson> _paragraphs;

        private class RegionSpan
        {
            public string regionId;
            public int start;
            public int end;
        }

        public void Render(
            List<ParagraphJson> paragraphs,
            List<RegionJson> regions,
            Dictionary<string, TokenData> regionTokens,
            Action<string> onRegionClicked
        )
        {
            Clear();

            _paragraphs = paragraphs ?? new List<ParagraphJson>();
            _regionTokens = regionTokens ?? new Dictionary<string, TokenData>();

            BuildSpanLookup(regions);

            foreach (var p in _paragraphs)
            {
                var view = Instantiate(paragraphPrefab, contentRoot);
                view.SetOnRegionClicked(onRegionClicked);

                ApplyAlignment(view.Tmp, p.align);
                view.Tmp.text = BuildParagraphTmp(
                    p,
                    _regionSpansByPid.TryGetValue(p.pid, out var spans) ? spans : null
                );

                _views.Add(view);
            }

            var scroll = contentRoot.GetComponentInParent<ScrollRect>();
            if (scroll != null)
            {
                Canvas.ForceUpdateCanvases();
                scroll.verticalNormalizedPosition = 1f;
                Canvas.ForceUpdateCanvases();
            }
        }

        public void RefreshToken(string _)
        {
            RefreshAll();
        }

        public void RefreshAll()
        {
            if (_paragraphs == null || _views.Count == 0) return;

            for (int i = 0; i < _views.Count && i < _paragraphs.Count; i++)
            {
                var p = _paragraphs[i];
                _views[i].Tmp.text = BuildParagraphTmp(
                    p,
                    _regionSpansByPid.TryGetValue(p.pid, out var spans) ? spans : null
                );
            }
        }

        private void BuildSpanLookup(List<RegionJson> regions)
        {
            _regionSpansByPid = new Dictionary<string, List<RegionSpan>>();

            foreach (var r in regions ?? new List<RegionJson>())
            {
                if (r.spans == null) continue;

                foreach (var sp in r.spans)
                {
                    if (!_regionSpansByPid.TryGetValue(sp.pid, out var list))
                    {
                        list = new List<RegionSpan>();
                        _regionSpansByPid[sp.pid] = list;
                    }

                    list.Add(new RegionSpan
                    {
                        regionId = r.id,
                        start = sp.start,
                        end = sp.end
                    });
                }
            }

            foreach (var kv in _regionSpansByPid)
                kv.Value.Sort((a, b) => a.start.CompareTo(b.start));
        }

        private static void ApplyAlignment(TMP_Text tmp, string align)
        {
            if (tmp == null) return;

            tmp.alignment = (align ?? "left").ToLowerInvariant() switch
            {
                "center" => TextAlignmentOptions.Top,
                "right" => TextAlignmentOptions.TopRight,
                "justify" => TextAlignmentOptions.TopJustified,
                _ => TextAlignmentOptions.TopLeft
            };
        }

        private string BuildParagraphTmp(ParagraphJson p, List<RegionSpan> spans)
        {
            spans ??= new List<RegionSpan>();

            var sb = new StringBuilder();
            int plainPos = 0;
            int spanIndex = 0;

            RegionSpan activeSpan = null;
            string activeCloseTags = string.Empty;

            foreach (var run in p.runs ?? new List<RunJson>())
            {
                string runText = run.text ?? "";
                int cursor = 0;

                while (cursor < runText.Length)
                {
                    int globalPos = plainPos + cursor;

                    // Abrir región si corresponde en esta posición
                    if (activeSpan == null && spanIndex < spans.Count && globalPos == spans[spanIndex].start)
                    {
                        activeSpan = spans[spanIndex];
                        var tags = GetRegionTags(activeSpan.regionId);
                        sb.Append(tags.open);
                        activeCloseTags = tags.close;
                    }

                    // Cerrar región si corresponde en esta posición
                    if (activeSpan != null && globalPos == activeSpan.end)
                    {
                        sb.Append(activeCloseTags);
                        activeSpan = null;
                        activeCloseTags = string.Empty;
                        spanIndex++;
                        continue;
                    }

                    int nextCut = runText.Length;

                    if (activeSpan == null && spanIndex < spans.Count)
                    {
                        int nextStartInRun = spans[spanIndex].start - plainPos;
                        if (nextStartInRun > cursor)
                            nextCut = Mathf.Min(nextCut, nextStartInRun);
                    }

                    if (activeSpan != null)
                    {
                        int nextEndInRun = activeSpan.end - plainPos;
                        if (nextEndInRun > cursor)
                            nextCut = Mathf.Min(nextCut, nextEndInRun);
                    }

                    int len = Mathf.Max(1, nextCut - cursor);
                    string chunk = runText.Substring(cursor, len);

                    sb.Append(ApplyRunStyle(chunk, run));
                    cursor += len;
                }

                plainPos += runText.Length;
            }

            if (activeSpan != null && !string.IsNullOrEmpty(activeCloseTags))
                sb.Append(activeCloseTags);

            return sb.ToString();
        }

        private string ApplyRunStyle(string chunk, RunJson run)
        {
            string t = EscapeTmp(chunk);

            if (run.bold) t = $"<b>{t}</b>";
            if (run.italic) t = $"<i>{t}</i>";
            if (run.underline) t = $"<u>{t}</u>";

            return t;
        }

        private (string open, string close) GetRegionTags(string regionId)
        {
            if (_regionTokens == null || !_regionTokens.TryGetValue(regionId, out var token) || token == null)
                return ($"<link={regionId}>", "</link>");

            // Antes de validar
            if (!token.locked)
            {
                if (!token.enabled)
                    return ("", "");

                if (token.selected)
                    return ($"<link={regionId}><mark=#FFFF0060>", "</mark></link>");

                return ($"<link={regionId}>", "</link>");
            }

            // Después de validar
            if (token.isCorrect && token.selected)
                return ($"<link={regionId}><mark=#00FF00AA>", "</mark></link>");

            if (token.isCorrect && !token.selected)
                return ($"<link={regionId}><mark=#00FF0044>", "</mark></link>");

            if (!token.isCorrect && token.selected)
                return ($"<link={regionId}><mark=#FF0000AA>", "</mark></link>");

            return ($"<link={regionId}>", "</link>");
        }

        private static string EscapeTmp(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Replace("<", "&lt;").Replace(">", "&gt;");
        }

        private void Clear()
        {
            foreach (Transform child in contentRoot)
                Destroy(child.gameObject);

            _views.Clear();
            _paragraphs = null;
            _regionSpansByPid = null;
        }
    }
}