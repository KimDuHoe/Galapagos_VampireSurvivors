using UnityEngine;
using System.Collections;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private Status speedPoint;
    [SerializeField] private Status healthPoint;
    [SerializeField] private Status attackPoint;
    [SerializeField] private Status defensePoint;

    public Status SpeedPoint => speedPoint;
    public Status HealthPoint => healthPoint;
    public Status DefensePoint => defensePoint;
    public Status AttackPoint => attackPoint;

    public Rigidbody2D target;

    public GameObject gemPrefab;
    public GameObject deathEffectPrefab;
    public float deathEffectScale = 0.5f;

    private Animator anim;
    private Rigidbody2D rigid;
    private Collider2D coll;

    void Awake()
    {
        // 스탯 객체 생성은 Awake에서 최초 한 번만 실행
        if (speedPoint == null) speedPoint = new Status();
        if (healthPoint == null) healthPoint = new Status();
        if (attackPoint == null) attackPoint = new Status();
        if (defensePoint == null) defensePoint = new Status();

        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }

    void Start()
    {
        // Start는 모든 Awake가 끝난 후 호출되므로, GameManager.instance가 확실히 존재함.
        if (GameManager.instance != null && GameManager.instance.player != null)
        {
            target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        }
    }

    void OnEnable()
    {
        // 오브젝트 풀에서 활성화될 때마다 스탯을 리셋합니다.
        speedPoint.CurrentReset();
        healthPoint.CurrentReset();
        attackPoint.CurrentReset();
        defensePoint.CurrentReset();

        // 리스폰 시 물리 및 충돌 활성화
        if (rigid != null) rigid.simulated = true;
        if (coll != null) coll.enabled = true;
    }

    public void hp_reduce(float damage)
    {
        // In a real game, you might calculate damage with defensePoint here.
        healthPoint.CurrentModify(-damage);

        if (healthPoint.CurrentValue > 0)
        {
            // Live, Hit Action
        }
        else
        {
            // .. die
            StartCoroutine(DeadCo());
        }
    }

    IEnumerator DeadCo()
    {
        // 1. 사망 상태 진입, 물리 비활성화
        gameObject.layer = LayerMask.NameToLayer("Dead"); // 다른 오브젝트와 상호작용 방지
        rigid.simulated = false;
        coll.enabled = false;

        // 2. 사망 애니메이션 및 이펙트 재생
        anim.SetTrigger("Die"); // 몬스터의 '가라앉는' 애니메이션
        GameObject effect = null;
        if (deathEffectPrefab != null)
        {
            effect = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        // 3. 이펙트 지속 시간 동안 트랜스폼 강제 제어
        float effectDuration = 1f; // 이펙트 애니메이션 시간보다 넉넉하게 설정
        float timer = 0;
        while (timer < effectDuration)
        {
            if (effect != null)
            {
                // 프리팹 내부의 애니메이션/스크립트에 의한 변형을 무시하고 크기와 회전을 강제 고정
                effect.transform.localScale = Vector3.one * deathEffectScale;
                effect.transform.rotation = Quaternion.identity;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        // 4. 이펙트가 씬에 남지 않도록 파괴
        if (effect != null)
        {
            Destroy(effect);
        }

        // 5. 경험치 구슬 생성
        if (gemPrefab != null)
        {
            Instantiate(gemPrefab, transform.position, Quaternion.identity);
        }

        // 6. 오브젝트 풀 반환
        GameManager.instance.kill++;
        gameObject.SetActive(false);
    }
}