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
    [SerializeField] private float spawnHeight = 1f; // ���� ����

    private void Awake()
    {
        // �̱��� ����
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
        // �ʱ� ������ ����
        foreach (var pool in spawnPools)
        {
            FillPool(pool);
        }
    }

    // Ǯ �� ������ ��� ä���
    private void FillPool(SpawnPool pool)
    {
        int itemsToAdd = pool.maxItemCount - pool.spawnedItems.Count;
        for (int i = 0; i < itemsToAdd; i++)
        {
            SpawnItemInPool(pool);
        }
    }

    // Ư�� Ǯ���� ������ �����ϱ�
    private void SpawnItemInPool(SpawnPool pool)
    {
        // �̹� �ִ� ������ŭ �����ƴ��� Ȯ��
        if (pool.spawnedItems.Count >= pool.maxItemCount)
        {
            return;
        }

        // ������ ������ ���� (Ȯ�� ���)
        ItemSpawnInfo selectedItem = GetRandomItemFromPool(pool);
        if (selectedItem == null) return;

        // ���� ��ġ ���
        float randomX = Random.Range(-spawnAreaXZ.x, spawnAreaXZ.x);
        float randomZ = Random.Range(-spawnAreaXZ.y, spawnAreaXZ.y);
        Vector3 spawnPosition = new Vector3(randomX, spawnHeight, randomZ);

        // ������ �ν��Ͻ� ����
        GameObject newItem = Instantiate(selectedItem.itemPrefab, spawnPosition, Quaternion.identity);

        // ������ ��Ʈ�ѷ� ���� ������Ʈ
        ItemController itemController = newItem.GetComponent<ItemController>();
        if (itemController != null && selectedItem.itemData != null)
        {
            itemController.SetItemData(selectedItem.itemData);
        }

        // ������ ������ ����
        pool.spawnedItems.Add(newItem);
    }

    // Ȯ�� ������� Ǯ���� ������ ����
    private ItemSpawnInfo GetRandomItemFromPool(SpawnPool pool)
    {
        if (pool.itemsInPool.Count == 0) return null;

        // �� Ȯ�� ���
        float totalProbability = 0;
        foreach (var item in pool.itemsInPool)
        {
            totalProbability += item.spawnProbability;
        }

        // Ȯ���� 0�̸� ��ȯ ����
        if (totalProbability <= 0) return null;

        // ���� �� ����
        float randomValue = Random.Range(0, totalProbability);
        float cumulativeProbability = 0;

        // Ȯ���� ���� ������ ����
        foreach (var item in pool.itemsInPool)
        {
            cumulativeProbability += item.spawnProbability;
            if (randomValue <= cumulativeProbability)
            {
                return item;
            }
        }

        // �⺻ ��ȯ�� (ù ��° ������)
        return pool.itemsInPool[0];
    }

    // ������ ���/���� �� ���� ť�� ���
    public void QueueItemRespawn(GameObject itemObject, ItemData itemData)
    {
        // � Ǯ�� ���ϴ��� Ȯ��
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
            // ������ ���� ã��
            ItemSpawnInfo matchingInfo = null;
            foreach (var item in targetPool.itemsInPool)
            {
                if (item.itemData == itemData)
                {
                    matchingInfo = item;
                    break;
                }
            }

            // ���� ��Ͽ��� ����
            targetPool.spawnedItems.Remove(itemObject);

            // ������ �� ���ο� ������ ����
            if (matchingInfo != null)
            {
                StartCoroutine(RespawnAfterDelay(targetPool, matchingInfo.spawnDelay));
            }
            else
            {
                // ��ġ�ϴ� ������ ������ �⺻ ������ ���
                StartCoroutine(RespawnAfterDelay(targetPool, 5f));
            }
        }

        // ���� ������ ����
        Destroy(itemObject);
    }

    private IEnumerator RespawnAfterDelay(SpawnPool pool, float delay)
    {
        // ������ �����̸�ŭ ���
        yield return new WaitForSeconds(delay);

        // ���ο� ������ ����
        SpawnItemInPool(pool);
    }

    // ��� Ǯ üũ �� �ʿ�� ����
    public void CheckAndSpawnItems()
    {
        foreach (var pool in spawnPools)
        {
            // ��ȿ���� ���� �׸� ����
            pool.spawnedItems.RemoveAll(i => i == null);

            // �ִ� ������ �������� �ʾ����� ����
            FillPool(pool);
        }
    }

    // ����׿� - �ʿ� �ִ� ������ Ȯ��
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(0, spawnHeight, 0),
            new Vector3(spawnAreaXZ.x * 2, 0.1f, spawnAreaXZ.y * 2));
    }
}
