using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardScript : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(Camera.main.transform.transform.rotation.eulerAngles.x, Camera.main.transform.transform.rotation.eulerAngles.y, Camera.main.transform.transform.rotation.eulerAngles.z);
        //transform.rotation = Camera.main.transform.transform.rotation;
    }
}
