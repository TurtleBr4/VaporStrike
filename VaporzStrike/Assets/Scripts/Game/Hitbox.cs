using System;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float damage;
    bool damageSet = false;

    public void setDamage(float d)
    {
        damage = d;
        damageSet = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (damageSet && other.CompareTag("Hurtbox"))
        {
            other.gameObject.SendMessage("passDamageToPlayer", damage);
        }
        else
        {
            Debug.LogWarning("Hitbox damage hasn't been set!");
        }
    }
}
  