using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Basic Data")]
    public string playerName;
    public GameObject playerPrefab;
    public int playerId;
    public float weight;  //determines knockback, fall speed
    public float moveSpeed;
    public float jumpPower;
    public float airSpeed;
    public float maxFallSpeed;
    public Sprite mugshot;
    [Header("Grab Data in Frames")]
    public int grabboxStartFrame;
    public int grabboxEndFrame;
    public int grabboxDuration;
    [Header("Jumpsquat and Landing")]
    [SerializeField] public int jumpSquatLength;
    [SerializeField] public int landingLength;



}
