using UnityEngine;

public class BlastZone : MonoBehaviour
{
    public GameManager g;

    private void Start()
    {
        g = FindFirstObjectByType<GameManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            doPlayerDeath(other.gameObject);
            other.gameObject.GetComponent<Player>().doBlastZoneDeath();
        }
    }

    public void doPlayerDeath(GameObject g) //rewrite to work with stocks later
    {
        CharacterController cc = g.GetComponent<CharacterController>();
        cc.enabled = false;
        g.transform.position = new Vector3(0, 8f, 4.5f);
        cc.enabled = true;
    }
}
