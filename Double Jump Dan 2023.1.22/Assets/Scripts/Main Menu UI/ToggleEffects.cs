using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToggleEffects : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] AudioClip toggleClick;
	[SerializeField] bool userToggle;
	[SerializeField] bool blankToggle;

    Toggle toggle;
	Animator animator;
	Text[] texts;
	bool isEquipButton;

    void Start()
    {
		if(gameObject.name == "Equip Button")
			isEquipButton = true;
		
        if(toggleClick == null)
            print(gameObject.name + " doesn't have a toggle click.");

		if(!blankToggle)
			animator = GetComponent<Animator>();
		
        toggle = GetComponent<Toggle>();

		if(!blankToggle)
			texts = GetComponentsInChildren<Text>();
    }

    void Update()
    {
		if(blankToggle)
			return;
		
		if(toggle.isOn)
		{
			animator.enabled = false;
			texts[0].color = Color.white;

			if(userToggle)
				texts[1].color = Color.white;
			
			toggle.interactable = false;

			if(isEquipButton)
				texts[0].text = "Equipped";
		}
		else
		{
			animator.enabled = true;
			texts[0].color = Color.black;

			if(userToggle)
				texts[1].color = Color.black;
			
			toggle.interactable = true;

			if(isEquipButton)
				texts[0].text = "Equip";
		}
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!toggle.interactable)
            return;

        if(toggleClick != null)
            AudioManager.Instance.PlaySound2D(toggleClick);
    }
}