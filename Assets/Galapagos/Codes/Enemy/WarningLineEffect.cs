using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class WarningLineEffect : MonoBehaviour
{
    [Tooltip("선이 채워지는 데 걸리는 시간")]
    public float fillDuration = 1.5f;

    [Header("부모 추적")]
    public Transform parentCrab; // 이 경고선을 생성한 DrillCrab

    private LineRenderer lineRenderer;
    private MaterialPropertyBlock propBlock;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        propBlock = new MaterialPropertyBlock();

        // --- 기본 설정 ---
        lineRenderer.useWorldSpace = false; // 부모 오브젝트를 따라다니도록 로컬 공간 사용

        if (lineRenderer.startWidth <= 0.01f)
        {
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
        }
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, new Vector3(10, 0, 0)); // 길이는 10으로 기본 설정
    }

    void Update()
    {
        // 부모가 사라지면 (비활성화되거나 파괴되면) 스스로를 파괴
        if (parentCrab == null || !parentCrab.gameObject.activeInHierarchy)
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        if (lineRenderer == null)
        {
            Debug.LogError("[WarningLineEffect] LineRenderer component is NULL.", gameObject);
            return;
        }
        StartCoroutine(FillLine());
    }

    private IEnumerator FillLine()
    {
        float elapsedTime = 0f;

        // 시작 시 항상 0으로 초기화
        lineRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_FillAmount", 0);
        lineRenderer.SetPropertyBlock(propBlock);

        while (elapsedTime < fillDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fillDuration;

            // MaterialPropertyBlock을 사용하여 _FillAmount 값을 조절
            lineRenderer.GetPropertyBlock(propBlock);
            propBlock.SetFloat("_FillAmount", progress);
            lineRenderer.SetPropertyBlock(propBlock);

            yield return null;
        }

        // 완료 시 1로 설정 (오차 방지)
        lineRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_FillAmount", 1);
        lineRenderer.SetPropertyBlock(propBlock);
    }
}