using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonEffects : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] AudioClip buttonClick;
    [SerializeField] Color textStartingColor = Color.black;
    [SerializeField] bool dontChangeText;
    Text text;
    Selectable selectable;
    bool pointerDown;
    CanvasGroup canvasGroup;

    void Start()
    {
        if(GetComponentInParent<CanvasGroup>() != null)
            canvasGroup = GetComponentInParent<CanvasGroup>();

        if(GetComponentInChildren<Text>() != null)
        {
            if(!dontChangeText)
                text = GetComponentInChildren<Text>();
        }

        selectable = GetComponent<Selectable>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(!selectable.interactable)
            return;

        if(canvasGroup != null)
            if(!canvasGroup.interactable)
                return;

        if(text != null)
            text.color = Color.white;


        if(buttonClick != null)
            AudioManager.Instance.PlaySound2D(buttonClick);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!selectable.interactable)
            return;

        if(canvasGroup != null)
            if(!canvasGroup.interactable)
                return;

        if(text != null)
            text.color = Color.white;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!selectable.interactable)
            return;

        if(canvasGroup != null)
            if(!canvasGroup.interactable)
                return;

        if(text != null && !pointerDown)
            text.color = textStartingColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(!selectable.interactable)
            return;

        if(canvasGroup != null)
            if(!canvasGroup.interactable)
                return;

        pointerDown = false;

        if(text != null)
            text.color = textStartingColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(!selectable.interactable)
            return;

        if(canvasGroup != null)
            if(!canvasGroup.interactable)
                return;

        pointerDown = true;

        if(text != null)
            text.color = Color.white;
    }
}