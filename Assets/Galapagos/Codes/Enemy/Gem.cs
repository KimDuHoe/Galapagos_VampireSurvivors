using UnityEngine;
using System.Collections;

public class Gem : MonoBehaviour
{
    public float speed = 3f;
    private Transform player;

    void Update()
    {
        if (player == null && GameManager.instance != null && GameManager.instance.player != null)
        {
            player = GameManager.instance.player.transform;
        }

        if (player != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.GetExp();
            // Assuming the gem will be pooled, otherwise use Destroy(gameObject)
            gameObject.SetActive(false);
        }
    }
}
