using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EnemyStats))]
public class ShootingFish : MonoBehaviour
{
    [Header("감지 및 공격 범위")]
    public float detectionRadius = 15f;
    public float attackRange = 10f;

    [Header("공격 설정")]
    public int spinePoolIndex; // PoolManager에 등록된 가시 프리팹의 인덱스
    public float attackCooldown = 2f;
    public int spineCount = 5; // 한 번에 발사할 가시의 수
    public float spreadAngle = 90f; // 가시가 퍼지는 총 각도
    public float attackDamage = 10f; // 가시의 데미지

    // 컴포넌트 참조
    private EnemyStats stats;
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private enum State { Chasing, Attacking, Cooldown }
    private State currentState = State.Chasing;

    void Awake()
    {
        stats = GetComponent<EnemyStats>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (GameManager.instance != null && GameManager.instance.player != null)
        {
            player = GameManager.instance.player.transform;
        }
    }

    void OnEnable()
    {
        StopAllCoroutines();
        currentState = State.Chasing;
    }

    void Update()
    {
        if (player == null || !GameManager.instance.isLive)
        {
            if (rb != null) rb.linearVelocity = Vector2.zero;
            return;
        }

        if (currentState == State.Chasing)
        {
            HandleChasingState();
        }
    }

    private void HandleChasingState()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            StartCoroutine(AttackSequence());
        }
        else if (distanceToPlayer <= detectionRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * stats.SpeedPoint.CurrentValue;
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = direction.x < 0;
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private IEnumerator AttackSequence()
    {
        currentState = State.Attacking;
        rb.linearVelocity = Vector2.zero;

        // --- 부채꼴 발사 로직 ---
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float startAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - spreadAngle / 2;
        float angleStep = spreadAngle / (spineCount > 1 ? spineCount - 1 : 1);

        for (int i = 0; i < spineCount; i++)
        {
            float currentAngle = startAngle + angleStep * i;
            Vector2 fireDirection = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad));

            GameObject spineObject = GameManager.instance.pool.Get(spinePoolIndex);
            spineObject.transform.position = transform.position;

            Spine spineScript = spineObject.GetComponent<Spine>();
            if (spineScript != null)
            {
                spineScript.damage = attackDamage;
                spineScript.Init(fireDirection);
            }
        }
        // --------------------

        currentState = State.Cooldown;
        yield return new WaitForSeconds(attackCooldown);

        currentState = State.Chasing;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Bullet")) return;
        stats.hp_reduce(other.GetComponent<Bullet>().damage);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
