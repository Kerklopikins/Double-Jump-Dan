using UnityEngine;

public class UIArea : MonoBehaviour
{
    [SerializeField] bool firstAreaOpen;
    [SerializeField] Animator animator;
    
    bool open;
    bool initialized;

    void Start()
    {
        if(firstAreaOpen && !initialized)
        {
            OpenArea(true);
            animator.SetBool("Initially Open", true);
            initialized = true;
        }

        if(initialized)
            animator.SetBool("Initially Open", false);
    }

    public void OpenArea(bool _open)
    {
        open = _open;
        animator.SetBool("Open", open);
    }

    public void OnEnable()
    {
        if(firstAreaOpen)
        {
            if(open && initialized)
                animator.SetBool("Open", true);
        }
        else
        {
            if(open)
                animator.SetBool("Open", true);
        }
    }

    void Update()
    {
        if(!open)
            animator.SetBool("Open", false);
    }
}