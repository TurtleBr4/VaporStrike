using UnityEngine;

public enum playerState
{
    actionable, attacking, blocking, falling, hitstun, blockstun
}

public class GenericCharacter : MonoBehaviour
{
    [Header("Static Character Variables")]
    [SerilizeField] private string playerName;
    [SerilizeField] private float weight;  //determines knockback, fall speed
    [SerilizeField] private float moveSpeed;
    [SerilizeField] private float airSpeed;
    [SerilizeField]

    [Header("Runtime Character Variables")]
    [SerilizeField] private float damagePercent = 0f;
    [SerilizeField] private playerState currentState;




}
