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
    [SerializeField] private List<float> spawnHeights = new List<float> { 1f }; // 스폰 높이

    private void Awake()
    {
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
        foreach (var pool in spawnPools)
        {
            FillPool(pool);
        }
    }

    /// <summary>
    /// 풀 내 아이템 모두 채우기
    /// </summary>
    private void FillPool(SpawnPool pool)
    {
        int itemsToAdd = pool.maxItemCount - pool.spawnedItems.Count;
        for (int i = 0; i < itemsToAdd; i++)
        {
            SpawnItemInPool(pool);
        }
    }

    /// <summary>
    /// 특정 풀에서 아이템 스폰하기
    /// </summary>
    private void SpawnItemInPool(SpawnPool pool)
    {
        if (pool.spawnedItems.Count >= pool.maxItemCount)
        {
            return;
        }

        // 스폰할 아이템 선택 (확률 계산)
        ItemSpawnInfo selectedItem = GetRandomItemFromPool(pool);
        if (selectedItem == null) return;

        float randomX = Random.Range(-spawnAreaXZ.x, spawnAreaXZ.x);
        float randomZ = Random.Range(-spawnAreaXZ.y, spawnAreaXZ.y);
        float randomHeight = GetRandomSpawnHeight();
        Vector3 spawnPosition = new Vector3(randomX, randomHeight, randomZ);

        GameObject newItem = Instantiate(selectedItem.itemPrefab, spawnPosition, Quaternion.identity);

        ItemController itemController = newItem.GetComponent<ItemController>();
        if (itemController != null && selectedItem.itemData != null)
        {
            itemController.SetItemData(selectedItem.itemData);
        }

        pool.spawnedItems.Add(newItem);
    }

    /// <summary>
    /// 랜덤 스폰 높이 계산
    /// </summary>
    private float GetRandomSpawnHeight()
    {
        if (spawnHeights.Count == 0)
        {
            return 1f;
        }

        int randomIndex = Random.Range(0, spawnHeights.Count);
        return spawnHeights[randomIndex];
    }

    /// <summary>
    /// 확률 기반으로 풀에서 아이템 선택
    /// </summary>
    private ItemSpawnInfo GetRandomItemFromPool(SpawnPool pool)
    {
        if (pool.itemsInPool.Count == 0) return null;

        float totalProbability = 0;
        foreach (var item in pool.itemsInPool)
        {
            totalProbability += item.spawnProbability;
        }

        if (totalProbability <= 0) return null;

        float randomValue = Random.Range(0, totalProbability);
        float cumulativeProbability = 0;

        foreach (var item in pool.itemsInPool)
        {
            cumulativeProbability += item.spawnProbability;
            if (randomValue <= cumulativeProbability)
            {
                return item;
            }
        }

        return pool.itemsInPool[0];
    }

    /// <summary>
    /// 아이템 사용/제거 후 스폰 큐에 등록
    /// </summary>
    public void QueueItemRespawn(GameObject itemObject, ItemData itemData)
    {
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
            ItemSpawnInfo matchingInfo = null;
            foreach (var item in targetPool.itemsInPool)
            {
                if (item.itemData == itemData)
                {
                    matchingInfo = item;
                    break;
                }
            }

            targetPool.spawnedItems.Remove(itemObject);

            if (matchingInfo != null)
            {
                StartCoroutine(RespawnAfterDelay(targetPool, matchingInfo.spawnDelay));
            }
            else
            {
                StartCoroutine(RespawnAfterDelay(targetPool, 5f));
            }
        }

        Destroy(itemObject);
    }

    /// <summary>
    /// 딜레이 후 아이템 스폰
    /// </summary>
    private IEnumerator RespawnAfterDelay(SpawnPool pool, float delay)
    {
        yield return new WaitForSeconds(delay);

        SpawnItemInPool(pool);
    }

    /// <summary>
    /// 모든 풀 체크 및 필요시 스폰
    /// </summary>
    public void CheckAndSpawnItems()
    {
        foreach (var pool in spawnPools)
        {
            pool.spawnedItems.RemoveAll(i => i == null);
            FillPool(pool);
        }
    }
}
