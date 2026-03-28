using UnityEngine;
using UnityEngine.UI;

public class ConfirmPurchase : MonoBehaviour 
{
    [SerializeField] Image itemImage;
    [SerializeField] Text confirmPurchaseText;
	[SerializeField] Image itemBackground;
	[SerializeField] Sprite normalItemBackground;
	[SerializeField] Sprite premiumItemBackground;

    public ShopItem shopItem { get; set; }
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
        gameManager.gems -= shopItem.item.price;

        if(shopItem.item.itemType == Item.ItemType.Hat)
            gameManager.ownedHats.Add(shopItem.item.itemID);

        if(shopItem.item.itemType == Item.ItemType.Gun)
            gameManager.ownedGuns.Add(shopItem.item.itemID);

		if(shopItem.item.itemType == Item.ItemType.Skin)
			gameManager.ownedSkins.Add(shopItem.item.itemID);
		
        MainMenuManager.Instance.RefreshGemsText();

		gameManager.SaveUserData();

        MainMenuManager.Instance.EquipItem(shopItem);
	}
}