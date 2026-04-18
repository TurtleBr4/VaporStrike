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
    public int hitboxStartTime;
    public int hitboxEndTime;
    public string animationTrigger;   // optional Animator trigger
    

    [Header("Hitbox(s) Data")] //define data here, gets used to make a hitbox later no need for yet another scriptable object
    public float radius;
    public float damage;
    public int hitStun;
    public float knockback;
    public Vector2 launchDirection;
    public HitboxLocations hitboxLocation;
    public bool isMultiHit = false;
    public int hits = 1; //if its a multi hit, raise this
    public HitboxDataStruct hds;

    [Header("Other Move Data")]
    public bool requiresGrounded = true; // only perform on ground
    public bool hasProjectile;
    
    [Header("Projectile Data")]
    public GameObject projectile;
    public float  projectileSpeed;
    public int projectileLifetime;

    private void OnEnable()
    {
        hds = new HitboxDataStruct();
        hds.damage = damage;
        hds.knockback = knockback;
        hds.launchDirection = launchDirection;
        hds.hitStun = hitStun;
        hds.radius = radius;
    }
}

public enum HitboxLocations
{
    LeftHand, RightHand, LeftFoot, RightFoot, LeftLeg, RightLeg, Torso, Head, LeftShoulder, RightShoulder
}

public struct HitboxDataStruct
{
    public float radius;
    public float damage;
    public float knockback;
    public int hitStun;
    public Vector2 launchDirection;
}
