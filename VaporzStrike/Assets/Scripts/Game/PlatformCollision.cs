using UnityEngine;

public class PlatformCollision : MonoBehaviour
{
    private Collider pc;

    void Awake()
    {
        pc = GetComponent<Collider>();
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
