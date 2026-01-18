using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Bomb : MonoBehaviour
{
    [Header("폭발 설정")]
    public float damage = 30f;
    public float explosionRadius = 3f;
    public float warningDuration = 1f; // 폭발 전 경고 시간

    [Header("물 효과 설정")]
    public SpriteMask spriteMask;     // 폭탄이 물에 잠긴 것처럼 보이게 할 SpriteMask
    public float bobbingSpeed = 4f;   // 둥둥 뜨는 효과의 속도
    public float bobbingHeight = 0.01f; // 둥둥 뜨는 효과의 높이

    [Header("시각 효과")]
    public float spinSpeed = 720f;       // 비행 중 회전 속도
    public SpriteRenderer circleOutline; // 폭발 범위 테두리
    public SpriteRenderer circleFill;    // 채워지는 원
    public Transform visualTransform;    // 스케일링/회전 효과를 줄 트랜스폼

        private Rigidbody2D rb;

        private MaterialPropertyBlock propBlock; // 렌더러 속성 제어를 위한 프로퍼티 블록

        private SpriteRenderer visualSpriteRenderer; // 비주얼 오브젝트의 SpriteRenderer

    

        void Awake()

        {

            rb = GetComponent<Rigidbody2D>();

            propBlock = new MaterialPropertyBlock();

    

            // visualTransform에서 SpriteRenderer 컴포넌트 찾기

            if (visualTransform != null)

            {

                visualSpriteRenderer = visualTransform.GetComponent<SpriteRenderer>();

            }

    

            // 초기 상태 설정

            // 비행 애니메이션이 마스크에 의해 가려지지 않도록 초기에는 마스크 상호작용을 끔

            if (visualSpriteRenderer != null)

            {

                visualSpriteRenderer.maskInteraction = SpriteMaskInteraction.None;

            }

            if (spriteMask)

            {

                spriteMask.enabled = false;

            }

    

            if (circleOutline)

            {

                circleOutline.transform.localScale = Vector3.one * explosionRadius * 2;

                circleOutline.enabled = false;

            }

            if (circleFill)

            {

                circleFill.transform.localScale = Vector3.one * explosionRadius * 2; // 크기는 최대로 고정

    

                // 프로퍼티 블록을 통해 Fill Amount 초기화

                circleFill.GetPropertyBlock(propBlock);

                propBlock.SetFloat("_FillAmount", 0);

                circleFill.SetPropertyBlock(propBlock);

    

                circleFill.enabled = false;

            }

        }

    /// <summary>
    /// 지정된 목표 지점을 향해 폭탄을 던집니다.
    /// </summary>
    /// <param name="targetPosition">목표 위치</param>
    /// <param name="flightDuration">비행 시간</param>
    public void Throw(Vector2 targetPosition, float flightDuration)
    {
        // 직선 비행에 필요한 속도 계산
        Vector2 startPosition = transform.position;
        Vector2 velocity = (targetPosition - startPosition) / flightDuration;
        rb.linearVelocity = velocity;

        // 비행 및 폭발 코루틴 시작
        StartCoroutine(FlightAndExplode(flightDuration));
        StartCoroutine(AnimateFlightVisuals(flightDuration));
    }

    private IEnumerator FlightAndExplode(float duration)
    {
        // 비행 시간 동안 대기
        yield return new WaitForSeconds(duration);

        // 착지 후 속도 정지
        rb.linearVelocity = Vector2.zero;

        // 물에 반쯤 잠긴 효과 활성화
        if (visualSpriteRenderer != null)
        {
            visualSpriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
        if (spriteMask)
        {
            spriteMask.enabled = true;
        }

        // 둥둥 뜨는 효과와 폭발 시퀀스를 동시에 시작
        StartCoroutine(BobbingEffect());
        StartCoroutine(ExplosionSequence());
    }

    private IEnumerator AnimateFlightVisuals(float duration)
    {
        if (visualTransform == null) yield break;

        float timer = 0f;
        Vector3 originalScale = visualTransform.localScale;
        Quaternion originalRotation = visualTransform.localRotation;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;

            // 1. 포물선 스케일 효과
            // 4 * (-x^2 + x) 포물선 공식을 사용하여 스케일을 조절 (0 -> 1 -> 0)
            float scaleMultiplier = 1f + 4f * (-Mathf.Pow(progress, 2) + progress);
            visualTransform.localScale = originalScale * scaleMultiplier;

            // 2. 회전 효과
            visualTransform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);

            yield return null;
        }

        // 비행이 끝나면 원래 스케일과 회전으로 복원
        visualTransform.localScale = originalScale;
        visualTransform.localRotation = originalRotation;
    }

    private IEnumerator ExplosionSequence()
    {
        // 1. 경고 표시 시작
        if (circleOutline) circleOutline.enabled = true;
        if (circleFill)
        {
            circleFill.enabled = true;

            // 프로퍼티 블록을 사용하여 Fill Amount 리셋
            circleFill.GetPropertyBlock(propBlock);
            propBlock.SetFloat("_FillAmount", 0);
            circleFill.SetPropertyBlock(propBlock);
        }

        // 2. 색상 채우기 (경고 시간 동안)
        float timer = 0f;
        while (timer < warningDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / warningDuration;
            if (circleFill)
            {
                // MaterialPropertyBlock을 사용하여 _FillAmount 값을 조절
                circleFill.GetPropertyBlock(propBlock);
                propBlock.SetFloat("_FillAmount", progress);
                circleFill.SetPropertyBlock(propBlock);
            }
            yield return null;
        }

        // 3. 폭발 (데미지 처리)
        Explode();

        // 4. 시각 효과 정리
        if (circleOutline) circleOutline.enabled = false;
        if (circleFill) circleFill.enabled = false;

        // 5. 공용 폭발 이펙트 생성
        GameObject explosion = GameManager.instance.pool.Get(7);
        explosion.transform.position = transform.position;

        // 6. 풀로 돌아가기 위해 비활성화
        gameObject.SetActive(false);
    }

    private void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in colliders)
        {
            if (hit.CompareTag("Player"))
            {
                playerstat playerStat = hit.GetComponent<playerstat>();
                if (playerStat != null)
                {
                    // playerStat.hp_reduce(damage); // 테스트를 위해 임시 주석 처리
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    private IEnumerator BobbingEffect()
    {
        if (spriteMask == null)
        {
            yield break;
        }

        float timer = 0f;
        Vector3 originalMaskPosition = spriteMask.transform.localPosition;

        while (timer < warningDuration)
        {
            timer += Time.deltaTime;

            // Mathf.Sin을 사용하여 위아래로 부드럽게 움직이는 효과 생성
            float newY = originalMaskPosition.y + Mathf.Sin(timer * bobbingSpeed) * bobbingHeight;
            spriteMask.transform.localPosition = new Vector3(
                originalMaskPosition.x,
                newY,
                originalMaskPosition.z
            );

            yield return null;
        }

        // 애니메이션이 끝나면 마스크 위치를 원래대로 복원
        spriteMask.transform.localPosition = originalMaskPosition;
    }
}