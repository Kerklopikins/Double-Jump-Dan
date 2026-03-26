using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GemsText : MonoBehaviour 
{
	[SerializeField] BuyButtonsUpdater[] buyButtonUpdaters;
	[SerializeField] AudioClip tooExpensiveSound;
	
    public Text gemsText { get; set; }
    GameManager gameManager;

	void Start()
	{
		gameManager = GameManager.Instance;

		gemsText = GetComponent<Text>();

        if(gameManager.gems != 1)
            gemsText.text = "<color=yellow>" + gameManager.gems.ToString() + "</color>" + "\nGems";
		else
            gemsText.text = "<color=yellow>" + gameManager.gems.ToString() + "</color>" + "\nGem";
	}

    public void GetOneThousandGems()
    {
        gameManager.gems += 1000;
        gemsText.text = "<color=yellow>" + gameManager.gems.ToString() + "</color>" + "\nGems";
        gameManager.SaveUserData();

		for(int i = 0; i < buyButtonUpdaters.Length; i++)
		{
			buyButtonUpdaters[i].UpdateButtons();
		}
    }

	public void Flash()
	{
		AudioManager.Instance.PlaySound2D(tooExpensiveSound);
		StartCoroutine(FlashCo());
	}

	IEnumerator FlashCo()
	{
		for(int i = 0; i < 3; i++)
		{
			if(gameManager.gems != 1)
				gemsText.text = "<color=red>" + gameManager.gems.ToString() + "</color>" + "<color=red>\nGems</color>";
			else
				gemsText.text = "<color=red>" + gameManager.gems.ToString() + "</color>" + "<color=red>\nGem</color>";

			yield return new WaitForSeconds(0.07f);

			if(gameManager.gems != 1)
				gemsText.text = "<color=yellow>" + gameManager.gems.ToString() + "</color>" + "<color=white>\nGems</color>";
			else
				gemsText.text = "<color=yellow>" + gameManager.gems.ToString() + "</color>" + "<color=white>\nGem</color>";

			yield return new WaitForSeconds(0.07f);
		}
	}
}