using UnityEngine;

public enum EnemyAttackType
{
    Bullet,
    Laser,
    Explode,
}

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Data/Enemy")]

public class EnemyData : ScriptableObject
{
    [Header("Name")]
    public string enemyName;

    [Header("Stats")]
    public float attackDamage;
    public float healthPoint;

    [Header("Attack Type")]
    public EnemyAttackType attackType;

    [Header("Pattern")]
    public GameObject enemyPatternPrefab;
    public float cooldown;
}
