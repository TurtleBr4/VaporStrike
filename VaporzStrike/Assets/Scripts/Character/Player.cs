using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Information")]
    [SerializeField] private string Name;
    [SerializeField] private float Percent;
    [SerializeField] private int playerID;
    [Header("References")]
    [SerializeField] PlayerStateController playerState;
    [SerializeField] PlayerMovement playerMove;

    public void takeDamage(int dmg, Vector2 direction)
    {
        Percent += dmg;
    }
}
