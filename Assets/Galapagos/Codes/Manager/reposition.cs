using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reposition : MonoBehaviour
{
    Collider2D col;
    public float distance;

    void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, GameManager.instance.player.transform.position) > distance)
        {
            Repositon();
        }
    }
    void Repositon()
    {
        Vector3 PlayerPos = GameManager.instance.player.transform.position;
        Vector3 myPos = transform.position;

        switch (transform.tag)
        {
            case "Ground":
                float diffX = PlayerPos.x - myPos.x;
                float diffY = PlayerPos.y - myPos.y;
                float dirX = diffX < 0 ? -1 : 1;
                float dirY = diffY < 0 ? -1 : 1;
                diffX = Mathf.Abs(diffX);
                diffY = Mathf.Abs(diffY);

                if (diffX > diffY)
                {
                    transform.Translate(Vector3.right * dirX * 40);
                }
                else if (diffX < diffY)
                {
                    transform.Translate(Vector3.up * dirY * 40);
                }
                else
                {
                    transform.Translate(Vector3.right * dirX * 40);
                    transform.Translate(Vector3.up * dirY * 40);
                }

                break;
            case "Enemy":
                if (col.enabled)
                {
                    Vector3 dist = PlayerPos - myPos;
                    Vector3 ran = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0);
                    transform.Translate(ran + dist * 2);
                }
                break;

            case "Mine":
                if (col.enabled)
                {
                    Vector3 dist = PlayerPos - myPos;
                    Vector3 ran = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0);
                    transform.Translate(ran + dist * 2);
                }
                break;
        }


    }

}
