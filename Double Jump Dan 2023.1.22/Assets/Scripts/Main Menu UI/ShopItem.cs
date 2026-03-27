using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopItem : MonoBehaviour 
{
	public Item item;
	[SerializeField] Sprite normalItemBackground;
	[SerializeField] Sprite premiumItemBackground;
    [SerializeField] MainMenuManager mainMenuManager;
	[SerializeField] Sprite[] fireRateSprites;
	[SerializeField] AudioClip equippedSound;

	Button equipButton;
	Text equipButtonText;
	GameManager gameManager;
	Image itemPicture;
	Text descriptionText;
    Image fireRateImage;
    Text fireModeText;
    RectTransform damageFillPivot;
    Text damageText;
	RectTransform rectTransform;
	Image image;
	ItemManager itemManager;
	Button buyButton;

	ConfirmPurchase confirmPurchase;
    Text priceText;
    Animator confirmPurchaseAnimator;
	bool refreshed;
	bool checkedVisibility;
	List<GameObject> children = new List<GameObject>();
	float lastStep;
	RectTransform contentHolder;

	void Awake()
	{
		gameManager = GameManager.Instance;
		itemManager = mainMenuManager.itemManager;
		equipButton = GetComponentInChildren<Button>();
		equipButtonText = equipButton.GetComponentInChildren<Text>();
		rectTransform = GetComponent<RectTransform>();
		image = GetComponent<Image>();
		contentHolder = transform.parent.GetComponent<RectTransform>();

		equipButton.onClick.AddListener(OnEquipButtonClicked);
		MainMenuManager.Instance.OnShopItemsChanged += Refresh;
		
		for(int i = 0; i < transform.childCount; i++)
			children.Add(transform.GetChild(i).gameObject);

		if(item.itemID != 1111)
		{
			buyButton = equipButton.transform.Find("Buy Button").GetComponent<Button>();

			confirmPurchase = mainMenuManager.confirmPurchase;
			priceText = buyButton.GetComponentInChildren<Text>();
			confirmPurchaseAnimator = confirmPurchase.GetComponent<Animator>();

			buyButton.onClick.AddListener(BuyButtonClicked);
		}
		
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
		}

		transform.Find("Title Text").GetComponent<Text>().text = transform.name;

		if(itemPicture != null)
			itemPicture.sprite = item.picture;

		if(descriptionText != null)
			descriptionText.text = item.description;

		Refresh();
		UpdateVisibility();
	}

	public void Refresh()
	{
		if(item.itemType == Item.ItemType.Hat)
		{
            if(transform.GetSiblingIndex() == 0)
            {
                if(gameManager.ownedHats.Count == 0)
                {
                    equipButton.interactable = false;
                    return;
                }
            }

            if(gameManager.hatID == item.itemID)
				equipButton.interactable = false;
            else if(gameManager.hatID != item.itemID)
				equipButton.interactable = true;

			if(buyButton != null)
			{
				if(gameManager.ownedHats.Contains(item.itemID))
					buyButton.gameObject.SetActive(false);
				else
                	buyButton.gameObject.SetActive(true);
			}
		}

		if(item.itemType == Item.ItemType.Gun)
		{
            if(transform.GetSiblingIndex() == 0)
            {
                if(gameManager.ownedGuns.Count == 0)
                {
                    equipButton.interactable = false;
                    return;
                }
            }

            if(gameManager.gunID == item.itemID)
				equipButton.interactable = false;
            else if(gameManager.gunID != item.itemID)
				equipButton.interactable = true;

			if(buyButton != null)
			{
				if(gameManager.ownedGuns.Contains(item.itemID))
					buyButton.gameObject.SetActive(false);
				else
					buyButton.gameObject.SetActive(true);
			}
		}

		if(item.itemType == Item.ItemType.Skin)
		{
			if(transform.GetSiblingIndex() == 0)
			{
				if(gameManager.ownedSkins.Count == 0)
				{
					equipButton.interactable = false;
					return;
				}
			}

			if(gameManager.skinID == item.itemID)
				equipButton.interactable = false;
			else if(gameManager.skinID != item.itemID)
				equipButton.interactable = true;

			if(buyButton != null)
			{
				if(gameManager.ownedSkins.Contains(item.itemID))
					buyButton.gameObject.SetActive(false);
				else
                	buyButton.gameObject.SetActive(true);
			}
		}

		if(equipButton.interactable)
		{
			equipButtonText.text = "Equip";
			equipButtonText.color = Color.black;
		}
		else
		{
			equipButtonText.text = "Equipped";
			equipButtonText.color = Color.white;
		}
		
		if(item.itemID == 1111)
			return;

		if(gameManager.gems >= item.price)
		{
			if(item.price > 0)
			{
				priceText.text = "<color=yellow>" + item.price.ToString() + "</color>" + "\nGems";
				buyButton.interactable = true;
			}
			else
			{
				priceText.text = "Free";
				buyButton.interactable = true;
			}
		}
		else
		{
			if(item.price > 0)
			{
				priceText.text = "<color=red>" + item.price.ToString() + "</color>" + "\nGems";
				buyButton.interactable = false;
			}
		}
	}
	public void OnEquipButtonClicked()
	{
		MainMenuManager.Instance.EquipItem(this);
		AudioManager.Instance.PlaySound2D(equippedSound);
	}

	public void BuyButtonClicked()
	{
        if(gameManager.gems >= item.price)
        {
            confirmPurchase.transform.SetAsLastSibling();
            confirmPurchase.gameObject.SetActive(true);
            confirmPurchaseAnimator.SetBool("Open", true);
            confirmPurchase.shopItem = this;

			if(item.price > 0)
				confirmPurchase.Open(itemPicture, "Confirm purchase for " + item.gameObject.name + " for <color=yellow> " + item.price + " </color> gems?", item.premiumItem);
			else
				confirmPurchase.Open(itemPicture, "Confirm purchase for " + item.gameObject.name + " for free?", item.premiumItem);
        }
    }

	void Update()
	{
		int currentStep = Mathf.FloorToInt(contentHolder.anchoredPosition.x / 70);

		if(currentStep != lastStep)
		{
			lastStep = currentStep;
			UpdateVisibility();
		}
	}

	void UpdateVisibility()
	{
		if(rectTransform.position.x < -100 || rectTransform.position.x > 100)
		{
			if(!checkedVisibility)
			{
				//print("Checked Vis " + gameObject.name);
				foreach(var child in children)
					child.SetActive(false);

				image.enabled = false;
				checkedVisibility = true;
			}
			
			refreshed = false;
		}
		else
		{
			if(!refreshed)
			{
				//print("Refreshed Vis " + gameObject.name);
				foreach(var child in children)
					child.SetActive(true);

				image.enabled = true;

				Refresh();
				refreshed = true;
			}
			
			checkedVisibility = false;
		}
	}
}