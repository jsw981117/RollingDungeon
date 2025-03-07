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
        public int maxItemCount = 5; // 맵에 존재할 수 있는 최대 아이템 개수
        public float spawnDelay = 5f; // 아이템 스폰 딜레이
        public List<GameObject> spawnedItems = new List<GameObject>(); // 스폰된 아이템 추적
    }

    [SerializeField] private List<ItemSpawnInfo> itemsToSpawn = new List<ItemSpawnInfo>();
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
        foreach (var item in itemsToSpawn)
        {
            for (int i = 0; i < item.maxItemCount; i++)
            {
                SpawnItem(item);
            }
        }
    }

    // 특정 아이템 타입 스폰하기
    public void SpawnItem(ItemSpawnInfo itemInfo)
    {
        // 이미 최대 개수만큼 스폰되었는지 확인
        if (itemInfo.spawnedItems.Count >= itemInfo.maxItemCount)
        {
            return;
        }

        // 스폰 확률 체크
        if (Random.Range(0f, 100f) > itemInfo.spawnProbability)
        {
            return;
        }

        // 랜덤 위치 계산
        float randomX = Random.Range(-spawnAreaXZ.x, spawnAreaXZ.x);
        float randomZ = Random.Range(-spawnAreaXZ.y, spawnAreaXZ.y);
        Vector3 spawnPosition = new Vector3(randomX, spawnHeight, randomZ);

        // 아이템 인스턴스 생성
        GameObject newItem = Instantiate(itemInfo.itemPrefab, spawnPosition, Quaternion.identity);

        // 아이템 컨트롤러 참조 업데이트
        ItemController itemController = newItem.GetComponent<ItemController>();
        if (itemController != null && itemInfo.itemData != null)
        {
            itemController.SetItemData(itemInfo.itemData);
        }

        // 생성된 아이템 추적
        itemInfo.spawnedItems.Add(newItem);
    }

    // 아이템 사용/제거 후 스폰 큐에 등록
    public void QueueItemRespawn(GameObject itemObject, ItemData itemData)
    {
        // 해당 아이템 정보 찾기
        ItemSpawnInfo matchingInfo = itemsToSpawn.Find(info => info.itemData == itemData);
        if (matchingInfo != null)
        {
            // 추적 목록에서 제거
            matchingInfo.spawnedItems.Remove(itemObject);

            // 딜레이 후 새로운 아이템 스폰
            StartCoroutine(RespawnAfterDelay(matchingInfo));
        }

        // 원본 아이템 제거
        Destroy(itemObject);
    }

    private IEnumerator RespawnAfterDelay(ItemSpawnInfo itemInfo)
    {
        // 설정된 딜레이만큼 대기
        yield return new WaitForSeconds(itemInfo.spawnDelay);

        // 새로운 아이템 스폰
        SpawnItem(itemInfo);
    }

    // 아이템 개수 확인 및 필요시 스폰
    public void CheckAndSpawnItems()
    {
        foreach (var item in itemsToSpawn)
        {
            // 유효하지 않은 항목 제거
            item.spawnedItems.RemoveAll(i => i == null);

            // 최대 개수에 도달하지 않았으면 스폰
            int itemsToAdd = item.maxItemCount - item.spawnedItems.Count;
            for (int i = 0; i < itemsToAdd; i++)
            {
                StartCoroutine(RespawnAfterDelay(item));
            }
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
