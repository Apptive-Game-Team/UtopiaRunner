using UnityEngine;

namespace _01.Scripts._08.Enemy
{
    public class BrokenTeddyBearButton : MonoBehaviour
    {
        private enum ButtonType
        {
            Head,
            Leg,
        }

        [SerializeField] private ButtonType buttonType;
        [SerializeField] private Vector3 headPoint;
        [SerializeField] private Vector3 legPoint;
        [SerializeField] private GameObject explodeObject;

        public Vector3 targetPoint;
        public float damage;

        private BrokenTeddyBearButtonExplode _explode;

        public void InitSetting(float d)
        {
            targetPoint = buttonType == ButtonType.Head ? headPoint : legPoint;
            damage = d;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Explode();
            }
        }

        public void Explode()
        {
            GameObject go = Instantiate(explodeObject, transform.position, Quaternion.identity);
            _explode = go.GetComponent<BrokenTeddyBearButtonExplode>();
            _explode.Init(damage);
            _explode.gameObject.SetActive(true);
            Destroy(gameObject);
        }
    }
}
