using UnityEngine;

public class WhiteTigerMoveToTarget : MonoBehaviour
{
    [SerializeField] private float destroyDistance = 0.1f;

    private Vector3 targetPosition;
    private float moveSpeed;
    private bool isInitialized;

    public void Init(Vector3 targetPosition, float moveSpeed)
    {
        this.targetPosition = targetPosition;
        this.moveSpeed = moveSpeed;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, targetPosition) <= destroyDistance)
        {
            Destroy(gameObject);
        }
    }
}