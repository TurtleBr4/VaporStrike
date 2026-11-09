using System;
using System.Collections.Generic;
using UnityEngine;

public class GrabBox : MonoBehaviour
{
    private List<Player> hits = new List<Player>();
    [SerializeField] private Player player;
    public Vector2 launchDirection;
    public float launchForce;

    private void OnEnable()
    {
        GetComponent<BoxCollider>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hurtbox"))
        {
            NeoHurtbox hurt = other.gameObject.GetComponent<NeoHurtbox>();
            if (hits.Contains(hurt.getPlayer()))
            {
                return; //we already interacted with this player
            }
            else
            {
                hits.Add(hurt.getPlayer());
                hurt.getPlayer().getGrabbed(transform);
                player.doAGrab();
            }

        }
    }
    
    private void OnDisable()
    {
        hits.Clear();
    }
}
