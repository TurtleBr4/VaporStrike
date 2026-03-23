using System;
using System.Drawing;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Scriptable Objects/Move")]
public class Move : ScriptableObject
{
    public string moveName;           // e.g., "Side Special"
    public InputType[] inputSequence;    // e.g., { "right", "special" }
    public int moveTime;
    public int blockStun;
    public int animationId;
    public string animationTrigger;   // optional Animator trigger

    [Header("Hitbox(s) Data")] //define data here, gets used to make a hitbox later no need for yet another scriptable object
    public float radius;
    public float damage;
    public int hitStun;
    public float knockback;
    public Vector2 launchDirection;

    public HitboxDataStruct hds;

    [Header("Other Move Data")]
    public bool requiresGrounded = true; // only perform on ground

    private void OnEnable()
    {
        hds = new HitboxDataStruct();
        hds.damage = damage;
        hds.knockback = knockback;
        hds.hitStun = hitStun;
        hds.radius = radius;
    }
}
