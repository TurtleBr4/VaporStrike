using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody rb;
    public float speed;
    private HitboxDataStruct data;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void initProjectile(HitboxDataStruct H)
    {
        data = H;
        rb.linearVelocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hurtbox"))
        {
            other.GetComponent<GenericCharacter>().doTakeDamage(data);
        }
        
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
    }
}
