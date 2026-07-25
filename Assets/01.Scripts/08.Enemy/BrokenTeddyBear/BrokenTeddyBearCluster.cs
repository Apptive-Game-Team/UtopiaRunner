using System;
using UnityEngine;

namespace _01.Scripts._08.Enemy.BrokenTeddyBear
{
    public class BrokenTeddyBearCluster : MonoBehaviour
    {
        [SerializeField] private GameObject clusterExplode;
        private float _damage;
        
        public void Init(float d)
        {
            _damage = d;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                GameObject ce = Instantiate(clusterExplode, transform.position, Quaternion.identity);
                ce.GetComponent<BrokenTeddyBearClusterExplode>().Init(_damage);
                ce.SetActive(true);
                Destroy(gameObject);
            }
        }
    }
}
