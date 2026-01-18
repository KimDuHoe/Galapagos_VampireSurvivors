using UnityEngine;

public class Spine : MonoBehaviour
{
    public float damage;
    public float speed = 10f;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector2 direction)
    {
        // 1. 속도 설정 (가시가 지정된 방향으로 날아가도록)
        rb.linearVelocity = direction.normalized * speed;

        // 2. 발사 방향 각도 계산
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 3. 스프라이트 방향 보정(처음 스프라이트에서 90도 회전 : 가로모양)
        float rotationAngle = angle - 90f;

        // 4. 회전 적용
        transform.rotation = Quaternion.AngleAxis(rotationAngle, Vector3.forward);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어에게 데미지 전달
            other.GetComponent<playerstat>()?.hp_reduce(damage);
            gameObject.SetActive(false); // 플레이어에게 닿으면 비활성화
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // 카메라 영역 밖으로 나가면 비활성화
        if (other.CompareTag("Area"))
        {
            gameObject.SetActive(false);
        }
    }
}
