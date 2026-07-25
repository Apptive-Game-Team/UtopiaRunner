using System.Collections;
using System.Collections.Generic;
using _01.Scripts._08.Enemy.BrokenTeddyBear;
using DG.Tweening;
using UnityEngine;

namespace _01.Scripts._08.Enemy
{
    public class BrokenTeddyBearController : MonoBehaviour
    {
        private enum PatternKind
        {
            HipDrop,
            SummonRobots
        }

        [Header("Components")]
        [SerializeField] private BossData data;
        private EnemySlotManager _enemySlotManager;

        [Header("Pattern - Button Throw")]
        [SerializeField] private BrokenTeddyBearButton[] buttonPrefabs;

        [Header("Pattern - Hip Drop")] 
        [SerializeField] private Vector3 spawnPosition;
        [SerializeField] private float jumpHeight = 5f;
        [SerializeField] private float jumpDuration = 0.6f;
        [SerializeField] private float landDuration = 0.2f;

        [Header("Pattern - Summon & Combine Robots")]
        [SerializeField] private GameObject[] minionRobotPrefabs;
        [SerializeField] private GameObject clusterPrefab;
        [SerializeField] private Vector3 clusterSpawnPos;
        [SerializeField] private float combinedSpawnMinY = -2f;
        [SerializeField] private float combinedSpawnMaxY = 2f;

        [Header("Pattern Option")]
        [SerializeField] private float delayBetweenPatterns = 1.5f;
        
        private List<GameObject> _activeMinions = new();
        private List<PatternRuntimeData> _runtimePatterns = new();
        
        private bool _isPatternRunning;
        private Vector3 _originalPosition;
        
        private BossPatternData _buttonPattern;
        private BossPatternData _hipDropPattern;
        private BossPatternData _summonPattern;

        private void Awake()
        {
            _originalPosition = transform.position;
            _enemySlotManager = FindFirstObjectByType<EnemySlotManager>();
            InitPatterns();
        }

        private void Start()
        {
            StartCoroutine(BasicButtonAttackRoutine());
            StartCoroutine(PatternLoop());
        }

        private void Update()
        {
            UpdateCooldowns();
        }

        private void InitPatterns()
        {
            _buttonPattern = data.patterns.Find(p => p.patternName == "Button");
            _hipDropPattern = data.patterns.Find(p => p.patternName == "HipDrop");
            _summonPattern = data.patterns.Find(p => p.patternName == "SummonRobots");
            
            _runtimePatterns.Clear();

            _runtimePatterns.Add(new PatternRuntimeData(PatternKind.HipDrop, _hipDropPattern.cooldown));
            _runtimePatterns.Add(new PatternRuntimeData(PatternKind.SummonRobots, _summonPattern.cooldown));
        }

        private void UpdateCooldowns()
        {
            foreach (var pattern in _runtimePatterns)
            {
                if (pattern.currentCooldown > 0f)
                {
                    pattern.currentCooldown -= Time.deltaTime;
                }
            }
        }
        
        private IEnumerator PatternLoop()
        {
            while (true)
            {
                if (_isPatternRunning)
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

                PatternRuntimeData selectedPattern = readyPatterns[Random.Range(0, readyPatterns.Count)];
                yield return StartCoroutine(UsePattern(selectedPattern));

                yield return new WaitForSeconds(delayBetweenPatterns);
            }
        }

        private List<PatternRuntimeData> GetReadyPatterns()
        {
            List<PatternRuntimeData> readyPatterns = new List<PatternRuntimeData>();

            foreach (var pattern in _runtimePatterns)
            {
                if (pattern.currentCooldown <= 0f)
                {
                    readyPatterns.Add(pattern);
                }
            }

            return readyPatterns;
        }

        private IEnumerator UsePattern(PatternRuntimeData pattern)
        {
            _isPatternRunning = true;

            switch (pattern.kind)
            {
                case PatternKind.HipDrop:
                    yield return StartCoroutine(HipDropPatternRoutine());
                    break;

                case PatternKind.SummonRobots:
                    yield return StartCoroutine(SummonOrCombineRoutine());
                    break;
            }

            pattern.currentCooldown = pattern.cooldown;
            yield return null;
            _isPatternRunning = false;
        }

        private IEnumerator BasicButtonAttackRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(_buttonPattern.cooldown);
                
                if (buttonPrefabs == null || buttonPrefabs.Length == 0)
                {
                    continue;
                }

