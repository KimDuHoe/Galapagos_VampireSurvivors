using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour
{
    [Tooltip("폭발 애니메이션의 길이(초)")]
    public float duration = 0.5f;

    void OnEnable()
    {
        StartCoroutine(DisableAfterAnimation());
    }

    private IEnumerator DisableAfterAnimation()
    {
        // 애니메이션 재생이 끝날 때까지 기다림
        yield return new WaitForSeconds(duration);

        // 오브젝트 풀로 돌아가기 위해 비활성화
        gameObject.SetActive(false);
    }
}
