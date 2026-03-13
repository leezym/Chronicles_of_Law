using UnityEngine;
using UnityEngine.UI;

public class DropCatcher : MonoBehaviour
{
    public static DropCatcher Instance;

    Image img;

    void Awake()
    {
        Instance = this;
        img = GetComponent<Image>();
        img.raycastTarget = false;
    }

    public void EnableCatcher()
    {
        img.raycastTarget = true;
    }

    public void DisableCatcher()
    {
        img.raycastTarget = false;
    }
}