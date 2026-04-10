using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemType itemType;
    public int itemID;
    public int price;
    public string description;
    public Sprite picture;
    public bool premiumItem;
    
    public enum ItemType { Hat, Gun, Skin}
}