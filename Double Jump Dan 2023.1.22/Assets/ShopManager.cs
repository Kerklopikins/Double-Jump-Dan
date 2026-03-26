using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShopManager : MonoBehaviour
{
    public Vector2 offset = new Vector2(21, 0);
    [SerializeField] Vector2 contentGrowthIncrements;
    [SerializeField] RectTransform gunsContent;
    [SerializeField] RectTransform hatsContent;
    [SerializeField] RectTransform skinsContent;
    [SerializeField] RectTransform placeHolderCard;
    [SerializeField] GameObject gunCard;
    [SerializeField] GameObject hatOrSkinCard;
    [SerializeField] float deactivationDistance;
    public Sprite normalItemBackground;
	public Sprite premiumItemBackground;
	public Sprite[] fireRateSprites;
    public ItemManager itemManager;

    public ZoneOpen zoneOpen;
    List<RectTransform> gunPlaceholders = new List<RectTransform>();
    List<RectTransform> hatPlaceholders = new List<RectTransform>();
    List<RectTransform> skinPlaceholders = new List<RectTransform>();
    public enum ZoneOpen { Guns, Hats, Skins }
    Vector3 contentPositionLastFrame;
    int scrollDirection = 1;

    void Awake()
    {
        ///itemManager = ItemManager.Instance;
        
        for(int i = 1; i < itemManager.guns.Count + 1; i++)
        {
            gunsContent.sizeDelta = new Vector2(contentGrowthIncrements.x * i, contentGrowthIncrements.y);
        }

        for(int i = 0; i < itemManager.guns.Count; i++)
        {
            var card = (RectTransform)Instantiate(placeHolderCard, Vector3.zero, Quaternion.identity);
            card.transform.parent = gunsContent.transform;
            card.transform.localPosition = new Vector3(contentGrowthIncrements.x * i, gunsContent.transform.localPosition.y, gunsContent.transform.localPosition.z);
            card.transform.localScale = Vector3.one;
            gunPlaceholders.Add(card);
        }

        for(int i = 1; i < itemManager.hats.Count + 1; i++)
        {
            hatsContent.sizeDelta = new Vector2(contentGrowthIncrements.x * i, contentGrowthIncrements.y);
        }

        for(int i = 0; i < itemManager.hats.Count; i++)
        {
            var card = (RectTransform)Instantiate(placeHolderCard, Vector3.zero, Quaternion.identity);
            card.transform.parent = hatsContent.transform;
            card.transform.localPosition = new Vector3(contentGrowthIncrements.x * i, hatsContent.transform.localPosition.y, hatsContent.transform.localPosition.z);
            card.transform.localScale = Vector3.one;
            hatPlaceholders.Add(card);
        }

        for(int i = 1; i < itemManager.skins.Count + 1; i++)
        {
            skinsContent.sizeDelta = new Vector2(contentGrowthIncrements.x * i, contentGrowthIncrements.y);
        }

        for(int i = 0; i < itemManager.skins.Count; i++)
        {
            var card = (RectTransform)Instantiate(placeHolderCard, Vector3.zero, Quaternion.identity);
            card.transform.parent = skinsContent.transform;
            card.transform.localPosition = new Vector3(contentGrowthIncrements.x * i, skinsContent.transform.localPosition.y, skinsContent.transform.localPosition.z);
            card.transform.localScale = Vector3.one;
            skinPlaceholders.Add(card);
        }

        PoolManager.instance.CreatePool("Guns", gunCard.gameObject, 6, gunsContent.transform);
        PoolManager.instance.CreatePool("Hats", hatOrSkinCard.gameObject, 6, hatsContent.transform);
        PoolManager.instance.CreatePool("Skins", hatOrSkinCard.gameObject, 6, skinsContent.transform);

        gunsContent.sizeDelta -= new Vector2(4, 0);
        hatsContent.sizeDelta -= new Vector2(4, 0);
        skinsContent.sizeDelta -= new Vector2(4, 0);

        zoneOpen = ZoneOpen.Guns;

        
    }

    void Start()
    {
        
    }
    public void SetZone(ZoneOpen _zoneOpen)
    {
        zoneOpen = _zoneOpen;
    }

    void UpdateCards()
    {
        for(int i = 0; i < 6; i++)
        {
            int index = gunPlaceholders[i].transform.GetSiblingIndex();
                    
            //Item item = ItemManager.Instance.guns[index];
            //ItemProperties properties = new ItemProperties();
            
            //properties.item = item;
           // properties.position = gunPlaceholders[i].position;
           // properties.shopManager = this;
            
           // if(GameManager.Instance.ownedGuns.Contains(item.itemID))
               // properties.owned = true;

           // if(GameManager.Instance.gunID == item.itemID)
               // properties.equipped = true;
         //   else if(GameManager.Instance.gunID == item.itemID)
                //properties.equipped = false;

          //  PoolManager.instance.ReuseObject("Guns", properties, scrollDirection);
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
            UpdateCards();

        if(zoneOpen == ZoneOpen.Guns)
        {   
            Vector3 delta = gunsContent.transform.position - contentPositionLastFrame;

            if(delta.x > 0)
                scrollDirection = -1;
            if(delta.x < 0)
                scrollDirection = 1;
            
            foreach(RectTransform rect in gunPlaceholders)
            {
                float distanceAbs = Mathf.Abs((rect.transform.position.x + offset.x) - transform.position.x);

                if(distanceAbs > deactivationDistance)
                {
                    rect.gameObject.SetActive(true);
                }
                else
                {
                    if(rect.gameObject.activeSelf == true)
                    {
                        int index = rect.transform.GetSiblingIndex();
                    
                       // Item item = ItemManager.Instance.guns[index];
                       // ItemProperties properties = new ItemProperties();
                        
                       // properties.item = item;
                       // properties.position = rect.position;
                       // properties.shopManager = this;
                        
                       // if(GameManager.Instance.ownedGuns.Contains(item.itemID))
                          //  properties.owned = true;

                        //if(GameManager.Instance.gunID == item.itemID)
                           // properties.equipped = true;
                       // else if(GameManager.Instance.gunID == item.itemID)
                           // properties.equipped = false;

                        //PoolManager.instance.ReuseObject("Guns", properties, scrollDirection);
                        rect.gameObject.SetActive(false);
                    }
                }
            }

            contentPositionLastFrame = gunsContent.position;
        }
    }

    void OnDrawGizmos()
    {
        foreach(RectTransform rect in gunPlaceholders)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(new Vector3(rect.transform.position.x + offset.x, rect.transform.position.y, rect.transform.position.y), 5);
      
        }
    }
}