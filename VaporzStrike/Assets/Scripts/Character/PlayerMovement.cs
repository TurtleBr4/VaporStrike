using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private NeoInputReader inputReader;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector2 stick = inputReader.GetLeftStick();

        float horizontal = stick.x;

        Vector3 movement = Vector3.right * (horizontal * moveSpeed);

        transform.position += movement * Time.deltaTime;
    }
}