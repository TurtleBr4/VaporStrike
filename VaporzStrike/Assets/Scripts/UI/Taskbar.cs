using UnityEngine;
using TMPro;
using System;
public class Taskbar : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeText.text = DateTime.Now.ToString("hh:mm tt");
    }
}
