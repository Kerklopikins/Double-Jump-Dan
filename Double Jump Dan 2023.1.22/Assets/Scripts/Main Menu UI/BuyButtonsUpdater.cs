using UnityEngine;
using UnityEngine.UI;

public class BuyButtonsUpdater : MonoBehaviour
{
	BuyButton[] buyButtons;

	void Awake() 
	{
		buyButtons = GetComponentsInChildren<BuyButton>();

		for(int i = 0; i < buyButtons.Length; i++)
		{
			buyButtons[i].buyButtonsUpdater = this;
		}
	}
	
	void Start()
	{
		GetComponent<ToggleGroup>().enabled = true;
	}

	public void UpdateButtons() 
	{
		for(int i = 0; i < buyButtons.Length; i++)
		{
			buyButtons[i].Refresh();
		}
	}
}