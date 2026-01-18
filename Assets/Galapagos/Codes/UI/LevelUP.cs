using UnityEngine;

public class LevelUP : MonoBehaviour
{
    RectTransform rect;
    Item[] items;

    void Awake()
    {
        rect = GetComponent<RectTransform>();    
        items = GetComponentsInChildren<Item>(true);
    }

    public void Show()
    {
        Next();
        rect.localScale = Vector3.one;
        GameManager.instance.Stop();
    }
    
    public void hide()
    {
        rect.localScale = Vector3.zero;
        GameManager.instance.ResumeTime();
    }

    public void Select(int index)
    {
        items[index].onClick();
    }

    void Next()
    {
        //Random Select

        //1. deactive all item
        foreach(Item item in items)
        {
            item.gameObject.SetActive(false);
        }

        //2. select 3 item from all item
        int[] ran = new int[3];
        while(true)
        {
            ran[0] = Random.Range(0, items.Length);
            ran[1] = Random.Range(0, items.Length);
            ran[2] = Random.Range(0, items.Length);

            if(ran[0]!=ran[1] && ran[1]!=ran[2] && ran[0]!=ran[2]) break;
        }

        for(int index = 0; index < ran.Length;index++)
        {
            Item ranItem = items[ran[index]];

            //3. max level item replace other item

            if(ranItem.level == ranItem.data.damages.Length)    
            {
                items[Random.Range(4,7)].gameObject.SetActive(true);
            }
            else
            {
                ranItem.gameObject.SetActive(true);
            }

            ranItem.gameObject.SetActive(true);
        }
        
    }
}
