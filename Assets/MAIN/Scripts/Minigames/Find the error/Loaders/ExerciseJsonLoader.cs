using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using EXERCISE.Model;

namespace EXERCISE.Loaders
{
    public static class ExerciseJsonLoader
    {
        public static ExerciseData LoadTableExercise(string folderName)
        {
            string folderPath = Path.Combine(FilePaths.streamingAssets_findError, folderName);
            string basePath = Path.Combine(folderPath, "base.json");
            string keyPath  = Path.Combine(folderPath, "key.json");

            var baseJson = JsonUtility.FromJson<TableBaseJson>(File.ReadAllText(basePath));
            var keyJson  = JsonUtility.FromJson<KeyJson>(File.ReadAllText(keyPath));

            var enabledSet = new HashSet<string>(keyJson.enabled ?? new List<string>());
            var correctSet = new HashSet<string>(keyJson.correct ?? new List<string>());

            var tokens = baseJson.tokens.Select(t =>
            {
                bool enabled = enabledSet.Contains(t.id);
                bool correct = correctSet.Contains(t.id);

                return new TableTokenData
                {
                    id = t.id,
                    text = t.text,
                    sheet = baseJson.sheet,
                    r0 = t.r0, c0 = t.c0,
                    r1 = t.r1, c1 = t.c1,
                    enabled = enabled,
                    isCorrect = enabled && correct,
                    points = (enabled && correct) ? keyJson.pointsPerCorrect : 0
                };
            }).ToList();

            var images = new List<TableImageData>();
            if (baseJson.images != null)
            {
                foreach (var im in baseJson.images)
                {
                    images.Add(new TableImageData
                    {
                        id = im.id,
                        file = im.file,
                        r0 = im.r0, c0 = im.c0,
                        r1 = im.r1, c1 = im.c1,
                        z = im.z
                    });
                }
            }

            return new TableExerciseData
            {
                exerciseId = baseJson.exerciseId,
                title = baseJson.title,
                type = ExerciseType.tabla,
                folder = folderPath,
                tableTokens = tokens,
                tableImages = images
            };
        }

        public static ExerciseData LoadDocumentExercise(string folder)
        {
            string basePath = Path.Combine(FilePaths.streamingAssets_findError, folder, "base.json");
            string regionsPath = Path.Combine(FilePaths.streamingAssets_findError, folder, "regions.json");

            if (!File.Exists(basePath))
            {
                Debug.LogError($"[LegalDocLoader] Missing base.json: {basePath}");
                return null;
            }
            if (!File.Exists(regionsPath))
            {
                Debug.LogError($"[LegalDocLoader] Missing regions.json: {regionsPath}");
                return null;
            }

            var baseText = File.ReadAllText(basePath);
            var regionsText = File.ReadAllText(regionsPath);

            var baseJson = JsonUtility.FromJson<DocumentBaseJson>(baseText);
            var regionsJson = JsonUtility.FromJson<RegionsJson>(regionsText);

            return new DocumentExerciseData
            {
                exerciseId = baseJson.exerciseId,
                title = baseJson.title,
                type = ExerciseType.documento,
                folder = folder,
                paragraphs = baseJson.paragraphs ?? new List<ParagraphJson>(),
                regions = regionsJson
            };
        }
    }
}