using UnityEngine;
using UnityEngine.UI;

namespace EXERCISE.UI
{
    public class TableImageView : MonoBehaviour
    {
        [SerializeField] private Image image;

        public void SetSprite(Sprite sprite)
        {
            if (image != null)
            {
                image.sprite = sprite;
                image.preserveAspect = true;

                RectTransform rt = (RectTransform)image.gameObject.transform;
                rt.offsetMin = new Vector2(100f, 0f);
                rt.offsetMax = new Vector2(-100f, 0f);
            }
        }

        public void SetRaycast(bool enabled)
        {
            if (image != null) image.raycastTarget = enabled;
        }
    }
}