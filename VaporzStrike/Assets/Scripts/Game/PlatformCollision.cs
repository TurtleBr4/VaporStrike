using UnityEngine;
using System.Collections;
public class PlatformCollision : MonoBehaviour
{
    private Collider pc;
    [SerializeField]private float colliderOffTime = 60; //time, in frames

    void Awake()
    {
        pc = GetComponent<Collider>();
        colliderOffTime = (colliderOffTime / 16.6f); 
    }

    public void handlePlayerFromBottom(Collider player)
    {
        //print("Player Detected Under Platform");
        
        Physics.IgnoreCollision(
            player,
            pc,
            true
        );
        
        StartCoroutine(platformWait(player));
   
    }

    public void handlePlayerFromTop(Collider player)
    {
        //print("Player Above Under Platform");
        
        Physics.IgnoreCollision(
            player,
            pc,
            false
        );
    }

    IEnumerator platformWait(Collider player)
    {
        yield return new WaitForSeconds(colliderOffTime);
        Physics.IgnoreCollision(player, pc, false);
    }

    void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        foreach (ContactPoint contact in collision.contacts)
        {
            //ignore collision
            if (contact.normal.y < 0.5f)
            {
                Physics.IgnoreCollision(
                    collision.collider,
                    pc,
                    true
                );
                return;
            }
        }

        //enable collision
        Physics.IgnoreCollision(
            collision.collider,
            pc,
            false
        );
    }
}
