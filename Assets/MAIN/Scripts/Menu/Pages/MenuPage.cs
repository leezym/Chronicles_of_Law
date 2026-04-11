using UnityEngine;

public class MenuPage : MonoBehaviour
{
    public enum MenuType { Main, CharacterSelector, Final, Pause, Navigation }
    public MenuType menuType;
    private const string OPEN = "Open";
    private const string CLOSE = "Close";
    [SerializeField] private Animator anim;

    public virtual void Open()
    {
        anim.SetTrigger(OPEN);
    }

    public virtual void Close()
    {
        anim.SetTrigger(CLOSE);
    }
}
