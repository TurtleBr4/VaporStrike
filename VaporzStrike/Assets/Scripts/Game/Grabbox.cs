using UnityEngine;

public class Grabbox : MonoBehaviour
{
    public GrabboxDataStruct data;
    public SphereCollider c;
    public int lifetime = 100;
    public int pummelDMG = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        c = GetComponent<SphereCollider>();
    }

    public void takeInGrabboxData(GrabboxDataStruct d)
    {
        data = d;
        c.radius = data.radius;
    }

    public void Pummel(int amount)
    {
        pummelDMG = amount;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && other.gameObject != transform.parent.gameObject)
        {
            transform.parent.GetComponent<GenericCharacter>().isGrabbing = true;
            other.SendMessage("GetGrabbed", SendMessageOptions.RequireReceiver);

        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && other.gameObject != transform.parent.gameObject)
        {
            if (lifetime <= 0)
            {
                gameObject.SetActive(false);
                lifetime = 100; 
                transform.parent.GetComponent<GenericCharacter>().isGrabbing = false;
                other.SendMessage("GrabRelease", SendMessageOptions.RequireReceiver);
            }
            else
            {
                lifetime--;

                if(pummelDMG > 0)
                {
                    pummelDMG = 0;
                    other.SendMessage("DoTakePercent", pummelDMG);
                }
            }
                
        }

    }

    private void Update()
    {

    }
}
