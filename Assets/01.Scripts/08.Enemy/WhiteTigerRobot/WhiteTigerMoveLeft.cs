using UnityEngine;

public class WhiteTigerMoveLeft : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float destroyX = -15f;

    private bool isMoving;

    public void Init(float speed)
    {
        moveSpeed = speed;
        isMoving = true;
    }

    private void Update()
    {
        if (!isMoving)
            return;

        transform.position += Vector3.left * (moveSpeed * Time.deltaTime);

        if (transform.position.x <= destroyX)
        {
            Destroy(gameObject);
        }
    }
}