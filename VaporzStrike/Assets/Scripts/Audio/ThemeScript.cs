using UnityEngine;

public class ThemeScript : MonoBehaviour
{
    public static ThemeScript instance { get; private set; }
    public AudioSource layer1;
    public AudioSource layer2;
    public AudioSource layer3;
    public int mode = 0;
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
    public void Restart()
    {
        /*
        layer1.volume = 0f;
        layer1.time = 0f;
        layer2.volume = 0f;
        layer2.time = 0f;
        layer3.volume = 0f;
        layer3.time = 0f;*/
    }
    public void StopAll()
    {
        layer1.Stop();
        layer2.Stop();
        layer3.Stop();
    }
    public void PlayAll()
    {
        if(!layer1.isPlaying)
            layer1.Play();
        if (!layer2.isPlaying)
            layer2.Play();
        if (!layer3.isPlaying)
            layer3.Play();
    }
    // Update is called once per frame
    void Update()
    {
        switch (mode) {
            case -1:/*
                layer1.volume = 0f;
                layer1.time = 0f;
                layer2.volume = 0f;
                layer2.time = 0f;
                layer3.volume = 0f;
                layer3.time = 0f;*/
                break;
            case 0:
                layer1.volume = 1f;
                layer2.volume = 0f;
                layer3.volume = 0f;
                break;
            case 1:
                layer1.volume = 0f;
                layer2.volume = 1f;
                layer3.volume = 0f;
                break;
            case 2:
                layer1.volume = 0f;
                layer2.volume = 0f;
                layer3.volume = 1f;
                break;

        }
    }
}
