using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteTigerRobotController : MonoBehaviour
{
    private enum PatternKind
    {
        GroundRise,
        GroundBreak,
        FallingObstacles,
        SummonMobs,
        Mines,
        Slash,
        Pillars,
        EnergyBall
    }

    [Header("Components")]
    [SerializeField] private BossHp bossHp;
    [SerializeField] private Transform player;

    [Header("Common")]
    [SerializeField] private float leftMoveSpeed = 5f;

    [Header("Pattern 1 - Ground Rise")]
    [SerializeField] private float groundRiseCooldown = 15f;
    [SerializeField] private GameObject groundRisePrefab;
    [SerializeField] private float groundRiseStartXOffset = 2.5f;
    [SerializeField] private float groundRiseTargetY = -2.5f;
    [SerializeField] private float groundRiseStartYOffset = -2f;
    [SerializeField] private float groundRiseSpeed = 5f;

    [Header("Pattern 2 - Ground Break")]
    [SerializeField] private float groundBreakCooldown = 15f;
    [SerializeField] private GameObject groundWarningPrefab;
    [SerializeField] private GameObject holePrefab;
    [SerializeField] private float groundWarningY = -3.2f;
    [SerializeField] private float holeY = -3.2f;
    [SerializeField] private float groundWarningDuration = 1f;
    [SerializeField] private float holeDuration = 3f;

    [Header("Pattern 3 - Falling Obstacles")]
    [SerializeField] private float fallingObstacleCooldown = 10f;
    [SerializeField] private GameObject headWarningPrefab;
    [SerializeField] private GameObject fallingObstaclePrefab;
    [SerializeField] private int fallingObstacleCount = 4;
    [SerializeField] private float headWarningY = 3f;
    [SerializeField] private float headWarningDuration = 1f;
    [SerializeField] private float fallingObstacleStartY = 6f;
    [SerializeField] private float fallingObstacleTargetY = 1f;
    [SerializeField] private float fallingObstacleFallSpeed = 8f;
    [SerializeField] private float fallingObstacleInterval = 0.35f;
    [SerializeField] private float fallingObstacleXInterval = 0.8f;

    [Header("Pattern 4 - Summon Mobs")]
    [SerializeField] private float summonMobCooldown = 10f;
    [SerializeField] private GameObject summonMobPrefab;
    [SerializeField] private Transform summonPointA;
    [SerializeField] private Transform summonPointB;
    [SerializeField] private Transform summonPointC;
    [SerializeField] private Transform summonPointD;
    [SerializeField] private int summonCountPerPattern = 2;
    [SerializeField] private int mobMergeCount = 4;
    [SerializeField] private float mobMergeSpeed = 8f;
    [SerializeField] private GameObject mergedMobAttackPrefab;
    [SerializeField] private float mergedMobChaseSpeed = 10f;

    [Header("Pattern 5 - Mines")]
    [SerializeField] private float mineCooldown = 10f;
    [SerializeField] private GameObject minePrefab;
    [SerializeField] private int mineCount = 3;
    [SerializeField] private float mineY = -3.2f;
    [SerializeField] private float mineMinForwardX = 1f;
    [SerializeField] private float mineMaxForwardX = 6f;

    [Header("Pattern 6 - Slash")]
    [SerializeField] private float slashCooldown = 20f;
    [SerializeField] private GameObject slashPrefab;
    [SerializeField] private Transform slashPoint;
    [SerializeField] private float slashDuration = 0.4f;

    [Header("Pattern 7 - Pillars")]
    [SerializeField] private float pillarCooldown = 10f;
    [SerializeField] private GameObject pillarPrefab;
    [SerializeField] private int pillarCount = 3;
    [SerializeField] private float pillarStartXOffset = 2.5f;
    [SerializeField] private float pillarY = -3.2f;
    [SerializeField] private float pillarIntervalX = 1.5f;

    [Header("Pattern 8 - Energy Ball")]
    [SerializeField] private float energyBallCooldown = 10f;
    [SerializeField] private GameObject chargeEffectPrefab;
    [SerializeField] private Transform chargeEffectPoint;
    [SerializeField] private GameObject energyBallPrefab;
    [SerializeField] private Transform energyBallFirePoint;
    [SerializeField] private Transform energyBallTargetPoint;
    [SerializeField] private float shortChargeDuration = 1f;
    [SerializeField] private float longChargeDuration = 2f;
    [SerializeField] private float longChargeChance = 0.5f;
    [SerializeField] private float energyBallSpeed = 12f;
    [SerializeField] private float energyBallArriveDelay = 0.5f;

    [Header("Pattern 8 - Explosion")]
    [SerializeField] private GameObject centerExplosionPrefab;
    [SerializeField] private GameObject outerExplosionPrefab;
    [SerializeField] private float explosionDuration = 1f;

    [Header("Phase")]
    [SerializeField] private float phase2HpRatio = 0.5f;

    [Header("Pattern Option")]
    [SerializeField] private float delayBetweenPatterns = 1f;

    private readonly List<PatternRuntimeData> runtimePatterns =
        new List<PatternRuntimeData>();

    private bool isPatternRunning;
    private bool isPhase2;

    private Transform[] summonPoints;
    private GameObject[] summonedMobs;

    private void Awake()
    {
        if (bossHp == null)
            bossHp = GetComponent<BossHp>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
                player = playerObj.transform;
        }

        summonPoints = new Transform[]
        {
            summonPointA,
            summonPointB,
            summonPointC,
            summonPointD
        };

        summonedMobs = new GameObject[summonPoints.Length];

        InitPatterns();
    }

    private void Start()
    {
        StartCoroutine(PatternLoop());
    }

    private void Update()
    {
        UpdateCooldowns();
        CheckPhase2Condition();
        CleanSummonedMobSlots();
    }

    private void InitPatterns()
    {
        runtimePatterns.Clear();

        runtimePatterns.Add(
            new PatternRuntimeData(PatternKind.GroundRise, groundRiseCooldown)
        );

        runtimePatterns.Add(
            new PatternRuntimeData(PatternKind.GroundBreak, groundBreakCooldown)
        );

        runtimePatterns.Add(
            new PatternRuntimeData(PatternKind.FallingObstacles, fallingObstacleCooldown)
        );

        runtimePatterns.Add(
            new PatternRuntimeData(PatternKind.SummonMobs, summonMobCooldown)
        );

        runtimePatterns.Add(
            new PatternRuntimeData(PatternKind.Mines, mineCooldown)
        );

        runtimePatterns.Add(
            new PatternRuntimeData(PatternKind.Slash, slashCooldown)
        );

        runtimePatterns.Add(
            new PatternRuntimeData(PatternKind.Pillars, pillarCooldown)
        );

        runtimePatterns.Add(
            new PatternRuntimeData(PatternKind.EnergyBall, energyBallCooldown)
        );
    }

    private void UpdateCooldowns()
    {
        foreach (PatternRuntimeData runtimePattern in runtimePatterns)
        {
            if (runtimePattern.currentCooldown > 0f)
            {
                runtimePattern.currentCooldown -= Time.deltaTime;
            }
        }
    }

    private void CheckPhase2Condition()
    {
        if (isPhase2) return;
        if (bossHp == null) return;

        float hpRatio = bossHp.currentHp / bossHp.maxHp;

        if (hpRatio <= phase2HpRatio)
        {
            isPhase2 = true;
        }
    }

    private IEnumerator PatternLoop()
    {
        while (true)
        {
            if (isPatternRunning)
            {
                yield return null;
                continue;
            }

            List<PatternRuntimeData> readyPatterns = GetReadyPatterns();

            if (readyPatterns.Count == 0)
            {
                yield return null;
                continue;
            }

            PatternRuntimeData selectedPattern =
                readyPatterns[Random.Range(0, readyPatterns.Count)];

            yield return StartCoroutine(UsePattern(selectedPattern));

            yield return new WaitForSeconds(delayBetweenPatterns);
        }
    }

    private List<PatternRuntimeData> GetReadyPatterns()
    {
        List<PatternRuntimeData> readyPatterns = new List<PatternRuntimeData>();

        foreach (PatternRuntimeData runtimePattern in runtimePatterns)
        {
            if (!isPhase2 &&
                (runtimePattern.kind == PatternKind.Pillars ||
                 runtimePattern.kind == PatternKind.EnergyBall))
            {
                continue;
            }

            if (runtimePattern.currentCooldown <= 0f)
            {
                readyPatterns.Add(runtimePattern);
            }
        }

        return readyPatterns;
    }

    private IEnumerator UsePattern(PatternRuntimeData runtimePattern)
    {
        isPatternRunning = true;

        switch (runtimePattern.kind)
        {
            case PatternKind.GroundRise:
                yield return StartCoroutine(GroundRisePattern());
                break;

            case PatternKind.GroundBreak:
                yield return StartCoroutine(GroundBreakPattern());
                break;

            case PatternKind.FallingObstacles:
                yield return StartCoroutine(FallingObstaclesPattern());
                break;

            case PatternKind.SummonMobs:
                yield return StartCoroutine(SummonMobsPattern());
                break;

            case PatternKind.Mines:
                yield return StartCoroutine(MinePattern());
                break;

            case PatternKind.Slash:
                yield return StartCoroutine(SlashPattern());
                break;

            case PatternKind.Pillars:
                yield return StartCoroutine(PillarPattern());
                break;

            case PatternKind.EnergyBall:
                yield return StartCoroutine(EnergyBallPattern());
                break;
        }

        runtimePattern.currentCooldown = runtimePattern.cooldown;

        isPatternRunning = false;
    }

    private IEnumerator GroundRisePattern()
    {
        if (groundRisePrefab == null || player == null)
            yield break;

        Vector3 targetPosition = new Vector3(
            player.position.x + groundRiseStartXOffset,
            groundRiseTargetY,
            0f
        );

        Vector3 spawnPosition = new Vector3(
            targetPosition.x,
            targetPosition.y + groundRiseStartYOffset,
            0f
        );

        GameObject ground = Instantiate(
            groundRisePrefab,
            spawnPosition,
            Quaternion.identity
        );

        yield return StartCoroutine(MoveObjectToPosition(
            ground,
            targetPosition,
            groundRiseSpeed
        ));

        AddMoveLeft(ground);
    }

    private IEnumerator GroundBreakPattern()
    {
        if (player == null)
            yield break;

        float targetX = player.position.x;

        Vector3 warningPosition = new Vector3(
            targetX,
            groundWarningY,
            0f
        );

        Vector3 holePosition = new Vector3(
            targetX,
            holeY,
            0f
        );

        GameObject warning = null;

        if (groundWarningPrefab != null)
        {
            warning = Instantiate(
                groundWarningPrefab,
                warningPosition,
                Quaternion.identity
            );
        }

        yield return new WaitForSeconds(groundWarningDuration);

        if (warning != null)
            Destroy(warning);

        GameObject hole = null;

        if (holePrefab != null)
        {
            hole = Instantiate(
                holePrefab,
                holePosition,
                Quaternion.identity
            );

            AddMoveLeft(hole);
        }

        yield return new WaitForSeconds(holeDuration);

        if (hole != null)
            Destroy(hole);
    }

    private IEnumerator FallingObstaclesPattern()
    {
        if (fallingObstaclePrefab == null || player == null)
            yield break;

        float targetX = player.position.x;

        Vector3 warningPosition = new Vector3(
            targetX,
            headWarningY,
            0f
        );

        GameObject warning = null;

        if (headWarningPrefab != null)
        {
            warning = Instantiate(
                headWarningPrefab,
                warningPosition,
                Quaternion.identity
            );
        }

        yield return new WaitForSeconds(headWarningDuration);

        if (warning != null)
            Destroy(warning);

        for (int i = 0; i < fallingObstacleCount; i++)
        {
            Vector3 spawnPosition = new Vector3(
                targetX + i * fallingObstacleXInterval,
                fallingObstacleStartY,
                0f
            );

            Vector3 targetPosition = new Vector3(
                spawnPosition.x,
                fallingObstacleTargetY,
                0f
            );

            GameObject obstacle = Instantiate(
                fallingObstaclePrefab,
                spawnPosition,
                Quaternion.identity
            );

            StartCoroutine(FallThenMoveLeft(
                obstacle,
                targetPosition,
                fallingObstacleFallSpeed
            ));

            yield return new WaitForSeconds(fallingObstacleInterval);
        }
    }

    private IEnumerator SummonMobsPattern()
    {
        CleanSummonedMobSlots();

        if (GetSummonedMobCount() >= mobMergeCount)
        {
            yield return StartCoroutine(MergeMobsAndLaunch());
            yield break;
        }

        for (int i = 0; i < summonCountPerPattern; i++)
        {
            SpawnMobToFirstEmptyPoint();
        }

        CleanSummonedMobSlots();

        if (GetSummonedMobCount() >= mobMergeCount)
        {
            yield return StartCoroutine(MergeMobsAndLaunch());
        }
    }

    private void SpawnMobToFirstEmptyPoint()
    {
        if (summonMobPrefab == null)
            return;

        for (int i = 0; i < summonPoints.Length; i++)
        {
            if (summonPoints[i] == null)
                continue;

            if (summonedMobs[i] != null)
                continue;

            GameObject mob = Instantiate(
                summonMobPrefab,
                summonPoints[i].position,
                Quaternion.identity
            );

            summonedMobs[i] = mob;
            return;
        }
    }

    private IEnumerator MergeMobsAndLaunch()
    {
        Vector3 mergePosition = GetSummonedMobCenterPosition();

        yield return StartCoroutine(MoveSummonedMobsToPosition(mergePosition));

        for (int i = 0; i < summonedMobs.Length; i++)
        {
            if (summonedMobs[i] != null)
            {
                Destroy(summonedMobs[i]);
                summonedMobs[i] = null;
            }
        }

        if (mergedMobAttackPrefab == null || player == null)
            yield break;

        GameObject mergedMob = Instantiate(
            mergedMobAttackPrefab,
            mergePosition,
            Quaternion.identity
        );

        WhiteTigerChasePlayer chase =
            mergedMob.GetComponent<WhiteTigerChasePlayer>();

        if (chase == null)
        {
            chase = mergedMob.AddComponent<WhiteTigerChasePlayer>();
        }

        chase.Init(player, mergedMobChaseSpeed);
    }

    private Vector3 GetSummonedMobCenterPosition()
    {
        Vector3 center = Vector3.zero;
        int count = 0;

        for (int i = 0; i < summonedMobs.Length; i++)
        {
            if (summonedMobs[i] == null)
                continue;

            center += summonedMobs[i].transform.position;
            count++;
        }

        if (count == 0)
            return transform.position;

        return center / count;
    }

    private IEnumerator MoveSummonedMobsToPosition(Vector3 targetPosition)
    {
        while (true)
        {
            bool allArrived = true;

            for (int i = 0; i < summonedMobs.Length; i++)
            {
                GameObject mob = summonedMobs[i];

                if (mob == null)
                    continue;

                mob.transform.position = Vector3.MoveTowards(
                    mob.transform.position,
                    targetPosition,
                    mobMergeSpeed * Time.deltaTime
                );

                if (Vector2.Distance(mob.transform.position, targetPosition) > 0.05f)
                {
                    allArrived = false;
                }
            }

            if (allArrived)
                break;

            yield return null;
        }
    }

    private void CleanSummonedMobSlots()
    {
        if (summonedMobs == null)
            return;

        for (int i = 0; i < summonedMobs.Length; i++)
        {
            if (summonedMobs[i] == null)
            {
                summonedMobs[i] = null;
            }
        }
    }

    private int GetSummonedMobCount()
    {
        int count = 0;

        for (int i = 0; i < summonedMobs.Length; i++)
        {
            if (summonedMobs[i] != null)
            {
                count++;
            }
        }

        return count;
    }

    private IEnumerator MinePattern()
    {
        if (minePrefab == null || player == null)
            yield break;

        for (int i = 0; i < mineCount; i++)
        {
            float randomX = Random.Range(mineMinForwardX, mineMaxForwardX);

            Vector3 spawnPosition = new Vector3(
                player.position.x + randomX,
                mineY,
                0f
            );

            GameObject mine = Instantiate(
                minePrefab,
                spawnPosition,
                Quaternion.identity
            );

            AddMoveLeft(mine);
        }

        yield return null;
    }

    private IEnumerator SlashPattern()
    {
        if (slashPrefab == null || player == null)
            yield break;

        Vector3 spawnPosition = slashPoint != null
            ? slashPoint.position
            : new Vector3(
                player.position.x + groundRiseStartXOffset,
                groundRiseTargetY,
                0f
            );

        GameObject slash = Instantiate(
            slashPrefab,
            spawnPosition,
            Quaternion.identity
        );

        yield return new WaitForSeconds(slashDuration);

        if (slash != null)
            Destroy(slash);
    }

    private IEnumerator PillarPattern()
    {
        if (pillarPrefab == null || player == null)
            yield break;

        float startX = player.position.x + pillarStartXOffset;

        for (int i = 0; i < pillarCount; i++)
        {
            Vector3 spawnPosition = new Vector3(
                startX + i * pillarIntervalX,
                pillarY,
                0f
            );

            GameObject pillar = Instantiate(
                pillarPrefab,
                spawnPosition,
                Quaternion.identity
            );

            AddMoveLeft(pillar);
        }

        yield return null;
    }

    private IEnumerator EnergyBallPattern()
    {
        if (energyBallPrefab == null)
            yield break;

        bool isLongCharge = Random.value < longChargeChance;

        float selectedChargeDuration = isLongCharge
            ? longChargeDuration
            : shortChargeDuration;

        Vector3 chargePosition = chargeEffectPoint != null
            ? chargeEffectPoint.position
            : transform.position;

        GameObject chargeEffect = null;

        if (chargeEffectPrefab != null)
        {
            chargeEffect = Instantiate(
                chargeEffectPrefab,
                chargePosition,
                Quaternion.identity
            );
        }

        yield return new WaitForSeconds(selectedChargeDuration);

        if (chargeEffect != null)
            Destroy(chargeEffect);

        Vector3 firePosition = energyBallFirePoint != null
            ? energyBallFirePoint.position
            : transform.position;

        Vector3 targetPosition = energyBallTargetPoint != null
            ? energyBallTargetPoint.position
            : firePosition;

        GameObject energyBall = Instantiate(
            energyBallPrefab,
            firePosition,
            Quaternion.identity
        );

        yield return StartCoroutine(MoveObjectToPosition(
            energyBall,
            targetPosition,
            energyBallSpeed
        ));

        yield return new WaitForSeconds(energyBallArriveDelay);

        if (energyBall != null)
            Destroy(energyBall);

        if (isLongCharge)
        {
            yield return StartCoroutine(SpawnExplosionAtPosition(
                outerExplosionPrefab,
                targetPosition
            ));

            yield return StartCoroutine(SpawnExplosionAtPosition(
                centerExplosionPrefab,
                targetPosition
            ));
        }
        else
        {
            yield return StartCoroutine(SpawnExplosionAtPosition(
                centerExplosionPrefab,
                targetPosition
            ));

            yield return StartCoroutine(SpawnExplosionAtPosition(
                outerExplosionPrefab,
                targetPosition
            ));
        }
    }

    private IEnumerator SpawnExplosionAtPosition(
    GameObject explosionPrefab,
    Vector3 spawnPosition
)
    {
        if (explosionPrefab == null)
            yield break;

        GameObject explosion = Instantiate(
            explosionPrefab,
            spawnPosition,
            Quaternion.identity
        );

        yield return new WaitForSeconds(explosionDuration);

        if (explosion != null)
            Destroy(explosion);
    }

    private IEnumerator FallThenMoveLeft(
        GameObject target,
        Vector3 targetPosition,
        float fallSpeed
    )
    {
        yield return StartCoroutine(MoveObjectToPosition(
            target,
            targetPosition,
            fallSpeed
        ));

        AddMoveLeft(target);
    }

    private IEnumerator MoveObjectToPosition(
        GameObject target,
        Vector3 targetPosition,
        float speed
    )
    {
        if (target == null)
            yield break;

        while (target != null &&
               Vector2.Distance(target.transform.position, targetPosition) > 0.05f)
        {
            target.transform.position = Vector3.MoveTowards(
                target.transform.position,
                targetPosition,
                speed * Time.deltaTime
            );

            yield return null;
        }

        if (target != null)
        {
            target.transform.position = targetPosition;
        }
    }

    private void AddMoveLeft(GameObject target)
    {
        if (target == null)
            return;

        WhiteTigerMoveLeft moveLeft =
            target.GetComponent<WhiteTigerMoveLeft>();

        if (moveLeft == null)
        {
            moveLeft = target.AddComponent<WhiteTigerMoveLeft>();
        }

        moveLeft.Init(leftMoveSpeed);
    }

    private class PatternRuntimeData
    {
        public PatternKind kind;
        public float cooldown;
        public float currentCooldown;

        public PatternRuntimeData(PatternKind kind, float cooldown)
        {
            this.kind = kind;
            this.cooldown = cooldown;
            currentCooldown = 0f;
        }
    }
}