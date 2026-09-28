using UnityEngine;
using System.Collections;
public class LowFPS : MonoBehaviour
{
    public int choppiness = 3;
    public bool frameOn = true;
    private Canvas canvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(FrameRate());
        canvas = GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        canvas.enabled = frameOn;
    }
    IEnumerator FrameRate()
    {
        while (true)
        {
            if (choppiness > 0)
            {
                frameOn = !frameOn;
            }
            else
            {
                frameOn = true ;
            }
                yield return new WaitForSeconds(choppiness / 100f);
        }
    }
}
