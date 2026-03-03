using System;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public HitboxData data; //i love my scriptable objects
    
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
  