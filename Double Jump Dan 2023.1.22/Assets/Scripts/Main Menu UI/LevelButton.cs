using UnityEngine;
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
		level = int.Parse(gameObject.name);

        button = GetComponent<Button>();
        buttonEffects = GetComponent<ButtonEffects>();

		if(level <= gameManager.levelsCompleted)
            button.interactable = true;
		else
			button.interactable = false;
	}

    public void LoadLevel()
    {
		if(Camera.main.orthographic)
			LevelLoadingManager.Instance.fadeSprite.transform.localScale = new Vector3(Camera.main.orthographicSize * ((float)Screen.width / Screen.height) * 16, Camera.main.orthographicSize * 16, 1);
		else
			LevelLoadingManager.Instance.fadeSprite.transform.localScale = new Vector3(Camera.main.orthographicSize * ((float)Screen.width / Screen.height) * 16 + 1, Camera.main.orthographicSize * 16 + 1, 1);
		
        buttonEffects.enabled = false;
		LevelLoadingManager.Instance.LoadScene("Level " + level);
    }
}