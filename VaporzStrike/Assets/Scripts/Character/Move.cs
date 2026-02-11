using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Scriptable Objects/Move")]
public class Move : ScriptableObject
{
    public string moveName;           // e.g., "Side Special"
    public InputType[] inputSequence;    // e.g., { "right", "special" }
    public float damage;
    public float hitStun;
    public float knockback;
    public string animationTrigger;   // optional Animator trigger

    [Header("Other Move Data")]
    public bool requiresGrounded = true; // only perform on ground
}
