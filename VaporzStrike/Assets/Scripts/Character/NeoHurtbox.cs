using UnityEngine;

public class NeoHurtbox : MonoBehaviour
{
    [SerializeField] private Player p;
    public Player getPlayer()
    {
        return p;
    }
}
