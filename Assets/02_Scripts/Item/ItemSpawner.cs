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
        public float spawnDelay = 5f; // ������ ���� ������
    }

    [System.Serializable]
    public class SpawnPool
    {
        public string poolName = "Pool";
        public int maxItemCount = 5; // �ʿ� ������ �� �ִ� �ִ� ������ ����
        public List<ItemSpawnInfo> itemsInPool = new List<ItemSpawnInfo>();
        public List<GameObject> spawnedItems = new List<GameObject>(); // ���� �ʿ� ������ ������ ����
    }

    [SerializeField] private List<SpawnPool> spawnPools = new List<SpawnPool>();
    [SerializeField] private Vector2 spawnAreaXZ = new Vector2(20f, 20f); // ���� ����
    [SerializeField] private List<float> spawnHeights = new List<float> { 1f }; // ���� ����

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
    /// Ǯ �� ������ ��� ä���
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
    /// Ư�� Ǯ���� ������ �����ϱ�
    /// </summary>
    private void SpawnItemInPool(SpawnPool pool)
    {
        if (pool.spawnedItems.Count >= pool.maxItemCount)
        {
            return;
        }

        // ������ ������ ���� (Ȯ�� ���)
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
    /// ���� ���� ���� ���
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
    /// Ȯ�� ������� Ǯ���� ������ ����
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
    /// ������ ���/���� �� ���� ť�� ���
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
    /// ������ �� ������ ����
    /// </summary>
    private IEnumerator RespawnAfterDelay(SpawnPool pool, float delay)
    {
        yield return new WaitForSeconds(delay);

        SpawnItemInPool(pool);
    }

    /// <summary>
    /// ��� Ǯ üũ �� �ʿ�� ����
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
