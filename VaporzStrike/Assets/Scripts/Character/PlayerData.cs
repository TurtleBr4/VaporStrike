using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{

    public string playerName;
    public GameObject playerPrefab;
    public int playerId;
    public float weight;  //determines knockback, fall speed
    public float moveSpeed;
    public float jumpPower;
    public float airSpeed;
    public float maxFallSpeed;
    public Moveset moveset;
    
}
