using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "CardEngine/Objects/Item")]
[Serializable]
public class Item : ScriptableObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] public Sprite sprite;
    public GameObject gObject;
    [SerializeField] public string itemName = "???";
    [SerializeField] public string itemHandle = "vanilla.itemHandle";
    [SerializeField] public ItemType type;
    [HideInInspector] public int itemAtk;
    [HideInInspector] public UpgradeType upgradeType;
    [HideInInspector] public UpgradeType affectedStat;
    [HideInInspector] public int itemQuality;
    [HideInInspector] public GameObject weaponBullet;
    [HideInInspector] public Transform bulletSpawn;
    [HideInInspector] public int reloadTime;
    [HideInInspector] public float bulletSpeed = 10f;
    public int lifetime;

    private void OnEnable()
    {

    }


}
public enum ItemType {Default, Consumable}
public enum UpgradeType { Health, Defense }
#if UNITY_EDITOR


[CustomEditor(typeof(Item))]
public class MyItemEditor : Editor
{

     int itemAtk = 1;
    UpgradeType upgradeType = UpgradeType.Defense;
    UpgradeType affectedStat = UpgradeType.Defense;
     int itemQuality = 1;
    GameObject weaponBullet = null;
    int reloadTime = 0;

    public override void OnInspectorGUI()
    {
        Item thisMoveset = (Item)target;
        DrawDefaultInspector();

        affectedStat = thisMoveset.affectedStat;
        upgradeType = thisMoveset.upgradeType;
        itemAtk = thisMoveset.itemAtk;
        weaponBullet = thisMoveset.weaponBullet;

        itemQuality = thisMoveset.itemQuality;

        if (thisMoveset.type == ItemType.Consumable)
        {
            affectedStat = (UpgradeType)EditorGUILayout.EnumPopup("Affected Stat", affectedStat);
            thisMoveset.affectedStat = affectedStat;

            itemQuality = EditorGUILayout.IntField("Item Quality", itemQuality);
            thisMoveset.itemQuality = itemQuality;
        }
    }
}
#endif