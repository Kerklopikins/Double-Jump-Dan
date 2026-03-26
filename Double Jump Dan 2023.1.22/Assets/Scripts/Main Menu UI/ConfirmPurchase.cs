using UnityEngine;
using UnityEngine.UI;

public class ConfirmPurchase : MonoBehaviour 
{
    [SerializeField] Image itemImage;
    [SerializeField] Text confirmPurchaseText;
	[SerializeField] Image itemBackground;
	[SerializeField] Sprite normalItemBackground;
	[SerializeField] Sprite premiumItemBackground;

	public Toggle toggle { get; set; }
    public ShopItem shopItem { get; set; }
    public GemsText _gemsText { get; set; }
    public GameObject _gameObject { get; set; }
	public BuyButton buyButton { get; set; }
    GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;
    }

	public void Open(Image _itemImage, string confirmPurchaseString, bool premiumItem)
	{
        itemImage.rectTransform.sizeDelta = new Vector2(_itemImage.rectTransform.sizeDelta.x * 2, _itemImage.rectTransform.sizeDelta.y * 2);
        itemImage.sprite = _itemImage.sprite;
        confirmPurchaseText.text = confirmPurchaseString;

		if(premiumItem)
			itemBackground.sprite = premiumItemBackground;
		else
			itemBackground.sprite = normalItemBackground;
	}
	
	public void BuyItem()
	{
        toggle.interactable = true;
        gameManager.gems -= shopItem.item.price;

        if(shopItem.item.itemType == Item.ItemType.Hat)
            gameManager.ownedHats.Add(shopItem.item.itemID);

        if(shopItem.item.itemType == Item.ItemType.Gun)
            gameManager.ownedGuns.Add(shopItem.item.itemID);

		if(shopItem.item.itemType == Item.ItemType.Skin)
			gameManager.ownedSkins.Add(shopItem.item.itemID);
		
        if(gameManager.gems != 1)
            _gemsText.gemsText.text = "<color=yellow>" + gameManager.gems.ToString() + "</color>" + "\nGems";
        else
            _gemsText.gemsText.text = "<color=yellow>" + gameManager.gems.ToString() + "</color>" + "\nGem";

        toggle.isOn = true;
		gameManager.SaveUserData();

		buyButton.buyButtonsUpdater.UpdateButtons();

        _gameObject.SetActive(false);
	}
}