using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using Unity.VisualScripting;
public class TitleScreen : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        if(ThemeScript.instance)
            ThemeScript.instance.mode = -1;

        InputSystem.onAnyButtonPress.CallOnce(control =>
        {
            if (control.device is Gamepad)
            {
                Play();
            }
        });
    }
    public void Play()
    {
        SceneManager.LoadScene("MainMenu");
        ThemeScript.instance.mode = 0;
        ThemeScript.instance.Restart();

    }
}
