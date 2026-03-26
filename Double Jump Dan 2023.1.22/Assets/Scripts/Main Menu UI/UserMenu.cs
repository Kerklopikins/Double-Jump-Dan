using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UserMenu : MonoBehaviour 
{
    [SerializeField] Toggle userToggle;
    [SerializeField] ToggleGroup userToggleGroup;
    [SerializeField] CanvasGroup usersParent;
    [SerializeField] InputField userNameInputField;
    [SerializeField] GameObject okayButton;
    [SerializeField] GameObject renameButton;
    [SerializeField] Button newButton;
    [SerializeField] Button deleteButton;
    [SerializeField] Button[] buttons;
    [SerializeField] Text currentUserText;
    [SerializeField] AudioClip buttonClick;
    [SerializeField] Text deleteUserPanelTitleText;
    [SerializeField] Text deleteUserText;
    [SerializeField] Text deleteUserButtonText;
    [SerializeField] Animator createNewUser;
    [SerializeField] InputField createNewUserInputField;
    [SerializeField] Button createNewUserButton;
    [SerializeField] EventSystem eventSystem;

    public User user { get; set; }
    public Animator mainMenu { get; set; }
    public UIScreenManager uiScreenManager { get; set; }
    GameManager gameManager;
    bool renaming;
	string oldUserName;

    void Start()
    {
        mainMenu = GameObject.FindGameObjectWithTag("Main Menu").GetComponent<Animator>();

        gameManager = GameManager.Instance;
        uiScreenManager = GetComponent<UIScreenManager>();

        if(gameManager.users.Count == 6)
            newButton.interactable = false;
        else
            newButton.interactable = true;

        userNameInputField.interactable = false;
        
        for(int i = 0; i < gameManager.userNames.Count; i++)
        {
            var _userToggle = (Toggle)Instantiate(userToggle, Vector3.zero, Quaternion.identity);
            _userToggle.transform.SetParent(usersParent.transform);
            _userToggle.transform.localScale = Vector3.one;
            _userToggle.transform.localPosition = Vector3.zero;
            _userToggle.group = userToggleGroup;
        }

        if(gameManager.users.Count == 0)
            GetComponent<UIScreenManager>().initiallyOpen = createNewUser;
    }

    void Update()
    {
        currentUserText.text = gameManager.currentUserName;

		if(userNameInputField.text.Length > 0 && !renaming && !gameManager.userNames.Contains(userNameInputField.text))
        {
            okayButton.SetActive(true);

            if(Input.GetKeyDown(KeyCode.Return))
                CreateNewUser();
        }
        else
        {
            okayButton.SetActive(false);
        }

        if(gameManager.users.Count <= 1)
            deleteButton.interactable = false;

        if(gameManager.users.Count == 0)
        {
            if(createNewUserInputField.text.Length > 0)
            {
                createNewUserButton.interactable = true;

                if(Input.GetKeyDown(KeyCode.Return))
                    CreateNewUser();
            }
            else
                createNewUserButton.interactable = false;
        }

		if(renaming)
		{
			user.userName = userNameInputField.text;
			user.text.text = user.userName;
		}

		if(userNameInputField.text.Length > 0 && renaming && !gameManager.userNames.Contains(userNameInputField.text) || userNameInputField.text == oldUserName)
        {
            renameButton.SetActive(true);

            if(Input.GetKeyDown(KeyCode.Return))
                RenameUser();
        }
        else
        {
            renameButton.SetActive(false);
        }
    }

    public void New()
    {
        AudioManager.Instance.PlaySound2D(buttonClick);

        userNameInputField.interactable = true;
        usersParent.interactable = false;

        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }

        eventSystem.SetSelectedGameObject(userNameInputField.gameObject);
    }

    public void CreateNewUser()
	{
		LevelLoadingManager.Instance.LoadScene(1);
		
        string userName = "";

        if(gameManager.users.Count == 0)
            userName = createNewUserInputField.text;
        else
            userName = userNameInputField.text;

        AudioManager.Instance.PlaySound2D(buttonClick);

        gameManager.DeleteTempValues();
        userNameInputField.interactable = false;
        usersParent.interactable = true;

        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = true;
        }

        if(!gameManager.userNames.Contains(userName))
        {
            int randomUserID = Random.Range(0, 9999);

            gameManager.userNames.Add(userName);
            gameManager.users.Add(randomUserID);

            gameManager.currentUser = randomUserID;
            gameManager.currentUserName = userName;
            gameManager.SaveData();

            gameManager.SaveUserData();

            var _userToggle = (Toggle)Instantiate(userToggle, Vector3.zero, Quaternion.identity);
            _userToggle.GetComponentInChildren<Text>().text = userName;
            _userToggle.transform.SetParent(usersParent.transform);
            _userToggle.transform.localScale = Vector3.one;
            _userToggle.transform.localPosition = Vector3.zero;
            _userToggle.group = userToggleGroup;
            //uiScreenManager.OpenPanel(mainMenu);
        }

        if(gameManager.users.Count == 6)
            newButton.interactable = false;
        else
            newButton.interactable = true;

        userNameInputField.text = "";
    }

    public void Rename()
    {
		oldUserName = user.userName;

        AudioManager.Instance.PlaySound2D(buttonClick);

        userNameInputField.interactable = true;
        usersParent.interactable = false;

        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }

        eventSystem.SetSelectedGameObject(userNameInputField.gameObject);
        userNameInputField.text = user.userName;
        renaming = true;
    }

    public void RenameUser()
    {
        AudioManager.Instance.PlaySound2D(buttonClick);

        userNameInputField.interactable = false;
        usersParent.interactable = true;
        gameManager.userNames[user.transform.GetSiblingIndex()] = user.userName;
        gameManager.currentUserName = userNameInputField.text;
        gameManager.SaveData();

        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = true;
        }

        userNameInputField.text = "";
        renaming = false;
    }

    public void SetDeletePanelTexts()
    {
        deleteUserPanelTitleText.text = "Delete " + gameManager.currentUserName;
        deleteUserText.text = "Are you sure you want to delete " + gameManager.currentUserName + "." + " All data for that user will be lost.";
        deleteUserButtonText.text = "Delete " + gameManager.currentUserName;
    }

    public void Delete()
    {
        if(gameManager.users.Count > 0)
        {
            gameManager.DeleteUserData(gameManager.currentUser);
            gameManager.users.RemoveAt(user.transform.GetSiblingIndex());
            gameManager.userNames.RemoveAt(user.transform.GetSiblingIndex());
            gameManager.currentUser = gameManager.users[0];
            gameManager.currentUserName = gameManager.userNames[0];
            Destroy(user.gameObject);
            gameManager.SaveData();

            StartCoroutine(UpdateUsers());
        }
        else if(gameManager.users.Count == 0)
        {
            userNameInputField.interactable = true;

            for(int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = false;
            }

            eventSystem.SetSelectedGameObject(userNameInputField.gameObject);
        }

        if(gameManager.users.Count == 6)
            newButton.interactable = false;
        else
            newButton.interactable = true;
    }

    IEnumerator UpdateUsers()
    {
        yield return new WaitForEndOfFrame();

        for(int i = 0; i < usersParent.transform.childCount; i++)
        {
            usersParent.transform.GetChild(i).GetComponent<User>().CheckIfUpdated();
        }
    }
}