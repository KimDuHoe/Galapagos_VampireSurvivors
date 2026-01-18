using UnityEngine;
using System.Collections;

public class Mine : MonoBehaviour
{
    public float triggerRadius = 1.5f;  // 폭발 반경
    public float warningDuration = 0.5f; // 자폭 준비 애니메이션 시간
    public float damage = 50f; // 폭발 데미지

    private Transform playerTransform;
    private bool isTriggered = false;
    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        // 오브젝트 풀에서 재사용될 때 상태를 초기화
        isTriggered = false;
        if (anim != null) anim.ResetTrigger("StartWarning");

        // 플레이어의 Transform을 가져오기
        if (GameManager.instance != null && GameManager.instance.player != null)
        {
            playerTransform = GameManager.instance.player.transform;
        }
        // 플레이어가 없을 경우 오브젝트 비활성화
        else
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // 플레이어가 없거나 이미 트리거가 실행된 경우
        if (isTriggered || playerTransform == null)
            return;

        // 플레이어와의 거리 체크, 폭발 반경 내에 있을 때만 실행
        if (Vector3.Distance(transform.position, playerTransform.position) < triggerRadius)
        {
            isTriggered = true; // 중복 실행 방지
            StartCoroutine(ExplodeSequence());
        }
    }

    IEnumerator ExplodeSequence()
    {
        // 1. 자폭 준비 애니메이션 실행
        if (anim != null) anim.SetTrigger("StartWarning");

        // 2. 자폭 준비 애니메이션 시간만큼 대기
        yield return new WaitForSeconds(warningDuration);

        // 3. 폭발 반경 내의 모든 적에게 데미지 주기
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, triggerRadius);
        foreach (Collider2D hit in colliders)
        {
            // 플레이어 데미지 받을 수 있도록 태그 추가 (Test일 때 비활성화)
            if (hit.CompareTag("Player"))
            {
                // 플레이어 데미지를 입는지 확인하고 싶다면, 아래 주석 삭제
                // hit.GetComponent<playerstat>().hp_reduce(damage);
            }
        }

        // 4. 공용 폭발 이펙트 생성 (PoolManager 사용)
        GameObject explosion = GameManager.instance.pool.Get(7);
        explosion.transform.position = transform.position;

        // 5. 이 Mine 오브젝트는 풀로 돌아가기 위해 비활성화
        gameObject.SetActive(false);
    }

    // Gizmos를 사용하여 에디터에서 폭발 반경을 시각적으로 표시, 범위 확인용
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}