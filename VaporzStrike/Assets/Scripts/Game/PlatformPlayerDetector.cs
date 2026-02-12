using System;
using UnityEngine;

public class PlatformBottomDetector : MonoBehaviour
{
    private PlatformCollision p;
    public bool bottom = true; //true means bottom, false means top

    private void Awake()
    {
        p = GetComponentInParent<PlatformCollision>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (bottom)
        {
            p.handlePlayerFromBottom(other);
        }
        else
        {
            p.handlePlayerFromTop(other);
        }
        
    }
}
