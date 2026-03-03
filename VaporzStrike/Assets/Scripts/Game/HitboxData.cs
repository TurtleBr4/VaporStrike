using UnityEngine;

[CreateAssetMenu(fileName = "HitboxData", menuName = "Scriptable Objects/HitboxData")]
public class HitboxData : ScriptableObject
{
    public float radius;
    public float damage;
    public float knockback;
    public Vector2 launchDirection;
}
