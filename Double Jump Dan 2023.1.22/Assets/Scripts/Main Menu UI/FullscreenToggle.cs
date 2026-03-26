using UnityEngine;
using UnityEngine.EventSystems;

public class FullscreenToggle : MonoBehaviour, IPointerClickHandler
{
	public void OnPointerClick(PointerEventData eventData)
	{
		Screen.fullScreen = !Screen.fullScreen;
	}
}