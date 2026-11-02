using System;
using UnityEngine;

[System.Serializable]
public class Loot : MonoBehaviour

{
    [SerializeField] public bool isCollected;
    [SerializeField] public Item currLoot;

    [SerializeField] public int xSpawn = 1;
    [SerializeField] public int ySpawn = 1;
    [SerializeField] public int lootID;
    private SpriteRenderer spriteRenderer;
    public float lifetime;

    [SerializeField] bool randomize = true;

    public Loot()
    {
        isCollected = false;
        xSpawn = 1;
        ySpawn = 1;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Spawn();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (randomize)
        {
            foreach (Item i in Resources.FindObjectsOfTypeAll<Item>())
            {
                int rando = UnityEngine.Random.Range(0, 9);
                if (rando == 1)
                {
                    currLoot = i;
                    break;
                }
            }
            if (currLoot == null) this.gameObject.SetActive(false);
        }
        lifetime = currLoot.lifetime;
    }
    public string ToJson()
    {
        bool prettyPrintJson = true;
        return JsonUtility.ToJson(this, prettyPrintJson);
    }
    // Update is called once per frame
    void Update()
    {
        if (currLoot != null)
        {
            if (currLoot.sprite != null)
            {
                spriteRenderer.sprite = currLoot.sprite;
            }
            currLoot.gObject = this.gameObject;

            if (isCollected)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }
        lifetime -= Time.deltaTime;

        if(lifetime < 0)
        {
            Destroy(gameObject);
        }
    }

    public void Spawn()
    {
        transform.position = new Vector3(xSpawn * 0.3f, 1 * 0.3f, (0f - ySpawn) * 0.3f);
        transform.rotation = Quaternion.identity;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Destroy(this.gameObject);
            other.GetComponent<Player>().takeDamage(currLoot.itemQuality*1, Vector2.zero, 0);

        }
    }
}
