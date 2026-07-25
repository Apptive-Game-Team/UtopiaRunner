using System;
using _01.Scripts._07.Character;
using UnityEngine;

namespace _01.Scripts._08.Enemy.BrokenTeddyBear
{
    public class BrokenTeddyBearClusterExplode : MonoBehaviour
    {
        private float _damage;

        private void Start()
        {
            Destroy(gameObject, 0.5f);
        }

        public void Init(float d)
        {
            _damage = d;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerController>().TakeDamage(_damage);
            }
        }
    }
}
