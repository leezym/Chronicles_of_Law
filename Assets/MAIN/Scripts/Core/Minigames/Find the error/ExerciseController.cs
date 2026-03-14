using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EXERCISE.Model;
using EXERCISE.UI;
using EXERCISE.Loaders;
using GAME;

namespace EXERCISE.Runtime
{
    public class ExerciseController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Button validateButton;
        [SerializeField] private TMP_Text selectionCounterText;
        Button closeButton => MinigamesManager.Instance.closeButton;


        [Header("Selection Rules")]
        [SerializeField] private int maxSelections;

        [Header("Renderers (solo uno activo)")]
        [SerializeField] private TableRenderer tableRenderer;
        [SerializeField] private DocumentRenderer documentRenderer;

        [Header("Load (Folder path is: StreamingAssets/Encontrar_el_error/)")]
        [SerializeField] private string exerciseFolderToLoad;

        private ExerciseData data;
        private bool validated = false;

        // Documento: regionId -> token runtime
        private readonly Dictionary<string, TokenData> docTokens = new();

        private bool IsTable => data != null && data.type == ExerciseType.tabla;
        private bool IsDocument => data != null && data.type == ExerciseType.documento;

        private void Awake()
        {
            if (validateButton != null)
                validateButton.onClick.AddListener(Validate);
        }

        private void Start()
        {
            exerciseFolderToLoad = GetLastPart(name);

            ExerciseData e = null;

            if (tableRenderer != null && documentRenderer != null)
                Debug.LogWarning("[ExerciseController] Ambos renderers están asignados. Se priorizará TableRenderer.");

            if (tableRenderer != null)
                e = ExerciseJsonLoader.LoadTableExercise(exerciseFolderToLoad);
            else if (documentRenderer != null)
                e = ExerciseJsonLoader.LoadDocumentExercise(exerciseFolderToLoad);
            else
                Debug.LogError("[ExerciseController] TableRenderer y DocumentRenderer son null. Asigna uno de los dos.");

            LoadExercise(e);
        }

        public void LoadExercise(ExerciseData exercise)
        {
            data = exercise;
            validated = false;
            docTokens.Clear();

            if (data == null)
            {
                Debug.LogError("[ExerciseController] exercise is null.");
                if (validateButton != null) validateButton.interactable = false;
                UpdateSelectionCounter();
                return;
            }

            if (IsDocument)
                BuildDocumentTokens(data as DocumentExerciseData);

            RenderCurrent();
            UpdateValidateButton();
            UpdateSelectionCounter();
        }

        private void BuildDocumentTokens(DocumentExerciseData docData)
        {
            if (docData?.regions?.regions == null) return;

            foreach (var r in docData.regions.regions)
            {
                float pts = (r.points > 0f) ? r.points : docData.regions.pointsPerCorrect;

                docTokens[r.id] = new TokenData
                {
                    id = r.id,
                    enabled = r.enabled,
                    isCorrect = r.correct,
                    points = pts,
                    selected = false,
                    locked = false
                };
            }
        }

        private int GetCurrentSelectionCount()
        {
            if (IsTable)
            {
                var tableData = data as TableExerciseData;
                if (tableData?.tableTokens != null)
                    return tableData.tableTokens.Count(t => t.enabled && t.selected);
            }
            else if (IsDocument)
            {
                return docTokens.Values.Count(t => t.enabled && t.selected);
            }

            return 0;
        }

        private bool CanSelectMore()
        {
            return GetCurrentSelectionCount() < maxSelections;
        }

        private void UpdateSelectionCounter()
        {
            if (selectionCounterText == null) return;

            int current = GetCurrentSelectionCount();
            selectionCounterText.text = $"{current}/{maxSelections}";
        }

        private void OnTableTokenClicked(TokenData token)
        {
            if (validated) return;
            if (token == null) return;
            if (!token.enabled) return;

            // Si ya estaba seleccionada, permitir deseleccionar
            if (token.selected)
            {
                token.selected = false;
            }
            else
            {
                if (!CanSelectMore())
                    return;

                token.selected = true;
            }

            tableRenderer?.RefreshToken(token.id);
            UpdateValidateButton();
            UpdateSelectionCounter();
        }

        // DocumentRenderer entrega regionId (string)
        private void OnRegionClicked(string regionId)
        {
            if (validated) return;
            if (string.IsNullOrEmpty(regionId)) return;

            if (!docTokens.TryGetValue(regionId, out var token))
                return;
            if (!token.enabled) return;

            // Si ya estaba seleccionada, permitir deseleccionar
            if (token.selected)
            {
                token.selected = false;
            }
            else
            {
                if (!CanSelectMore())
                    return;

                token.selected = true;
            }

            documentRenderer?.RefreshToken(regionId);
            UpdateValidateButton();
            UpdateSelectionCounter();
        }

        private void UpdateValidateButton()
        {
            if (validateButton == null) return;

            if (validated || data == null)
            {
                validateButton.interactable = false;
                return;
            }

            validateButton.interactable = GetCurrentSelectionCount() > 0;
        }

        private void Validate()
        {
            if (validated) return;
            if (data == null) return;

            validated = true;
            float pointsToAdd = 0f;

            if (IsTable)
            {
                var tableData = data as TableExerciseData;

                if (tableData?.tableTokens != null)
                {
                    foreach (var t in tableData.tableTokens.Where(t => t.enabled))
                    {
                        t.locked = true;
                        if (t.selected && t.isCorrect)
                            pointsToAdd += t.points;
                    }
                }

                tableRenderer?.RefreshAll();
            }
            else if (IsDocument)
            {
                foreach (var t in docTokens.Values.Where(t => t.enabled))
                {
                    t.locked = true;
                    if (t.selected && t.isCorrect)
                        pointsToAdd += t.points;
                }

                documentRenderer?.RefreshAll();
            }

            if (GameManager.Instance != null)
                GameManager.Instance.SetPoints(pointsToAdd);

            if (validateButton != null)
                validateButton.interactable = false;

            if (HighlighterToolController.Instance != null)
                HighlighterToolController.Instance.ReturnToHome();

            closeButton.interactable = true;
        }

        private void RenderCurrent()
        {
            if (IsTable)
            {
                var tableData = data as TableExerciseData;
                if (tableData == null)
                    throw new Exception("[ExerciseController] data.type es tabla pero data no es TableExerciseData.");

                if (tableRenderer == null)
                    throw new NullReferenceException("ExerciseType.tabla pero tableRenderer es null en esta escena.");

                documentRenderer?.gameObject.SetActive(false);
                tableRenderer.gameObject.SetActive(true);

                tableRenderer.Render(
                    tableData.tableTokens,
                    tableData.tableImages,
                    data.folder,
                    OnTableTokenClicked
                );
                return;
            }

            if (IsDocument)
            {
                var docData = data as DocumentExerciseData;
                if (docData == null)
                    throw new Exception("[ExerciseController] data.type es documento pero data no es DocumentExerciseData.");

                if (documentRenderer == null)
                    throw new NullReferenceException("ExerciseType.documento pero documentRenderer es null en esta escena.");

                tableRenderer?.gameObject.SetActive(false);
                documentRenderer.gameObject.SetActive(true);

                var regionsList = docData.regions?.regions;

                documentRenderer.Render(
                    docData.paragraphs,
                    regionsList,
                    docTokens,
                    OnRegionClicked
                );
                return;
            }

            throw new Exception("ExerciseData.type no soportado.");
        }

        public static string GetLastPart(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            int index = text.LastIndexOf('.');

            if (index < 0 || index == text.Length - 1)
                return text;

            return text.Substring(index + 1);
        }
    }
}