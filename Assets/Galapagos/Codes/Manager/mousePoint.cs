using UnityEngine;

public class mousePoint : MonoBehaviour
{
    Vector2 mousepos;
    Rigidbody2D mouserigid;

    private void Awake()
    {
        //gameObject.SetActive(true);
        mouserigid = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        mousepos = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
        mouserigid.position = mousepos;
    }
}
