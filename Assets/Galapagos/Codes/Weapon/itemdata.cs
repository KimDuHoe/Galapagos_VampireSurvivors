using UnityEngine;


[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemType{ Melee, Range, Heal, Glove, Shoe}
    [Header("# Main Info")]
    public ItemType itemType;
    public int itemId;
    public string itemName;
    
    [TextArea]
    public string itemDesc;
    public Sprite itemIcon;


    [Header("# Level Data")]
    public float baseDamage;
    public int baseCount;
    public int basePenetrating_power;
    public Vector2 baseRange;
    public float baseSpeed;
    public float[] damages;
    public int[] counts;
    public int[] penetrating_power;
    public float[] range;
    public float[] speeds;



    [Header("# Weapon")]
    public GameObject projectile;



 
}
