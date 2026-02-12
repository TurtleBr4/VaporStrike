using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PlayerDatabase", menuName = "Scriptable Objects/PlayerDatabase")]
public class PlayerDatabase : ScriptableObject
{
    [FormerlySerializedAs("items")] public List<PlayerData> players = new List<PlayerData>(); //list of every item in the game

    private Dictionary<int, PlayerData> itemLookup;

    void OnEnable() //go through every item on our list and put in in our dictionary for effecient lookups
    {
        itemLookup = new Dictionary<int, PlayerData>();
        foreach (var pd in players)
        {
            itemLookup[pd.playerId] = pd;
        }
    }

    public PlayerData getPlayerById(int id) //pass in an id, return item data
    {
        return itemLookup.GetValueOrDefault(id);
    }
}
