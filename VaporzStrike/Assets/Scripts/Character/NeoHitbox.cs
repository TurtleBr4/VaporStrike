using System.Collections.Generic;
using UnityEngine;

public class NeoHitbox : MonoBehaviour
{
    [SerializeField] private float damage = 5;
    [SerializeField] private float knockback = 10f;
    [SerializeField] private Vector2 direction;
    [SerializeField] private Player player; //to grab needed info for the move performed like hitstun

    private List<Player> hits = new List<Player>();

    private Collider hitCollider;

    private void Awake()
    {
        hitCollider = GetComponent<Collider>();
        hitCollider.enabled = false;
    }

    private void OnEnable()
    {
        GetComponent<BoxCollider>().enabled = true;
    }
    /*

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Hurtbox"))
        {
            print("faech");
            NeoHurtbox hurt = collision.gameObject.GetComponent<NeoHurtbox>();
            if (hits.Contains(hurt.getPlayer()) || hurt.getPlayer() == player)
            {
                return; //we already interacted with this player
            }
            else
            {
                hits.Add(hurt.getPlayer());
                hurt.getPlayer().takeDamage(damage, new Vector2(direction.x * knockback, direction.y * knockback), player.getCurrentMove().hitStun); //hold
            }

        }
    }
    */

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Hurtbox"))
        {
            NeoHurtbox hurt = other.gameObject.GetComponent<NeoHurtbox>();
            if (hits.Contains(hurt.getPlayer()) || hurt.getPlayer() == player)
            {
                return; //we already interacted with this player
            }
            else
            {
                hits.Add(hurt.getPlayer());
                hurt.getPlayer().takeDamage(damage, new Vector2(direction.x * knockback, direction.y * knockback), player.getCurrentMove().hitStun); //hold
            }

        }
    }

    private void OnDisable()
    {
        hits.Clear();
    }
}
