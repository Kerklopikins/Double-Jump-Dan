using UnityEngine;
using System.Collections;

public class UIScreenManager : MonoBehaviour
{
    public Animator initiallyOpen;

    public Animator m_Open { get; protected set; }
    int m_OpenParameterId;
    const string k_OpenTransitionName = "Open";
    const string k_ClosedStateName = "Closed";

    void Start()
    {
        StartCoroutine(StartCo());
    }

    IEnumerator StartCo()
    {
        yield return new WaitForEndOfFrame();

        m_OpenParameterId = Animator.StringToHash(k_OpenTransitionName);
        OpenPanel(initiallyOpen);
    }

    public void OpenPanel(Animator anim)
    {
        if(m_Open == anim)
            return;

        anim.gameObject.SetActive(true);
        anim.transform.SetAsLastSibling();

        CloseCurrent();

        m_Open = anim;
        m_Open.SetBool(m_OpenParameterId, true);
    }

    void CloseCurrent()
    {
        if(m_Open == null)
            return;

        m_Open.SetBool(m_OpenParameterId, false);

        StartCoroutine(DisablePanelDeleyed(m_Open));
        m_Open = null;
    }

    IEnumerator DisablePanelDeleyed(Animator anim)
    {
        bool closedStateReached = false;
        bool wantToClose = true;

        while(!closedStateReached && wantToClose)
        {
            if(!anim.IsInTransition(0))
                closedStateReached = anim.GetCurrentAnimatorStateInfo(0).IsName(k_ClosedStateName);

            wantToClose = !anim.GetBool(m_OpenParameterId);

            yield return new WaitForEndOfFrame();
        }

        if(wantToClose)
            anim.gameObject.SetActive(false);
    }

    public void OpenMiniPanel(Animator animator)
    {
        animator.transform.SetAsLastSibling();
        animator.gameObject.SetActive(true);
        animator.SetBool("Open", true);
    }
}