using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public float spawnTime;
    public GameObject item;
    public Item[] lootPool;
    public Transform[] spawnLoc;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnTime = Random.Range(20, 40);
    }

    // Update is called once per frame
    void Update()
    {
        if(spawnTime <= 0)
        {
            spawnTime = Random.Range(10, 40);
            SpawnItem();
        }
        spawnTime -= Time.deltaTime;
    }

    void SpawnItem()
    {
        int locChoose = Random.Range(0, spawnLoc.Length);

        GameObject newItem = item;

        int locChoose2 = Random.Range(0, lootPool.Length);
        newItem.GetComponent<Loot>().currLoot = lootPool[locChoose2];

        Instantiate(newItem, spawnLoc[locChoose].position, Quaternion.identity);
    }
}
