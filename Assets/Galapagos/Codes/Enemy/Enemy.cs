using UnityEngine;
using System.Collections;

// 이 스크립트는 이제 '플레이어를 향해 추적하는 행동' 담당 및 '피격 시 넉백과 이펙트 처리' 담당
// 스탯 관리는 EnemyStats 스크립트로 분리
// 공용 몹 설정 (DrillCrab 제외)

[RequireComponent(typeof(EnemyStats))]
public class Enemy : MonoBehaviour
{
    private EnemyStats stats; // 스탯 정보를 담을 변수
    private Rigidbody2D rigid;
    private SpriteRenderer sprite;
    private bool isKnockback = false;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;

    [Header("Hit Effect Settings")]
    [SerializeField] private float hitEffectDuration = 0.1f;
    private bool isHitEffectActive = false;


    void Awake()
    {
        // EnemyStats 컴포넌트를 가져와서 stats 변수에 할당
        stats = GetComponent<EnemyStats>();
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (stats.target == null || isKnockback) return;

        // 이동 로직은 stats에 저장된 target과 SpeedPoint를 사용하도록 개선
        Vector2 dirVec = (stats.target.position - rigid.position).normalized;
        rigid.linearVelocity = dirVec * stats.SpeedPoint.CurrentValue * Time.fixedDeltaTime;
    }

    void LateUpdate()
    {
        if (stats.target == null || isKnockback) return;

        // 방향 전환 로직도 stats의 target을 사용
        sprite.flipX = stats.target.position.x > rigid.position.x;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            OnHit(other.GetComponent<Bullet>().damage);
        }
    }

    public void OnHit(float damage)
    {
        if (stats.HealthPoint.CurrentValue <= 0) return; // 이미 죽었으면 무시

        stats.hp_reduce(damage);

        // hp_reduce 호출로 인해 Dead()가 실행되어 오브젝트 비활성화 방지위해 조건 추가
        // 따라서 활성 상태일 때만 코루틴을 실행
        if (gameObject.activeSelf)
        {
            StartCoroutine(Knockback());
            StartCoroutine(HitEffect());
        }
    }

    IEnumerator Knockback() // 피격 넉백 코루틴
    {
        isKnockback = true;

        if (stats.target != null)
        {
            Vector2 playerPos = stats.target.position;
            Vector2 dirVec = ((Vector2)transform.position - playerPos).normalized;
            rigid.linearVelocity = Vector2.zero;
            rigid.AddForce(dirVec * knockbackForce, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(knockbackDuration);

        isKnockback = false;
    }

    IEnumerator HitEffect() // 피격 이펙트 코루틴
    {
        if (isHitEffectActive) yield break;

        isHitEffectActive = true;
        sprite.color = Color.red;

        yield return new WaitForSeconds(hitEffectDuration);

        sprite.color = Color.white;
        isHitEffectActive = false;
    }
}
