using UnityEngine;
using TMPro;

public class PlayerIndicator : Billboard
{
    public GenericCharacter player;
    public TMP_Text playerNumber;

    private void Start()
    {
        player = GetComponentInParent<GenericCharacter>();
        playerNumber.text = "P" +(player.getPlayerID() + 1).ToString();
    }
}
