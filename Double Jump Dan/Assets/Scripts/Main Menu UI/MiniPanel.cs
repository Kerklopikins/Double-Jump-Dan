using UnityEngine;

public class MiniPanel : MonoBehaviour 
{
    public void Close(Animator animator)
    {
        animator.SetBool("Open", false);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}