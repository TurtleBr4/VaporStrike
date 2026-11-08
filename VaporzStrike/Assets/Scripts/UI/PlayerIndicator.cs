using UnityEngine;
using TMPro;

public class PlayerIndicator : Billboard
{
    public Player player;
    public TMP_Text playerNumber;

    private void Start()
    {
        player = GetComponentInParent<Player>();
        playerNumber.text = "P" +(player.getPlayerID() + 1).ToString();
    }
}
