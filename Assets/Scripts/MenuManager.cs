using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private static readonly int Click = Animator.StringToHash("click");
    public Animator fade;
    public Animator icon;

    public void IconClick()
    {
        fade.SetBool(Click, true);
        icon.SetBool(Click, true);
    }
}