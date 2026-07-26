using _01.Scripts._00.Manager;
using _01.Scripts._05.Utility;
using UnityEngine;

namespace _01.Scripts._06.Weapon.GravityCore
{
    public class GravityCoreSkillAttack : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float pullRadius = 100f;
        [Range(0.01f, 1f)]
        [SerializeField] private float pullSensitivity = 0.1f;
        [SerializeField] private float destroyDistance = 0.2f;

        [Header("Optimization")]
        [SerializeField] private int maxTargetCount = 50;
        
        private Collider2D[] _results;
        private ContactFilter2D _filter;

        private void Awake()
        {
            _results = new Collider2D[maxTargetCount];
            _filter.useTriggers = true;
            Destroy(gameObject, ValueFormula.GetWeaponSkillValues(InGameManager.Instance.weapon.weaponInfo,
                GameManager.Instance.playerData.weaponGrade[InGameManager.Instance.weapon.weaponInfo.id])[0]);
        }

        private void Update()
        {
            PullEnemies();
        }

        private void PullEnemies()
        {
            int count = Physics2D.OverlapCircle(transform.position, pullRadius, _filter, _results);

            for (int i = 0; i < count; i++)
            {
                Collider2D col = _results[i];

                if (col && col.CompareTag("Enemy") && !col.GetComponent<BossHp>())
                {
                    col.transform.position = Vector2.Lerp(
                        col.transform.position, 
                        transform.position, 
                        pullSensitivity * Time.deltaTime * 50f
                    );

                    float distance = Vector2.Distance(transform.position, col.transform.position);
                    if (distance <= destroyDistance)
                    {
                        if (col.TryGetComponent(out EnemyHp enemyHp))
                        {
                            enemyHp.TakeDamage(9999f);
                        }
                        else
                        {
                            Destroy(col.gameObject);
                        }
                    }
                }
            }
        }
    }
}