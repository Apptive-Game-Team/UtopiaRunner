using System;
using _01.Scripts._00.Manager;
using UnityEngine;

namespace _01.Scripts._06.Weapon
{
    public class AutoAttackProjectile : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;

        // ���� ���: ���� ����� ���� ã�Ƽ� �� �������� �߻�
        // private GameObject _targetEnemy;
        // private Vector3 _targetDirection;

        // ���� ���: �׳� ���������� ������ �̵�
        private Vector3 _moveDirection = Vector3.right;

        protected Action<GameObject, float> OnHitEffects;
        protected float Damage;
        private Color _textureColor;

        private void Awake()
        {
            Sprite sprite = GetComponent<SpriteRenderer>().sprite;

            Texture2D texture = sprite.texture;
            Rect rect = sprite.textureRect;

            Color[] pixels = texture.GetPixels(
                (int)rect.x,
                (int)rect.y,
                (int)rect.width,
                (int)rect.height);

            Color average = Color.black;

            foreach (Color pixel in pixels)
            {
                average += pixel;
            }

            average /= pixels.Length;
            average.a = 1;

            _textureColor = average * GetComponent<SpriteRenderer>().color;
        }

        public void Init(float damage)
        {
            Damage = damage;
        }

        public void AddEffect(Action<GameObject, float> effect)
        {
            OnHitEffects += effect;
        }

        private void Update()
        {
            // ���� ���: ���� ����� ���� ã�Ƽ� �� �������� �̵�
            /*
            if (_targetEnemy == null)
            {
                _targetEnemy = FindNearestEnemy();

                if (_targetEnemy == null)
                {
                    Destroy(gameObject);
                    return;
                }

                _targetDirection =
                    (_targetEnemy.transform.position - transform.position).normalized;
            }

            transform.Translate(_targetDirection * (speed * Time.deltaTime));
            */

            // ���� ���: ���������� ������ �̵�
            transform.Translate(
                _moveDirection * (speed * Time.deltaTime),
                Space.World
            );
        }

        // ���� ���: ���� ����� �� ã��
        /*
        private GameObject FindNearestEnemy()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            GameObject nearestEnemy = null;
            float nearestDistance = float.MaxValue;

            foreach (GameObject enemy in enemies)
            {
                float distance =
                    Vector2.Distance(transform.position, enemy.transform.position);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = enemy;
                }
            }

            return nearestEnemy;
        }
        */

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy"))
            {
                ApplyHit(collision.gameObject);
                EffectManager.Instance.PlayEffect(EffectType.Hit, collision.ClosestPoint(transform.position), _textureColor);
                Destroy(gameObject);
            }
        }

        protected virtual void ApplyHit(GameObject enemy)
        {
            float damage = CalculateDamage();

            enemy.GetComponent<EnemyHp>()?.TakeDamage(damage);
            enemy.GetComponent<BossHp>()?.TakeDamage(damage);
            
            OnHitEffects?.Invoke(enemy, damage);
        }

        protected virtual float CalculateDamage()
        {
            return Damage;
        }
    }
}