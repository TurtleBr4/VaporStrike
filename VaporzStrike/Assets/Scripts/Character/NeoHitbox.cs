using UnityEngine;

public class NeoHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 5;
    [SerializeField] private float knockback = 10f;

    private Collider hitCollider;

    private void Awake()
    {
        hitCollider = GetComponent<Collider>();
        hitCollider.enabled = false;
    }

    public void Activate()
    {
        hitCollider.enabled = true;
    }

    public void Deactivate()
    {
        hitCollider.enabled = false;
    }
}
