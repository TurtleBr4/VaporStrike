using System;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    private GenericCharacter player;

    private void Start()
    {
        player = GetComponentInParent<GenericCharacter>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hitbox"))
        {
            player.doTakeDamage();
            
        }
    }
}
