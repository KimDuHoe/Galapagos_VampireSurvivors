using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EnemyStats))]

public class BombCrab : MonoBehaviour

{

    [Header("감지 및 공격 범위")]

    public float detectionRadius = 10f; // 플레이어를 감지하는 반경

    public float attackRange = 7f;      // 공격을 시작하는 최대 사거리



    [Header("공격 설정")]

    public GameObject bombPrefab;       // 투척할 폭탄 프리팹

    public float chargeTime = 1f;       // 폭탄 투척 전 준비 시간

    public float attackCooldown = 3f;   // 공격 후 쿨타임



    [Header("투척 설정")]

    public Transform throwPoint; // 폭탄이 생성될 위치 (보통 손)

    public float bombFlightDuration = 1.5f; // 폭탄 비행 시간



    // 컴포넌트 참조

    private Rigidbody2D rb;

    private SpriteRenderer spriteRenderer;

    private Animator anim;

    private EnemyStats stats;

    private Transform player;



    private enum State { Chasing, Charging, Attacking, Cooldown }

    private State currentState = State.Chasing;



    private Enemy enemy; // 공통 피격 처리를 위한 Enemy 스크립트 참조



    void Awake()

    {

        rb = GetComponent<Rigidbody2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        anim = GetComponent<Animator>();

        stats = GetComponent<EnemyStats>();

        enemy = GetComponent<Enemy>(); // Enemy 컴포넌트 가져오기

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

            if (rb != null)

            {

                rb.linearVelocity = Vector2.zero;

            }

            return;

        }



        float distanceToPlayer = Vector2.Distance(transform.position, player.position);



        if (currentState == State.Chasing)

        {

            HandleChasingState(distanceToPlayer);

        }

    }



    private void HandleChasingState(float distanceToPlayer)

    {

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



            currentState = State.Charging;



            rb.linearVelocity = Vector2.zero;



    



            Vector2 directionToPlayer = (player.position - transform.position).normalized;



            if (spriteRenderer != null)



            {



                spriteRenderer.flipX = directionToPlayer.x < 0;



            }



    



            anim?.SetTrigger("Charge");



            yield return new WaitForSeconds(chargeTime);



    



            // 피격으로 인해 공격 시퀀스가 중단될 수 있으므로, 공격 재개 전 상태 확인



            // 이 로직은 Enemy.cs의 isKnockback 상태에 따라 변경될 수 있음



            // 현재는 BombCrab 자체 로직에 의해 중단되는 경우가 없으므로 이대로 유지



            // 만약 Enemy.cs의 OnHit() 호출로 인해 넉백이 발생하면, Charge 상태가 변경될 수 있음



            // 따라서 isKnockback 플래그 대신 currentState 변화를 확인하는 것이 더 견고할 수 있음



            if (currentState != State.Charging) yield break;



    



            currentState = State.Attacking;



            anim?.SetTrigger("Attack");



    



            // --- 폭탄 투척 로직 ---



            if (bombPrefab != null)



            {



                Vector2 startPos = (throwPoint != null) ? throwPoint.position : transform.position;



                GameObject bombInstance = Instantiate(bombPrefab, startPos, Quaternion.identity);



                Bomb bombScript = bombInstance.GetComponent<Bomb>();



                if (bombScript != null)



                {



                    bombScript.Throw(player.position, bombFlightDuration);



                }



            }



            // --------------------



    



            yield return new WaitForSeconds(0.5f); // 공격 애니메이션 시간



    



            currentState = State.Cooldown;



            yield return new WaitForSeconds(attackCooldown);



    



            currentState = State.Chasing;



        }



    



        public void OnHit(float damage)



        {



            // 피격 처리를 Enemy.cs에 위임



            enemy?.OnHit(damage);



        }



    



        void OnDrawGizmosSelected()



        {



            Gizmos.color = Color.yellow;



            Gizmos.DrawWireSphere(transform.position, detectionRadius);



            Gizmos.color = Color.red;



            Gizmos.DrawWireSphere(transform.position, attackRange);



        }



    }
