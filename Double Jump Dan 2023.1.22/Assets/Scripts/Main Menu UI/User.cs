using UnityEngine;
using UnityEngine.UI;
public class User : MonoBehaviour
{
    [SerializeField] Text fileSizeText;
    
    public int user { get; set; }
    public string userName { get; set; }
    public Text text { get; set; }
    GameManager gameManager;
    UserMenu userMenu;
    Toggle toggle;

	void Start() 
	{
        gameManager = GameManager.Instance;
        userMenu = Object.FindFirstObjectByType<UserMenu>();
        toggle = GetComponent<Toggle>();
        text = GetComponentInChildren<Text>();

        user = gameManager.users[transform.GetSiblingIndex()];
        userName = gameManager.userNames[transform.GetSiblingIndex()];
        text.text = userName;

        gameObject.name = userName;

        if(gameManager.currentUser == user)
            toggle.isOn = true;
        else
            toggle.isOn = false;

        MainMenuManager.Instance.OnUsersRefresh += RefreshUserByteSize;
        RefreshUserByteSize();
	}
    
    public void RefreshUserByteSize()
    {
        int userFileSize = gameManager.LoadUserFileSize(user) - 217;
        userFileSize = Mathf.Clamp(userFileSize, 0, 200000);
		fileSizeText.text = userFileSize.ToString() + " Bytes";
    }
    public void CheckIfUpdated()
    {
        if(gameManager.currentUser == user)
        {
            userMenu.user = this;
            toggle.isOn = true;
        }
        else
        {
            toggle.isOn = false;
        }
    }

	public void SelectUser() 
	{
        userMenu.user = this;
        gameManager.currentUser = userMenu.user.user;
        gameManager.currentUserName = userMenu.user.userName;
        gameManager.SaveData();
        gameManager.LoadUserData();
	}
}