                if (_isPatternRunning)
                {
                    continue;
                }
                
                BrokenTeddyBearButton selectedButton = buttonPrefabs[Random.Range(0, buttonPrefabs.Length)];
                selectedButton.InitSetting(_buttonPattern.attackDamage);
                selectedButton.gameObject.SetActive(true);
                
                Vector3 targetPoint = selectedButton.targetPoint;
                
                GameObject buttonObj = Instantiate(selectedButton.gameObject, transform.position, Quaternion.identity);
                buttonObj.transform.DOJump(targetPoint, Vector3.Distance(transform.position, targetPoint) / 4, 1, 2f)
                    .SetEase(Ease.InSine)
                    .OnComplete(() =>
                    {
                        if (buttonObj)
                        {
                            buttonObj.GetComponent<BrokenTeddyBearButton>().Explode();
                        }
                    });
            }
        }
        
        private IEnumerator HipDropPatternRoutine()
        {
            _isPatternRunning = true;
            
            Vector3 peakPos = _originalPosition + Vector3.up * jumpHeight;

            Sequence seq = DOTween.Sequence();

            seq.Append(transform.DOMove(peakPos, jumpDuration).SetEase(Ease.OutCubic));
            seq.Append(transform.DOMove(_originalPosition, landDuration).SetEase(Ease.InCubic));

            yield return seq.WaitForCompletion();

            transform.position = _originalPosition;
            
            GameObject go = Instantiate(_hipDropPattern.patternPrefab, spawnPosition, Quaternion.identity);
            var sw = go.GetComponent<BrokenTeddyBearShockWave>();
            sw.Init(_hipDropPattern.attackDamage);
            sw.gameObject.SetActive(true);

            yield return new WaitForSeconds(0.5f);

            _isPatternRunning = false;
        }
        
        private IEnumerator SummonOrCombineRoutine()
        {
            _isPatternRunning = true;
            
            Sequence seq = DOTween.Sequence();
            
            if (_activeMinions.Count >= 4)
            {
                GameObject clusterObj = Instantiate(clusterPrefab, clusterSpawnPos, Quaternion.identity);
                clusterObj.GetComponent<BrokenTeddyBearCluster>().Init(_summonPattern.attackDamage);
                
                for (int i = _activeMinions.Count - 1; i >= 0; i--)
                {
                    Vector2 randomOffset = Random.insideUnitCircle * 0.3f;
                    Vector3 targetPos = clusterObj.transform.position + (Vector3)randomOffset;
                    float randomRotation = Random.Range(-720f, 720f);
                    
                    _activeMinions[i].GetComponent<Collider2D>().enabled = false;
                    _activeMinions[i].GetComponent<EnemyController>().enabled = false;
                    _enemySlotManager.UnregisterEnemy(_activeMinions[i].GetComponent<SlotEnemy>());
                    _activeMinions[i].transform.SetParent(clusterObj.transform);
                    seq.Join(_activeMinions[i].transform.DOMove(targetPos, 0.5f)
                        .SetEase(Ease.OutSine));
                    seq.Join(_activeMinions[i].transform
                        .DORotate(new Vector3(0, 0, randomRotation), 0.5f, RotateMode.FastBeyond360)
                        .SetEase(Ease.InQuad));
                }
                
                yield return seq.WaitForCompletion();
                _activeMinions.Clear();
                
                float randomY = Random.Range(combinedSpawnMinY, combinedSpawnMaxY);
                Vector3 dir = (new Vector3(-6f, randomY, 0f) - clusterObj.transform.position);
                Vector3 endPos = clusterObj.transform.position + dir * 3f;
                clusterObj.transform.DOMove(endPos, 2f)
                    .SetEase(Ease.OutSine)
                    .OnComplete(() =>
                    {
                        if (clusterObj)
                        {
                            Destroy(clusterObj);
                        }
                    });
            }
            else
            {
                EnemyLane[] lanes =
                {
                    EnemyLane.Bottom,
                    EnemyLane.Middle
                };

                for (int i = 0; i < minionRobotPrefabs.Length && i < lanes.Length; i++)
                {
                    GameObject minion = _enemySlotManager.SpawnEnemy(minionRobotPrefabs[i], lanes[i], true);

                    if (minion != null)
                    {
                        _activeMinions.Add(minion);
                    }
                }
            }

            yield return new WaitForSeconds(0.5f);

            _isPatternRunning = false;
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
}