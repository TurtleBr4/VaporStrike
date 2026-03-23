using System;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public HitboxDataStruct data;
    public SphereCollider c;

    private void Start()
    {
        c = GetComponent<SphereCollider>();
    }

    public void takeInHitboxData(HitboxDataStruct d)
    {
        data = d;
        c.radius = data.radius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hurtbox"))
        {
            other.gameObject.SendMessage("passDamageToPlayer", data, SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.LogWarning("Not A Hitbox!");
        }
    }
}

public struct HitboxDataStruct
{
    public float radius;
    public float damage;
    public float knockback;
    public int hitStun;
    public Vector2 launchDirection;
}
  