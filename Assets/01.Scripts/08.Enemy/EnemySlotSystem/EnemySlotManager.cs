using System.Collections.Generic;
using UnityEngine;

public class EnemySlotManager : MonoBehaviour
{
    [Header("Slots")]
    [SerializeField] private EnemySlot[] slots;

    [Header("Move Option")]
    [SerializeField] private float moveSpeed = 20f;

    [Header("Overflow Option")]
    [SerializeField] private bool destroyOverflowEnemy = true;

    private Dictionary<EnemyLane, List<EnemySlot>> slotMap =
        new Dictionary<EnemyLane, List<EnemySlot>>();

    private Dictionary<EnemyLane, List<SlotEnemy>> enemyMap =
        new Dictionary<EnemyLane, List<SlotEnemy>>();

    private void Awake()
    {
        InitSlots();
    }

    private void Update()
    {
        MoveEnemiesToSlots();
    }

    private void InitSlots()
    {
        slotMap.Clear();
        enemyMap.Clear();

        foreach (EnemyLane lane in System.Enum.GetValues(typeof(EnemyLane)))
        {
            slotMap.Add(lane, new List<EnemySlot>());
            enemyMap.Add(lane, new List<SlotEnemy>());
        }

        foreach (EnemySlot slot in slots)
        {
            if (slot == null) continue;

            slotMap[slot.lane].Add(slot);
        }

        foreach (EnemyLane lane in slotMap.Keys)
        {
            slotMap[lane].Sort((a, b) => a.slotIndex.CompareTo(b.slotIndex));

            enemyMap[lane].Clear();

            for (int i = 0; i < slotMap[lane].Count; i++)
            {
                enemyMap[lane].Add(null);
            }
        }
    }

    public GameObject SpawnEnemy(GameObject enemyPrefab, EnemyLane lane, bool isSpawnByBoss = false)
    {
        if (enemyPrefab == null) return null;
        if (!slotMap.ContainsKey(lane)) return null;
        if (!enemyMap.ContainsKey(lane)) return null;

        List<EnemySlot> laneSlots = slotMap[lane];
        List<SlotEnemy> laneEnemies = enemyMap[lane];

        if (laneSlots.Count == 0) return null;

        PushLineLeft(lane);

        int spawnIndex = isSpawnByBoss ? 1 : 0;
        EnemySlot spawnSlot = laneSlots[spawnIndex];

        Vector3 spawnPosition =
            spawnSlot.transform.position + GetPrefabSlotOffset(enemyPrefab);

        GameObject enemyObj = Instantiate(
            enemyPrefab,
            spawnPosition,
            Quaternion.identity
        );

        SlotEnemy slotEnemy = enemyObj.GetComponent<SlotEnemy>();

        if (!slotEnemy && enemyObj.GetComponent<EnemyHp>())
        {
            slotEnemy = enemyObj.AddComponent<SlotEnemy>();
        }

        if (slotEnemy)
        {
            slotEnemy.Init(this, lane, spawnIndex);
            laneEnemies[spawnIndex] = slotEnemy;
        }

        if (EveMemoryManager.Instance != null)
        {
            EveMemoryManager.Instance.UpdateAccumulatedMonsterCount(GetMaxLaneEnemyCount());
        }

        return enemyObj;
    }

    private void PushLineLeft(EnemyLane lane)
    {
        List<SlotEnemy> laneEnemies = enemyMap[lane];

        if (laneEnemies.Count == 0) return;

        int lastIndex = laneEnemies.Count - 1;

        if (laneEnemies[lastIndex] != null)
        {
            if (destroyOverflowEnemy)
            {
                Destroy(laneEnemies[lastIndex].gameObject);
            }

            laneEnemies[lastIndex] = null;
        }

        for (int i = lastIndex - 1; i >= 0; i--)
        {
            SlotEnemy enemy = laneEnemies[i];

            if (enemy == null) continue;

            laneEnemies[i + 1] = enemy;
            laneEnemies[i] = null;

            enemy.SetSlot(lane, i + 1);
        }
    }

    private void MoveEnemiesToSlots()
    {
        foreach (EnemyLane lane in enemyMap.Keys)
        {
            List<SlotEnemy> laneEnemies = enemyMap[lane];
            List<EnemySlot> laneSlots = slotMap[lane];

            for (int i = 0; i < laneEnemies.Count; i++)
            {
                SlotEnemy enemy = laneEnemies[i];

                if (enemy == null) continue;

                Vector3 targetPosition =
                    laneSlots[i].transform.position + GetEnemySlotOffset(enemy);

                enemy.transform.position = Vector3.MoveTowards(
                    enemy.transform.position,
                    targetPosition,
                    moveSpeed * Time.deltaTime
                );
            }
        }
    }

    private Vector3 GetEnemySlotOffset(SlotEnemy enemy)
    {
        EnemySlotOffset offset = enemy.GetComponent<EnemySlotOffset>();

        if (offset == null)
            return Vector3.zero;

        return offset.slotOffset;
    }

    private Vector3 GetPrefabSlotOffset(GameObject enemyPrefab)
    {
        EnemySlotOffset offset = enemyPrefab.GetComponent<EnemySlotOffset>();

        if (offset == null)
            return Vector3.zero;

        return offset.slotOffset;
    }

    public void UnregisterEnemy(SlotEnemy enemy)
    {
        if (enemy == null) return;

        EnemyLane lane = enemy.CurrentLane;
        int index = enemy.CurrentSlotIndex;

        if (!enemyMap.ContainsKey(lane)) return;
        if (index < 0 || index >= enemyMap[lane].Count) return;

        if (enemyMap[lane][index] == enemy)
        {
            enemyMap[lane][index] = null;
        }
    }

    public int GetMaxLaneEnemyCount()
    {
        int maxCount = 0;

        foreach (EnemyLane lane in enemyMap.Keys)
        {
            int count = 0;

            foreach (SlotEnemy enemy in enemyMap[lane])
            {
                if (enemy != null)
                {
                    count++;
                }
            }

            if (count > maxCount)
            {
                maxCount = count;
            }
        }

        return maxCount;
    }
}