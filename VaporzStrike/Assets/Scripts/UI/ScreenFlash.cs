using UnityEngine;

public class ScreenFlash : MonoBehaviour
{
    private CanvasGroup group;
    public float flashSpeed = 0.1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       group = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        group.alpha -= flashSpeed;
    }

    public void Flash()
    {
        group.alpha = 1f;
    }
    public void Flash(float amount)
    {
        group.alpha = amount;
    }
}
