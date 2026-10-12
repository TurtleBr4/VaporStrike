using System;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    /*
    public HitboxDataStruct data;
    public GenericCharacter self;
    public float damage;
    public float knockback;
    public int hitStun;
    public int blockStun;
    public Vector2 launchDirection;
    public SphereCollider c;

    private void Start()
    {
        c = GetComponent<SphereCollider>();
        data = new HitboxDataStruct
        {
            hitStun = hitStun,
            damage = damage,
            knockback = knockback,
            launchDirection = launchDirection,
            blockStun = blockStun
        };
    }

    private void Update()
    {
        if (self.getFaceDir() != faceDirection.right)
        {
            data.launchDirection = -launchDirection;
        }
        else
        {
            data.launchDirection = launchDirection;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hurtbox"))
        {
            GenericCharacter guy = other.GetComponentInParent<GenericCharacter>();
            if (!guy.gameObject.Equals(self.gameObject))
            {
                print("Hit an Enemy");
                guy.doTakeDamage(data);
            }
        }
        else
        {
            Debug.LogWarning("Not A Hitbox!");
        }
    }
    */
}

public struct HitboxDataStruct
{
    public float damage;
    public float knockback;
    public int hitStun;
    public int blockStun;
    public Vector2 launchDirection;
}


  