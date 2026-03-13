using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EXERCISE.Model;
using System;

namespace EXERCISE.UI
{
    public class TokenView : MonoBehaviour
    {
        [SerializeField] private Color correctSelectedColor = new Color();
        [SerializeField] private Color correctMissedColor = new Color();
        [SerializeField] private Color incorrectSelectedColor = new Color();
        [SerializeField] private Color selectedColor = new Color();
        [SerializeField] private Color transparent = new Color();

        [Header("UI")]
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text label;
        [SerializeField] private Image cellBackground;
        [SerializeField] private Image highlightBackground;

        private TokenData token;
        private Action<TokenData> onClick;

        public void Bind(TokenData tokenData, Action<TokenData> onClickCallback)
        {
            token = tokenData;
            onClick = onClickCallback;

            label.text = token.text ?? "";

            // Interacción sólo si está habilitado y no está locked
            button.interactable = token.enabled && !token.locked;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick?.Invoke(token));

            ApplyVisual();
        }

        public void ApplyVisual()
        {
            if (token == null) return;

            if (highlightBackground != null)
            {
                // === DESPUÉS DE VALIDAR ===
                if (token.locked)
                {
                    if (token.isCorrect && token.selected)
                    {
                        // Correcta seleccionada → verde fuerte
                        highlightBackground.color = correctSelectedColor;
                    }
                    else if (token.isCorrect && !token.selected)
                    {
                        // Correcta NO seleccionada → verde suave
                        highlightBackground.color = correctMissedColor;
                    }
                    else if (!token.isCorrect && token.selected)
                    {
                        // Incorrecta seleccionada → rojo
                        highlightBackground.color = incorrectSelectedColor;
                    }
                    else
                    {
                        // Incorrecta no seleccionada → sin color
                        highlightBackground.color = transparent;
                    }
                }
                else
                {
                    // === ANTES DE VALIDAR ===
                    if (token.selected)
                        highlightBackground.color = selectedColor;
                    else
                        highlightBackground.color = transparent;
                }
            }

            if (button != null)
                button.interactable = token.enabled && !token.locked;
        }
    }
}