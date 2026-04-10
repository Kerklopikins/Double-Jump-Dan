using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour 
{	
	[SerializeField] LevelSelectMenu levelSelectMenu;
    Button button;
    ButtonEffects buttonEffects;
	GameManager gameManager;
	int level;

    void Start()
	{
		gameManager = GameManager.Instance;

		if(levelSelectMenu == null)
			Debug.LogError("Level Select Menu is null " + gameObject.name);
		else
			levelSelectMenu.OnLevelButtonsRefresh += Refresh;

		level = int.Parse(gameObject.name);

        button = GetComponent<Button>();
        buttonEffects = GetComponent<ButtonEffects>();

		Refresh();
	}

	void Refresh()
	{
		if(level <= gameManager.currentUser.levelsCompleted)
            button.interactable = true;
		else
			button.interactable = false;
	}

    public void LoadLevel()
    {
        buttonEffects.enabled = false;
		LevelLoadingManager.Instance.LoadScene("Level " + level);
    }
}