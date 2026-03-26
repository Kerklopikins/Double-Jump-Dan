using UnityEngine;
using UnityEngine.UI;

public class PauseMenuButton : MonoBehaviour
{
    [SerializeField] ButtonType buttonType;
    [SerializeField] Sprite normalSprite;
    [SerializeField] Sprite highlightedSprite;
    [SerializeField] Sprite disabledSprite;
    [SerializeField] AudioClip buttonClick;   

    public enum ButtonType { TogglePause, Restart, MainMenu }
    Text text;
    Canvas canvas;
    RectTransform rectTransform;
    bool interactable;
    Image image;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponent<Canvas>();
        image = GetComponent<Image>();

        if(GetComponentInChildren<Text>() != null)
            text = GetComponentInChildren<Text>();
    }

    void Update()
    {
        if(text != null)
        {
            if(LevelLoadingManager.Instance.loading)
            {
                image.sprite = normalSprite;
                text.color = Color.black;

                return;
            }
        }

        if(!interactable)
        {
            image.sprite = disabledSprite;

            if(text != null)
                text.color = Color.black;
        }
        
        if(IsCursorOverButton() && interactable)
        {
            image.sprite = highlightedSprite;

            if(text != null)
                text.color = Color.white;

            if(Input.GetMouseButtonUp(0))
            {
                switch (buttonType)
                {
                    case ButtonType.TogglePause:
                        GameHUD.Instance.TogglePause();
                        break;
                    case ButtonType.Restart:
                        GameHUD.Instance.Restart();
                        AudioManager.Instance.PlaySound2D(buttonClick);
                        break;
                    case ButtonType.MainMenu:
                        GameHUD.Instance.LoadMainMenu();
                        AudioManager.Instance.PlaySound2D(buttonClick);
                        break;
                }
            }
        }
        else if(!IsCursorOverButton() && interactable)
        {
            image.sprite = normalSprite;

            if(text != null)
                text.color = Color.black;
        }

    }

    public bool IsCursorOverButton()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, canvas.worldCamera);
    }

    public void SetInteractable(bool _interactable)
    {
        interactable = _interactable;
    }
}