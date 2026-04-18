using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody rb;
    public int ownerID;
    public float speed;
    public int lifeTime;
    private HitboxDataStruct data;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void initProjectile(HitboxDataStruct H, int id, float s, int lt)
    {
        data = H;
        ownerID = id;
        speed = s;
        lifeTime = lt;
        rb.linearVelocity = transform.forward * speed;
    }
    
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hurtbox"))
        {
            GenericCharacter guy = other.GetComponentInParent<GenericCharacter>();
            if (guy.getPlayerID() != ownerID) //implement multihits later
            {
                print("Hit an Enemy With a Projectile");
                guy.doTakeDamage(data);
                Destroy(gameObject);
            }
        }
    }
}
