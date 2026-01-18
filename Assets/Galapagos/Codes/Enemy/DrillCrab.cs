using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EnemyStats))]
public class DrillCrab : MonoBehaviour
{
    [Header("감지 및 공격 범위")]
    public float detectionRadius = 15f;
    public float chargeTriggerRadius = 5f;

    [Header("공격 설정")]
    public float chargeTime = 1.5f;
    public float dashSpeed = 20f;
    public float dashDuration = 1f;
    public float attackCooldown = 3f;

    [Header("이펙트 및 애니메이션")]
    public GameObject warningLinePrefab;
    public string drillAnimationTrigger = "TransformToDrill";
    public string revertAnimationTrigger = "RevertFromDrill";
    public string chaseAnimationFloat = "ChaseSpeed";

    // 컴포넌트 참조
    private EnemyStats stats;
    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private GameObject warningLineInstance;

    // 상태 변수
    private enum State { Chasing, Charging, Dashing, Cooldown }
    private State currentState = State.Chasing;

    // --- 피격 관련 변수 ---
    private bool isKnockback = false;
    [Header("피격 효과")]
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;
    [SerializeField] private float hitEffectDuration = 0.1f;
    private bool isHitEffectActive = false;
    // -----------------------

    // 물리 상태 저장을 위한 변수
    private float originalLinearDamping;
    private float originalMass;
    private float heavyMass = 10f; // 저지력을 위한 높은 질량값

    void Awake()
    {
        stats = GetComponent<EnemyStats>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        // 인스펙터에서 Linear Drag 값을 실수로 0으로 두는 것을 방지하기 위한 안전장치
        if (rb.linearDamping < 0.1f)
        {
            rb.linearDamping = 5f;
        }
        originalLinearDamping = rb.linearDamping;
        originalMass = rb.mass; // 시작할 때의 질량 값을 저장
    }

    void Start()
    {
        if (GameManager.instance != null && GameManager.instance.player != null)
        {
            player = GameManager.instance.player.transform;
        }
        else
        {
            Debug.LogError("DrillCrab could not find player from GameManager!", gameObject);
        }
    }

    void OnEnable()
    {
        if (stats.SpeedPoint.CurrentValue <= 0)
            stats.SpeedPoint.SetValue(3f);

        StopAllCoroutines();

        // 상태 초기화
        isKnockback = false;
        isHitEffectActive = false;
        spriteRenderer.color = Color.white;
        transform.rotation = Quaternion.identity;

        // 물리 상태 초기화
        rb.mass = originalMass;
        rb.linearDamping = originalLinearDamping;

        currentState = State.Chasing;
        if (anim != null)
        {
            anim.SetFloat(chaseAnimationFloat, 0f);
            anim.ResetTrigger(drillAnimationTrigger);
            anim.ResetTrigger(revertAnimationTrigger);
        }
        if (col != null) col.isTrigger = false;
        HideWarningLine();

        if (player == null && GameManager.instance?.player != null)
            player = GameManager.instance.player.transform;
    }

    void OnDisable()
    {
        // 오브젝트가 비활성화될 때 경고선이 남아있지 않도록 정리합니다.
        HideWarningLine();
    }
    void FixedUpdate()
    {
        if (player == null || !GameManager.instance.isLive || isKnockback)
        {
            if (currentState != State.Dashing && !isKnockback)
            {
                rb.linearVelocity = Vector2.zero;
            }
            if (currentState == State.Chasing)
            {
                if (anim != null) anim.SetFloat(chaseAnimationFloat, 0f);
            }
            return;
        }

        // isKnockback이 아닐 때만 행동 로직 실행
        if (currentState == State.Chasing)
        {
            HandleChasingState();
        }
    }

    private void HandleChasingState()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= chargeTriggerRadius)
        {
            if (anim != null) anim.SetFloat(chaseAnimationFloat, 0f);
            StartCoroutine(ChargeAndDash());
        }
        else if (distanceToPlayer <= detectionRadius)
        {
            if (anim != null) anim.SetFloat(chaseAnimationFloat, 1f);

            Vector2 direction = ((Vector2)player.position - rb.position).normalized;
            rb.linearVelocity = direction * stats.SpeedPoint.CurrentValue;
            spriteRenderer.flipX = direction.x > 0;
        }
        else
        {
            if (anim != null) anim.SetFloat(chaseAnimationFloat, 0f);
            rb.linearVelocity = Vector2.zero;
        }
    }

    private IEnumerator ChargeAndDash()
    {
        // --- CHARGING ---
        currentState = State.Charging;
        rb.mass = heavyMass;
        rb.linearVelocity = Vector2.zero;

        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        if (anim != null) anim.SetTrigger(drillAnimationTrigger);
        ShowWarningLine(directionToPlayer);

        yield return new WaitForSeconds(chargeTime);

        // 피격으로 인해 공격 시퀀스가 중단되었는지 확인
        if (currentState != State.Charging)
        {
            HideWarningLine();
            yield break;
        }

        // --- DASHING ---
        HideWarningLine();
        currentState = State.Dashing;
        rb.mass = originalMass;
        rb.linearDamping = 0;
        if (col != null) col.isTrigger = true;

        spriteRenderer.flipX = true;

        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        rb.linearVelocity = directionToPlayer * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        // --- COOLDOWN ---
        rb.linearVelocity = Vector2.zero;
        rb.mass = heavyMass;
        rb.linearDamping = originalLinearDamping;
        currentState = State.Cooldown;
        if (col != null) col.isTrigger = false;

        transform.rotation = Quaternion.identity;

        if (anim != null) anim.SetTrigger(revertAnimationTrigger);

        yield return new WaitForSeconds(attackCooldown);

        // --- Back to CHASING ---
        currentState = State.Chasing;
        rb.mass = originalMass;
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
        if (stats.HealthPoint.CurrentValue <= 0) return;

        stats.hp_reduce(damage);

        if (gameObject.activeSelf)
        {
            // 돌진 준비나 돌진 중에는 넉백 스킵
            if (currentState != State.Charging && currentState != State.Dashing)
            {
                StartCoroutine(Knockback());
            }
            StartCoroutine(HitEffect());
        }
    }

    IEnumerator Knockback()
    {
        isKnockback = true;
        // 돌진 코루틴이 실행중이었다면 중단
        StopCoroutine(ChargeAndDash());
        // 상태를 추적으로 안전하게 리셋
        currentState = State.Chasing;
        transform.rotation = Quaternion.identity; // 회전 초기화
        rb.mass = originalMass; // 질량 초기화

        if (player != null)
        {
            Vector2 playerPos = player.position;
            Vector2 dirVec = ((Vector2)transform.position - playerPos).normalized;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(dirVec * knockbackForce, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(knockbackDuration);
        isKnockback = false;
    }

    IEnumerator HitEffect()
    {
        if (isHitEffectActive) yield break;

        isHitEffectActive = true;
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(hitEffectDuration);

        spriteRenderer.color = Color.white;
        isHitEffectActive = false;
    }

    private void ShowWarningLine(Vector2 direction)
    {
        if (warningLinePrefab == null)
        {
            Debug.LogError("Warning Line Prefab is not assigned on " + gameObject.name, gameObject);
            return;
        }
        warningLineInstance = Instantiate(warningLinePrefab, transform.position, Quaternion.identity);

        WarningLineEffect effect = warningLineInstance.GetComponent<WarningLineEffect>();
        if (effect != null)
        {
            effect.parentCrab = transform;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        warningLineInstance.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void HideWarningLine()
    {
        if (warningLineInstance != null)
        {
            Destroy(warningLineInstance);
        }
    }
}