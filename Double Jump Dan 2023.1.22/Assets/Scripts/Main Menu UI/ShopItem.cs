using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShopItem : MonoBehaviour 
{
	public Item item;
	[SerializeField] Sprite normalItemBackground;
	[SerializeField] Sprite premiumItemBackground;
    [SerializeField] MainMenuManager mainMenuManager;
	[SerializeField] Sprite[] fireRateSprites;
	
	public bool initialized { get; set; }
	public Toggle toggle { get; set; }
	public GameManager gameManager { get; set; }
	Image itemPicture;
	Text descriptionText;
    Image fireRateImage;
    Text fireModeText;
    RectTransform damageFillPivot;
    Text damageText;
	RectTransform rectTransform;
	Image image;
	ItemManager itemManager;

	void OnEnable()
	{
		StartCoroutine(InitializedCo());
	}

	void Awake()
	{
		gameManager = GameManager.Instance;
		itemManager = mainMenuManager.itemManager;
		toggle = GetComponentInChildren<Toggle>();
		rectTransform = GetComponent<RectTransform>();
		image = GetComponent<Image>();

		if(gameObject.name != "None")
		{
			if(item == null)
				print("Item is null");
			if(item.premiumItem)
				gameObject.GetComponent<Image>().sprite = premiumItemBackground;
			else
				gameObject.GetComponent<Image>().sprite = normalItemBackground;

			itemPicture = transform.Find("Item Image").GetComponent<Image>();
			descriptionText = transform.Find("Description Text").GetComponent<Text>();
		}

		if(item.itemType == Item.ItemType.Hat)
		{
            if(transform.GetSiblingIndex() == 0)
            {
                if(gameManager.ownedHats.Count == 0)
                {
                    toggle.isOn = true;
                    return;
                }
            }

            if(gameManager.hatID == item.itemID)
				toggle.isOn = true;
            else if(gameManager.hatID != item.itemID)
				toggle.isOn = false;
		}

		if(item.itemType == Item.ItemType.Gun)
		{
            fireRateImage = transform.Find("Fire Rate Image").GetComponent<Image>();
            fireModeText = transform.Find("Fire Mode Text").GetComponent<Text>();
            damageFillPivot = transform.Find("Damage").GetChild(0).GetComponent<RectTransform>();
            damageText = transform.Find("Damage Text").GetComponent<Text>();

            foreach(var _gun in itemManager.guns)
            {
                GunInfo gunInfo = _gun.GetComponent<GunInfo>();

                if(_gun.itemID == item.itemID)
                {
                    switch(gunInfo._fireRate)
                    {
                        case GunInfo.FireRate.ExtremelySlow:
                            fireRateImage.sprite = fireRateSprites[0];
                            break;
                        case GunInfo.FireRate.VerySlow:
                            fireRateImage.sprite = fireRateSprites[1];
                            break;
                        case GunInfo.FireRate.Slow:
                            fireRateImage.sprite = fireRateSprites[2];
                            break;
                        case GunInfo.FireRate.Normal:
                            fireRateImage.sprite = fireRateSprites[3];
                            break;
                        case GunInfo.FireRate.Fast:
                            fireRateImage.sprite = fireRateSprites[4];
                            break;
                        case GunInfo.FireRate.VeryFast:
                            fireRateImage.sprite = fireRateSprites[5];
                            break;
                        case GunInfo.FireRate.ExtremelyFast:
                            fireRateImage.sprite = fireRateSprites[6];
                            break;
                    }

					float damagePercent = (float)gunInfo.damage / 100;
                    damageFillPivot.localScale = new Vector3(damagePercent, 1, 1);
                    damageText.text = "Damage: " + gunInfo.damage;

                    if(gunInfo.fireMode == GunInfo.FireMode.Single)
                        fireModeText.text = " Fire Mode - Single";
                    else if(gunInfo.fireMode == GunInfo.FireMode.Automatic)
                        fireModeText.text = "Fire Mode - Automatic";
					else if(gunInfo.fireMode == GunInfo.FireMode.Burst)
                        fireModeText.text = "Fire Mode - Burst";
                }
            }

            if(transform.GetSiblingIndex() == 0)
            {
                if(gameManager.ownedGuns.Count == 0)
                {
                    toggle.isOn = true;
                    return;
                }
            }

            if(gameManager.gunID == item.itemID)
				toggle.isOn = true;
            else if(gameManager.gunID != item.itemID)
				toggle.isOn = false;
		}

		if(item.itemType == Item.ItemType.Skin)
		{
			if(transform.GetSiblingIndex() == 0)
			{
				if(gameManager.ownedSkins.Count == 0)
				{
					toggle.isOn = true;
					return;
				}
			}

			if(gameManager.skinID == item.itemID)
				toggle.isOn = true;
			else if(gameManager.skinID != item.itemID)
				toggle.isOn = false;
		}

		transform.Find("Title Text").GetComponent<Text>().text = transform.name;

		if(itemPicture != null)
			itemPicture.sprite = item.picture;

		if(descriptionText != null)
			descriptionText.text = item.description;
	}

	IEnumerator InitializedCo()
	{
		yield return new WaitForEndOfFrame();

		initialized = true;
	}

	void Update()
	{
		if(item.itemType == Item.ItemType.Gun)
		{
			if(rectTransform.position.x < -100 || rectTransform.position.x > 100)
			{
				transform.GetChild(0).gameObject.SetActive(false);
				transform.GetChild(1).gameObject.SetActive(false);
				transform.GetChild(2).gameObject.SetActive(false);
				transform.GetChild(3).gameObject.SetActive(false);
				transform.GetChild(4).gameObject.SetActive(false);
				transform.GetChild(5).gameObject.SetActive(false);
				transform.GetChild(6).gameObject.SetActive(false);
				//transform.GetChild(7).gameObject.SetActive(false);
				image.enabled = false;
			}
			else
			{
				transform.GetChild(0).gameObject.SetActive(true);
				transform.GetChild(1).gameObject.SetActive(true);
				transform.GetChild(2).gameObject.SetActive(true);
				transform.GetChild(3).gameObject.SetActive(true);
				transform.GetChild(4).gameObject.SetActive(true);
				transform.GetChild(5).gameObject.SetActive(true);
				transform.GetChild(6).gameObject.SetActive(true);
				//transform.GetChild(7).gameObject.SetActive(true);
				image.enabled = true;
			}
		}
		else
		{
			if(rectTransform.position.x < -100 || rectTransform.position.x > 100)
			{
				transform.GetChild(0).gameObject.SetActive(false);
				transform.GetChild(1).gameObject.SetActive(false);
				transform.GetChild(2).gameObject.SetActive(false);
				//transform.GetChild(3).gameObject.SetActive(false);
				image.enabled = false;
			}
			else
			{
				transform.GetChild(0).gameObject.SetActive(true);
				transform.GetChild(1).gameObject.SetActive(true);
				transform.GetChild(2).gameObject.SetActive(true);
				//transform.GetChild(3).gameObject.SetActive(true);
				image.enabled = true;
			}
		}
	}
}