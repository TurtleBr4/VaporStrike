using UnityEngine;

public class ThemeScript : MonoBehaviour
{
    public static ThemeScript instance { get; private set; }
    void Start()
    {

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            Debug.Log("Destroying");
        }
        else
        {
            Debug.Log("Creating Instance");
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
