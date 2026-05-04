using UnityEngine;

[CreateAssetMenu(fileName = "HitboxData", menuName = "Scriptable Objects/HitboxData")]
public class HitboxData : ScriptableObject //probably dont use this anymore?
{
    public float radius;
    public float damage;
    public float knockback;
    public int hitStun;
    public Vector2 launchDirection;
}
