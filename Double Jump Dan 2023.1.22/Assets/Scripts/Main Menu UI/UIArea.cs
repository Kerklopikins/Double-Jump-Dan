using UnityEngine;

public class UIArea : MonoBehaviour
{
    [SerializeField] bool firstAreaOpen;
    [SerializeField] Animator animator;
    
    bool open;

    void Start()
    {
        if(firstAreaOpen)
            OpenArea(true);
    }

    public void OpenArea(bool _open)
    {
        open = _open;
        animator.SetBool("Open", open);
    }

    public void OnEnable()
    {
        if(open)
            animator.SetBool("Open", true);
    }

    void Update()
    {
        if(!open)
            animator.SetBool("Open", false);
    }
}