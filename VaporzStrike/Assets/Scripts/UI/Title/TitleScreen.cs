using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
public class TitleScreen : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Update()
    {
        if (Gamepad.current.startButton.wasReleasedThisFrame)
        {
            Play();
        }
    }
    public void Play()
    {
        SceneManager.LoadScene("MainMenu");

        
    }
}
