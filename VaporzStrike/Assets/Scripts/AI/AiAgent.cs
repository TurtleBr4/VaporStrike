using UnityEngine;

public class AiAgent : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Player agent;
    [SerializeField] private Player targetPlayer;
    
    
    [Header("Settings")]
    [SerializeField] private int aiLevel = 1;
    
    [Header("Data")]
    [SerializeField] private MoveDefinition[] closeRangeMoves;
    [SerializeField] private MoveDefinition[] mediumRangeMoves;
    [SerializeField] private MoveDefinition[] longRangeMoves;
}
