using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner Instance { get; private set; }

    [System.Serializable]
    public class ItemSpawnInfo
    {
        public GameObject itemPrefab;
        public ItemData itemData;
        [Range(0, 100)]
        public float spawnProbability = 100f;
        public float spawnDelay = 5f; // 아이템 스폰 딜레이
    }

    [System.Serializable]
    public class SpawnPool
    {
        public string poolName = "Pool";
        public int maxItemCount = 5; // 맵에 존재할 수 있는 최대 아이템 개수
        public List<ItemSpawnInfo> itemsInPool = new List<ItemSpawnInfo>();
        public List<GameObject> spawnedItems = new List<GameObject>(); // 현재 맵에 스폰된 아이템 추적
    }

    [SerializeField] private List<SpawnPool> spawnPools = new List<SpawnPool>();
    [SerializeField] private Vector2 spawnAreaXZ = new Vector2(20f, 20f); // 스폰 영역
    [SerializeField] private float spawnHeight = 1f; // 스폰 높이

    private void Awake()
    {
        // 싱글톤 구현
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // 초기 아이템 스폰
        foreach (var pool in spawnPools)
        {
            FillPool(pool);
        }
    }

    // 풀 내 아이템 모두 채우기
    private void FillPool(SpawnPool pool)
    {
        int itemsToAdd = pool.maxItemCount - pool.spawnedItems.Count;
        for (int i = 0; i < itemsToAdd; i++)
        {
            SpawnItemInPool(pool);
        }
    }

    // 특정 풀에서 아이템 스폰하기
    private void SpawnItemInPool(SpawnPool pool)
    {
        // 이미 최대 개수만큼 스폰됐는지 확인
        if (pool.spawnedItems.Count >= pool.maxItemCount)
        {
            return;
        }

        // 스폰할 아이템 선택 (확률 계산)
        ItemSpawnInfo selectedItem = GetRandomItemFromPool(pool);
        if (selectedItem == null) return;

        // 랜덤 위치 계산
        float randomX = Random.Range(-spawnAreaXZ.x, spawnAreaXZ.x);
        float randomZ = Random.Range(-spawnAreaXZ.y, spawnAreaXZ.y);
        Vector3 spawnPosition = new Vector3(randomX, spawnHeight, randomZ);

        // 아이템 인스턴스 생성
        GameObject newItem = Instantiate(selectedItem.itemPrefab, spawnPosition, Quaternion.identity);

        // 아이템 컨트롤러 참조 업데이트
        ItemController itemController = newItem.GetComponent<ItemController>();
        if (itemController != null && selectedItem.itemData != null)
        {
            itemController.SetItemData(selectedItem.itemData);
        }

        // 생성된 아이템 추적
        pool.spawnedItems.Add(newItem);
    }

    // 확률 기반으로 풀에서 아이템 선택
    private ItemSpawnInfo GetRandomItemFromPool(SpawnPool pool)
    {
        if (pool.itemsInPool.Count == 0) return null;

        // 총 확률 계산
        float totalProbability = 0;
        foreach (var item in pool.itemsInPool)
        {
            totalProbability += item.spawnProbability;
        }

        // 확률이 0이면 반환 없음
        if (totalProbability <= 0) return null;

        // 랜덤 값 생성
        float randomValue = Random.Range(0, totalProbability);
        float cumulativeProbability = 0;

        // 확률에 따라 아이템 선택
        foreach (var item in pool.itemsInPool)
        {
            cumulativeProbability += item.spawnProbability;
            if (randomValue <= cumulativeProbability)
            {
                return item;
            }
        }

        // 기본 반환값 (첫 번째 아이템)
        return pool.itemsInPool[0];
    }

    // 아이템 사용/제거 후 스폰 큐에 등록
    public void QueueItemRespawn(GameObject itemObject, ItemData itemData)
    {
        // 어떤 풀에 속하는지 확인
        SpawnPool targetPool = null;
        foreach (var pool in spawnPools)
        {
            if (pool.spawnedItems.Contains(itemObject))
            {
                targetPool = pool;
                break;
            }
        }

        if (targetPool != null)
        {
            // 아이템 정보 찾기
            ItemSpawnInfo matchingInfo = null;
            foreach (var item in targetPool.itemsInPool)
            {
                if (item.itemData == itemData)
                {
                    matchingInfo = item;
                    break;
                }
            }

            // 추적 목록에서 제거
            targetPool.spawnedItems.Remove(itemObject);

            // 딜레이 후 새로운 아이템 스폰
            if (matchingInfo != null)
            {
                StartCoroutine(RespawnAfterDelay(targetPool, matchingInfo.spawnDelay));
            }
            else
            {
                // 일치하는 정보가 없으면 기본 딜레이 사용
                StartCoroutine(RespawnAfterDelay(targetPool, 5f));
            }
        }

        // 원본 아이템 제거
        Destroy(itemObject);
    }

    private IEnumerator RespawnAfterDelay(SpawnPool pool, float delay)
    {
        // 설정된 딜레이만큼 대기
        yield return new WaitForSeconds(delay);

        // 새로운 아이템 스폰
        SpawnItemInPool(pool);
    }

    // 모든 풀 체크 및 필요시 스폰
    public void CheckAndSpawnItems()
    {
        foreach (var pool in spawnPools)
        {
            // 유효하지 않은 항목 제거
            pool.spawnedItems.RemoveAll(i => i == null);

            // 최대 개수에 도달하지 않았으면 스폰
            FillPool(pool);
        }
    }

    // 디버그용 - 맵에 있는 아이템 확인
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(0, spawnHeight, 0),
            new Vector3(spawnAreaXZ.x * 2, 0.1f, spawnAreaXZ.y * 2));
    }
}
