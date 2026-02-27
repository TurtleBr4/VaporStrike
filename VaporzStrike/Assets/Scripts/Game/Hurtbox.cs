using System;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    private GenericCharacter player;

    private void Start()
    {
        player = GetComponentInParent<GenericCharacter>();
    }

    public void passDamageToPlayer(float damage)
    {
        player.doTakeDamage();
    }
}
