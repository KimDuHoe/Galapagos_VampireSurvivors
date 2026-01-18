using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Scripting;

public class Item : MonoBehaviour
{
    public ItemData data;
    public int level;
    public Weapon weapon;
    public Gear gear;

    Image icon;
    Text textLevel;
    Text textName;
    Text textDesc;

    void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;

        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0];
        textName = texts[1];
        textDesc = texts[2];
        textName.text = data.itemName;
    }

    void OnEnable()
    {
        textLevel.text = "Lv." + (level + 1);

        switch(data.itemType) {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100, data.speeds[level]);
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                textDesc.text = string.Format(data.itemDesc, data.damages[level]);
                break;
            default:
                textDesc.text = string.Format(data.itemDesc);
                break;
        }

    }



    public void onClick()
    {
        switch(data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
            if(level == 0)
            {
                GameObject newWeapon = new GameObject();   
                weapon = newWeapon.AddComponent<Weapon>();
                weapon.Init(data);
            }
            else
            {
                float nextDamage = data.baseDamage;
                int nextCount = data.baseCount;
                float nextSpeed = data.baseSpeed;
                int nextpenetrating_power = data.basePenetrating_power;

                nextDamage += data.baseDamage * data.damages[level];
                nextCount = data.counts[level];
                nextSpeed = data.baseSpeed * data.speeds[level];
                nextpenetrating_power =  data.penetrating_power[level];

                weapon.Levelup(nextDamage, nextCount, nextSpeed, nextpenetrating_power);
            }
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
            if(level == 0) 
                {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(data);
                }
                else
                {
                    float nextRate = data.damages[level];
                    gear.Levelup(nextRate);
                }
                break;
            case ItemData.ItemType.Heal:
                break;
        }
    level++;

    if(level == data.damages.Length)
    {
        GetComponent<Button>().interactable = false;
    }

    }
    
    
}

