using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour 
{
    Button button;
    ButtonEffects buttonEffects;
	GameManager gameManager;
	int level;

    void Start()
	{
		gameManager = GameManager.Instance;
		MainMenuManager.Instance.OnLevelButtonsRefresh += Refresh;

		level = int.Parse(gameObject.name);

        button = GetComponent<Button>();
        buttonEffects = GetComponent<ButtonEffects>();

		Refresh();
	}

	void Refresh()
	{
		if(level <= gameManager.levelsCompleted)
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