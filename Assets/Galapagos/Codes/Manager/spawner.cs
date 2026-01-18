using UnityEngine;

public class Spawner : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnData
    {
        public int poolIndex; // PoolManager에 등록된 프리팹의 인덱스
        public float weight;  // 스폰 가중치 (높을수록 잘 나옴)
    }

    [Header("타겟 및 감지")]
    public Transform[] spawnPoint;
    public SpawnData[] spawnData; // 몬스터별 스폰 가중치 설정
    public LayerMask spawnLayerMask; // 겹침을 확인할 레이어
    public float spawnRate = 0.8f;  // 전체 스폰 주기
    public float spawnCheckRadius = 2.0f; // 스폰 위치 겹침 체크 반경

    private float totalWeight;  // 전체 가중치 합계(몬스터별 가중치 합)
    float timer;

    private void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
        CalculateTotalWeight();
    }

    // 인스펙터에서 값이 변경될 때마다 호출 (에디터 활용 가능)
    void OnValidate()
    {
        CalculateTotalWeight();
    }

    // 몹 스폰율 가중치 설정
    void CalculateTotalWeight()
    {
        totalWeight = 0;
        if (spawnData == null) return;
        foreach (var data in spawnData)
        {
            totalWeight += data.weight;
        }
    }

    private void Update()
    {
        if (!GameManager.instance.isLive) return;

        timer += Time.deltaTime;

        if (timer > spawnRate)
        {
            timer = 0;
            Spawn();
        }
    }

    void Spawn()
    {
        if (spawnPoint.Length <= 1 || spawnData == null || spawnData.Length == 0 || totalWeight <= 0) return;

        Transform spawnLocation = spawnPoint[Random.Range(1, spawnPoint.Length)];

        Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnLocation.position, spawnCheckRadius, spawnLayerMask);
        if (colliders.Length > 0)
        {
            return;
        }

        // 가중치 기반으로 스폰할 몬스터의 PoolManager 인덱스 결정
        int poolIndex = GetRandomPoolIndex();

        // 인덱스가 유효하면 풀에서 오브젝트를 가져와 스폰 위치에 배치
        if (poolIndex != -1)
        {
            GameObject instant = GameManager.instance.pool.Get(poolIndex);
            instant.transform.position = spawnLocation.position;
        }
    }

    // 가중치 기반으로 랜덤 인덱스 선택
    int GetRandomPoolIndex()
    {
        float randomValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0;

        foreach (var data in spawnData)
        {
            cumulativeWeight += data.weight;
            if (randomValue < cumulativeWeight)
            {
                return data.poolIndex;
            }
        }
        return -1; // Should not happen
    }
}
