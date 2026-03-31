using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    public ConfirmPurchase confirmPurchase;
	[SerializeField] Text gemsText;
    [SerializeField] AudioClip tooExpensiveSound;
    public ScrollRect gunScrollRect;
    public ScrollRect hatScrollRect;
    public ScrollRect skinScrollRect;
    public Vector2 minMaxVisibilityDistance;
    
    public ItemManager itemManager;

    public event Action OnShopItemsChanged;
    public event Action OnShopTabsChanged;
    public RectTransform currentGunRect { get; set; }
    public RectTransform currentHatRect { get; set; }
    public RectTransform currentSkinRect { get; set; }

    CurrentShopTab currentShopTab = CurrentShopTab.Guns;
    public enum CurrentShopTab { Guns, Hats, Skins }
    GameManager gameManager;

    void Awake()
    {
        Instance = this;       
    }
    void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void EquipItem(ShopItem shopItem)
	{
        itemManager.EquipItem(shopItem);
        OnShopItemsChanged?.Invoke();
    }

    public void RefreshShop()
    {
        RefreshGemsText();
        OnShopItemsChanged?.Invoke();  
    }

    public void RefreshShopScrollRects()
    {
        StartCoroutine(DelayShopContentCentering());
    }

    IEnumerator DelayShopContentCentering()
    {
        yield return null;
        yield return null;
        
        switch(currentShopTab)
        {   
            case CurrentShopTab.Guns:
                CenterShopScrollRect(gunScrollRect, currentGunRect);
                break;
            case CurrentShopTab.Hats:
                CenterShopScrollRect(hatScrollRect, currentHatRect);
                break;
            case CurrentShopTab.Skins:
                CenterShopScrollRect(skinScrollRect, currentSkinRect);
                break;
        }
    }
    public void CenterShopScrollRect(ScrollRect scrollRect, RectTransform currentRect)
    {
        RectTransform content = scrollRect.content;
        RectTransform viewport = scrollRect.viewport;

        float contentWidth = content.rect.width;
        float viewportWidth = viewport.rect.width;

        float itemPosition = Mathf.Abs(currentRect.anchoredPosition.x);
        float targetPosition = itemPosition - (viewportWidth / 2);

        float normalized = Mathf.Clamp01(targetPosition / (contentWidth - viewportWidth));
        scrollRect.horizontalNormalizedPosition = normalized;

        OnShopTabsChanged?.Invoke();
    }
    
    public void SwitchToGunTab()
    {
        currentShopTab = CurrentShopTab.Guns;
        RefreshShopScrollRects();
    }

    public void SwitchToSkinTab()
    {
        currentShopTab = CurrentShopTab.Skins;
        RefreshShopScrollRects();
    }

    public void SwitchToHatTab()
    {
        currentShopTab = CurrentShopTab.Hats;
        RefreshShopScrollRects();
    }
    
    public void RefreshGemsText()
    {
        if(gameManager.gems != 1)
            gemsText.text = "<color=yellow>" + gameManager.gems.ToString() + "</color>" + "\nGems";
		else
            gemsText.text = "<color=yellow>" + gameManager.gems.ToString() + "</color>" + "\nGem";
    }
    public void RefreshGemsText(string text)
    {
        gemsText.text = text;
    }

    public void GetOneThousandGems()
    {
        gameManager.gems += 1000;
        gameManager.totalGemsCollected += 1000;
        RefreshGemsText();
        gameManager.SaveUserData();

		RefreshShop();
    }

    public void GetEverything()
    {
        foreach(var hat in itemManager.hats)
        {
            if(!gameManager.ownedHats.Contains(hat.itemID))            
                gameManager.ownedHats.Add(hat.itemID);
        }

        foreach(var gun in itemManager.guns)
        {
            if(!gameManager.ownedGuns.Contains(gun.itemID))            
                gameManager.ownedGuns.Add(gun.itemID);
        }

		foreach(var skin in itemManager.skins)
        {
            if(!gameManager.ownedSkins.Contains(skin.itemID))            
                gameManager.ownedSkins.Add(skin.itemID);
        }
		
        RefreshShop();
		gameManager.SaveUserData();
    }

	public void FlashGemsText()
	{
		AudioManager.Instance.PlaySound2D(tooExpensiveSound);
		StartCoroutine(FlashGemsTextCo());
	}

	IEnumerator FlashGemsTextCo()
	{
		for(int i = 0; i < 3; i++)
		{
			if(gameManager.gems != 1)
				RefreshGemsText("<color=red>" + gameManager.gems.ToString() + "</color>" + "<color=red>\nGems</color>");
			else
				RefreshGemsText("<color=red>" + gameManager.gems.ToString() + "</color>" + "<color=red>\nGem</color>");

			yield return new WaitForSeconds(0.07f);

			if(gameManager.gems != 1)
				RefreshGemsText("<color=yellow>" + gameManager.gems.ToString() + "</color>" + "<color=white>\nGems</color>");
			else
				RefreshGemsText("<color=yellow>" + gameManager.gems.ToString() + "</color>" + "<color=white>\nGem</color>");

			yield return new WaitForSeconds(0.07f);
		}
    }
}