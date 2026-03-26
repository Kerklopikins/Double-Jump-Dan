using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GemsTextFlashTrigger : MonoBehaviour, IPointerClickHandler
{
	Button button;

	void Start()
	{
		button = GetComponent<Button>();
	}
	public void OnPointerClick(PointerEventData eventData)
	{
		if(!button.interactable)
			MainMenuManager.Instance.FlashGemsText();
	}
}