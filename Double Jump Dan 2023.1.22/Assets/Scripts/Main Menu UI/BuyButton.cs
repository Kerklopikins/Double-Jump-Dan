using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuyButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GemsText _gemsText;
	[SerializeField] MainMenuManager mainMenuManager;

    public ShopItem shopItem { get; set; }
	public BuyButtonsUpdater buyButtonsUpdater { get; set; }
	ConfirmPurchase confirmPurchase;
    Text priceText;
    Toggle toggle;
    Animator confirmPurchaseAnimator;
    Image itemImage;
    GameManager gameManager;

	void Start()
	{
		gameManager = GameManager.Instance;

        shopItem = transform.parent.parent.GetComponentInParent<ShopItem>();

        confirmPurchase = mainMenuManager.confirmPurchase;
		priceText = GetComponentInChildren<Text>();
		toggle = GetComponentInParent<Toggle>();
        confirmPurchaseAnimator = confirmPurchase.GetComponent<Animator>();
        itemImage = transform.parent.parent.Find("Item Image").GetComponent<Image>();

        if(shopItem.item.itemType == Item.ItemType.Hat)
		{
            if(gameManager.ownedHats.Contains(shopItem.item.itemID))
            {
                toggle.interactable = true;
				gameObject.SetActive(false);
            }
			else
            {
                toggle.interactable = false;
				gameObject.SetActive(true);
            }
		}

        if(shopItem.item.itemType == Item.ItemType.Gun)
		{
            if(gameManager.ownedGuns.Contains(shopItem.item.itemID))
            {
                toggle.interactable = true;
				gameObject.SetActive(false);
            }
			else
            {
                toggle.interactable = false;
				gameObject.SetActive(true);
            }
		}

		if(shopItem.item.itemType == Item.ItemType.Skin)
		{
			if(gameManager.ownedSkins.Contains(shopItem.item.itemID))
			{
				toggle.interactable = true;
				gameObject.SetActive(false);
			}
			else
			{
				toggle.interactable = false;
				gameObject.SetActive(true);
			}
		}

		Refresh();
	}

	public void Buy()
	{
        if(gameManager.gems >= shopItem.item.price)
        {
            confirmPurchase.transform.SetAsLastSibling();
            confirmPurchase.gameObject.SetActive(true);
            confirmPurchaseAnimator.SetBool("Open", true);
            confirmPurchase.toggle = toggle;
            confirmPurchase.shopItem = shopItem;
            confirmPurchase._gemsText = _gemsText;
            confirmPurchase._gameObject = gameObject;
			confirmPurchase.buyButton = this;

			if(shopItem.item.price > 0)
				confirmPurchase.Open(itemImage, "Confirm purchase for " + shopItem.item.gameObject.name + " for <color=yellow> " + shopItem.item.price + " </color> gems?", shopItem.item.premiumItem);
			else
				confirmPurchase.Open(itemImage, "Confirm purchase for " + shopItem.item.gameObject.name + " for free?", shopItem.item.premiumItem);
        }
		else
		{
			print("WORKING");
			_gemsText.Flash();
		}
    }

	public void Refresh()
	{
		if(gameManager.gems >= shopItem.item.price)
		{
			if(shopItem.item.price > 0)
			{
				priceText.text = "<color=yellow>" + shopItem.item.price.ToString() + "</color>" + "\nGems";
				GetComponent<Button>().interactable = true;
			}
			else
			{
				priceText.text = "Free";
				GetComponent<Button>().interactable = true;
			}
		}
		else
		{
			if(shopItem.item.price > 0)
			{
				priceText.text = "<color=red>" + shopItem.item.price.ToString() + "</color>" + "\nGems";
				GetComponent<Button>().interactable = false;
			}
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if(gameManager.gems < shopItem.item.price)
			_gemsText.Flash();
	}
}