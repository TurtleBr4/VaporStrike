using UnityEngine;

public class Grabbox : MonoBehaviour
{
    public GrabboxDataStruct data;
    public SphereCollider c;
    public int lifetime = 100;
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
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            transform.parent.GetComponent<GenericCharacter>().isGrabbing = true;
            other.SendMessage("GetGrabbed", SendMessageOptions.RequireReceiver);

        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
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
            }
                
        }

    }

    private void Update()
    {

    }
}
