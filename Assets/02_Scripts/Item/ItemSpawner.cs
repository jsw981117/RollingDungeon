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
        public int maxItemCount = 5; // �ʿ� ������ �� �ִ� �ִ� ������ ����
        public float spawnDelay = 5f; // ������ ���� ������
        public List<GameObject> spawnedItems = new List<GameObject>(); // ������ ������ ����
    }

    [SerializeField] private List<ItemSpawnInfo> itemsToSpawn = new List<ItemSpawnInfo>();
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
        foreach (var item in itemsToSpawn)
        {
            for (int i = 0; i < item.maxItemCount; i++)
            {
                SpawnItem(item);
            }
        }
    }

    // Ư�� ������ Ÿ�� �����ϱ�
    public void SpawnItem(ItemSpawnInfo itemInfo)
    {
        // �̹� �ִ� ������ŭ �����Ǿ����� Ȯ��
        if (itemInfo.spawnedItems.Count >= itemInfo.maxItemCount)
        {
            return;
        }

        // ���� Ȯ�� üũ
        if (Random.Range(0f, 100f) > itemInfo.spawnProbability)
        {
            return;
        }

        // ���� ��ġ ���
        float randomX = Random.Range(-spawnAreaXZ.x, spawnAreaXZ.x);
        float randomZ = Random.Range(-spawnAreaXZ.y, spawnAreaXZ.y);
        Vector3 spawnPosition = new Vector3(randomX, spawnHeight, randomZ);

        // ������ �ν��Ͻ� ����
        GameObject newItem = Instantiate(itemInfo.itemPrefab, spawnPosition, Quaternion.identity);

        // ������ ��Ʈ�ѷ� ���� ������Ʈ
        ItemController itemController = newItem.GetComponent<ItemController>();
        if (itemController != null && itemInfo.itemData != null)
        {
            itemController.SetItemData(itemInfo.itemData);
        }

        // ������ ������ ����
        itemInfo.spawnedItems.Add(newItem);
    }

    // ������ ���/���� �� ���� ť�� ���
    public void QueueItemRespawn(GameObject itemObject, ItemData itemData)
    {
        // �ش� ������ ���� ã��
        ItemSpawnInfo matchingInfo = itemsToSpawn.Find(info => info.itemData == itemData);
        if (matchingInfo != null)
        {
            // ���� ��Ͽ��� ����
            matchingInfo.spawnedItems.Remove(itemObject);

            // ������ �� ���ο� ������ ����
            StartCoroutine(RespawnAfterDelay(matchingInfo));
        }

        // ���� ������ ����
        Destroy(itemObject);
    }

    private IEnumerator RespawnAfterDelay(ItemSpawnInfo itemInfo)
    {
        // ������ �����̸�ŭ ���
        yield return new WaitForSeconds(itemInfo.spawnDelay);

        // ���ο� ������ ����
        SpawnItem(itemInfo);
    }

    // ������ ���� Ȯ�� �� �ʿ�� ����
    public void CheckAndSpawnItems()
    {
        foreach (var item in itemsToSpawn)
        {
            // ��ȿ���� ���� �׸� ����
            item.spawnedItems.RemoveAll(i => i == null);

            // �ִ� ������ �������� �ʾ����� ����
            int itemsToAdd = item.maxItemCount - item.spawnedItems.Count;
            for (int i = 0; i < itemsToAdd; i++)
            {
                StartCoroutine(RespawnAfterDelay(item));
            }
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
