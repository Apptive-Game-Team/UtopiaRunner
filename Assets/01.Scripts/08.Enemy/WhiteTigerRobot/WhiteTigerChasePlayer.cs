using UnityEngine;

public class WhiteTigerChasePlayer : MonoBehaviour
{
    [SerializeField] private float destroyDistance = 0.1f;

    private Transform target;
    private float moveSpeed;
    private bool isInitialized;

    public void Init(Transform target, float moveSpeed)
    {
        this.target = target;
        this.moveSpeed = moveSpeed;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized || target == null)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}