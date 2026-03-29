using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class User : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
{
    [SerializeField] Text fileSizeText;
    public Text usernameText;
    [SerializeField] AudioClip buttonClick;
    [SerializeField] Sprite normalSprite;
    [SerializeField] Sprite highlightedSprite;
    [SerializeField] Sprite disabledSprite;
    public int user { get; set; }
    public string userName { get; set; }
    GameManager gameManager;
    UserMenu userMenu;
    Button button;
    bool isPointerOver;
    bool isPointerDown;
    Image buttonImage;

	void Start() 
	{
        gameManager = GameManager.Instance;
        userMenu = GameObject.FindWithTag("Main Menu").GetComponent<UserMenu>();
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();

        user = gameManager.users[transform.GetSiblingIndex()];
        userName = gameManager.userNames[transform.GetSiblingIndex()];
        usernameText.text = userName;

        gameObject.name = userName;

        Refresh();

        MainMenuManager.Instance.OnUsersRefresh += RefreshUserByteSize;
        RefreshUserByteSize();

        userMenu.OnUserButtonsRefresh += Refresh;
        userMenu.OnButtonsDisabled += ButtonDisable;
	}
    
    public void Remove()
    {
        MainMenuManager.Instance.OnUsersRefresh -= RefreshUserByteSize;

        userMenu.OnUserButtonsRefresh -= Refresh;
        userMenu.OnButtonsDisabled -= ButtonDisable;
        Destroy(gameObject);
    }
    public void RefreshUserByteSize()
    {
        int userFileSize = gameManager.LoadUserFileSize(user) - 217;
        userFileSize = Mathf.Clamp(userFileSize, 0, 200000);
		fileSizeText.text = userFileSize.ToString() + " Bytes";
    }
    public void Refresh()
    {
        if(gameManager.currentUser == user)
        {
            userMenu.user = this;
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
    }

    public void ButtonDisable(bool interactable)
    {
        if(gameManager.currentUser != user)
            button.interactable = interactable;
    }
    
	public void SelectUser() 
	{
        gameManager.SaveUserData();
        userMenu.user = this;
        gameManager.currentUser = userMenu.user.user;
        gameManager.currentUserName = userMenu.user.userName;
        gameManager.SaveData();
        gameManager.LoadUserData();
        userMenu.RefreshUsers();
	}
    void Update()
    {
        if(!button.interactable)
        {
            SetDisabled();
            return;
        }

        if(isPointerDown && isPointerOver)
            SetPressed();
        else if(isPointerOver)
            SetHighlighted();
        else if(!isPointerOver && isPointerDown)
            SetPressed();
        else
            SetNormal();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(buttonClick != null && button.interactable)
            AudioManager.Instance.PlaySound2D(buttonClick);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {       
        isPointerDown = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
    }

    void SetNormal()
    {
        buttonImage.sprite = normalSprite;
        usernameText.color = Color.black;
        fileSizeText.color = Color.black;
    }

    void SetHighlighted()
    {
        buttonImage.sprite = highlightedSprite;
        usernameText.color = Color.white;
        fileSizeText.color = Color.white;
    }

    void SetPressed()
    {
        buttonImage.sprite = highlightedSprite;
        usernameText.color = Color.white;
        fileSizeText.color = Color.white;
    }

    void SetDisabled()
    {
        if(gameManager.currentUser == user)
        {
            buttonImage.sprite = highlightedSprite;
            usernameText.color = Color.white;
            fileSizeText.color = Color.white;
        }
        else
        {
            buttonImage.sprite = disabledSprite;
            usernameText.color = Color.black;
            fileSizeText.color = Color.black;
        }
    }
}