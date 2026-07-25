using System;
using System.Collections.Generic;
using _01.Scripts._00.Manager;
using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Database")]
    [SerializeField] private StageWaveDatabase stageWaveDatabase;

    [Header("Runtime Wave Data")]
    [SerializeField] private StageWaveData stageWaveData;

    [Header("Systems")]
    [SerializeField] private EnemySlotManager enemySlotManager;

    [Header("Clear UI")]
    [SerializeField] private GameObject clearPanel;
    [SerializeField] private TMP_Text earnedGoldText;
    [SerializeField] private TMP_Text earnedEveMemoryText;

    private List<ScheduledSpawn> scheduledSpawns = new List<ScheduledSpawn>();

    private float elapsedTime;
    private int currentSpawnIndex;
    private bool isPlaying;
    private bool isClearChecked;
    private bool isGameOver;

    private void Start()
    {
        LoadSelectedStageWaveData();

        BuildSchedule();
        StartStage();
    }

    private void Update()
    {
        if (!isPlaying) return;

        elapsedTime += Time.deltaTime;

        while (currentSpawnIndex < scheduledSpawns.Count &&
               elapsedTime >= scheduledSpawns[currentSpawnIndex].spawnTime)
        {
            Spawn(scheduledSpawns[currentSpawnIndex]);
            currentSpawnIndex++;
        }

        CheckStageClear();
    }

    private void LoadSelectedStageWaveData()
    {
        if (stageWaveDatabase == null)
        {
            Debug.LogError("StageWaveDatabase�� WaveManager�� ������� �ʾҽ��ϴ�.");
            return;
        }

        if (StageManager.Instance == null)
        {
            Debug.LogError("StageManager.Instance�� �����ϴ�.");
            return;
        }

        WorldNum selectedWorldNum = StageManager.Instance.selectedWorldNum;
        StageNum selectedStageNum = StageManager.Instance.selectedStageNum;

        stageWaveData = stageWaveDatabase.GetWaveData(selectedWorldNum, selectedStageNum);

        if (stageWaveData == null)
        {
            Debug.LogError($"StageWaveData �ε� ����. World: {selectedWorldNum}, Stage: {selectedStageNum}");
            return;
        }

        Debug.Log($"StageWaveData �ε� ����. World: {selectedWorldNum}, Stage: {selectedStageNum}, Data: {stageWaveData.name}");
    }

    public void StartStage()
    {
        elapsedTime = 0f;
        currentSpawnIndex = 0;

        isPlaying = true;
        isClearChecked = false;
        isGameOver = false;

        if (clearPanel != null)
        {
            clearPanel.SetActive(false);
        }

        if (GoldManager.Instance != null)
        {
            GoldManager.Instance.ResetStageGold();
        }

        if (EveMemoryManager.Instance != null)
        {
            EveMemoryManager.Instance.ResetStageRecord();
        }

        Time.timeScale = 1f;
    }

    private void BuildSchedule()
    {
        scheduledSpawns.Clear();

        if (stageWaveData == null)
        {
            return;
        }

        foreach (WaveTimelineData timeline in stageWaveData.waveTimelines)
        {
            if (timeline == null) continue;

            foreach (SpawnEventData spawnEvent in timeline.spawnEvents)
            {
                if (spawnEvent == null || spawnEvent.prefab == null) continue;

                scheduledSpawns.Add(new ScheduledSpawn
                {
                    prefab = spawnEvent.prefab,
                    lane = spawnEvent.lane,
                    spawnTime = timeline.baseTime + spawnEvent.delayFromBaseTime
                });
            }
        }

        scheduledSpawns.Sort((a, b) => a.spawnTime.CompareTo(b.spawnTime));
    }

    private void Spawn(ScheduledSpawn scheduled)
    {
        if (enemySlotManager == null)
        {
            return;
        }

        enemySlotManager.SpawnEnemy(
            scheduled.prefab,
            scheduled.lane
        );
    }

    private void CheckStageClear()
    {
        if (isClearChecked) return;

        bool allWavesSpawned = currentSpawnIndex >= scheduledSpawns.Count;

        if (!allWavesSpawned) return;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length == 0)
        {
            StageClear();
        }
    }

    private void StageClear()
    {
        isClearChecked = true;
        isPlaying = false;

        int earnedGold = 0;
        int earnedEveMemory = 0;

        if (GoldManager.Instance != null)
        {
            earnedGold = GoldManager.Instance.ApplyClearGold();
        }

        if (EveMemoryManager.Instance != null)
        {
            earnedEveMemory = EveMemoryManager.Instance.ApplyStageClearMemory();
        }

        MarkCurrentStageCleared();

        UnlockCharacterAndWeapon(
            StageManager.Instance.selectedWorldNum,
            StageManager.Instance.selectedStageNum
        );

        ShowClearPanel(earnedGold, earnedEveMemory);

        Debug.Log("스테이지 클리어");
    }

    private void MarkCurrentStageCleared()
    {
        if (GameManager.Instance == null)
            return;

        WorldNum worldNum = StageManager.Instance.selectedWorldNum;
        StageNum stageNum = StageManager.Instance.selectedStageNum;

        GameManager.Instance.playerData
            .clearedStages[(int)worldNum]
            .stages[(int)stageNum]
            .isCleared = true;

        GameManager.Instance.SaveGame();
    }

    private void UnlockCharacterAndWeapon(WorldNum worldNum, StageNum stageNum)
    {
        if (worldNum == WorldNum.World0)
        {
            var characterList = GameManager.Instance.playerData.unlockedCharacters;
            var weaponList = GameManager.Instance.playerData.unlockedWeapons;
            switch (stageNum)
            {
                case StageNum.Stage0:
                    characterList[2] = true;
                    weaponList[1] = true;
                    break;
                case StageNum.Stage1:
                    characterList[3] = true;
                    weaponList[2] = true;
                    break;
                case StageNum.Stage2:
                    weaponList[3] = true;
                    break;
                case StageNum.Stage3:
                    characterList[4] = true;
                    weaponList[4] = true;
                    break;
                case StageNum.Stage4:
                    characterList[5] = true;
                    weaponList[5] = true;
                    break;
                case StageNum.Stage5:
                    break;
            }
            
            GameManager.Instance.SaveGame();
        }
    }

    private void ShowClearPanel(int earnedGold, int earnedEveMemory)
    {
        if (clearPanel != null)
        {
            clearPanel.SetActive(true);
            clearPanel.transform.SetAsLastSibling();
        }

        if (earnedGoldText != null)
        {
            earnedGoldText.text = $"획득 골드: {earnedGold}";
        }

        if (earnedEveMemoryText != null)
        {
            earnedEveMemoryText.text = $"이브의 기억: {earnedEveMemory}";
        }

        Time.timeScale = 0f;
    }

    private class ScheduledSpawn
    {
        public GameObject prefab;
        public EnemyLane lane;
        public float spawnTime;
    }
}