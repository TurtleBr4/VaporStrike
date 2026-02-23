using UnityEngine;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour
{
    public bool isPaused = false;
    public GameObject pauseCanvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pauseCanvas.SetActive(isPaused);
        Time.timeScale = (isPaused ? 0.0f : 1.0f);

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            isPaused = !isPaused;
        }

        if(isPaused && Input.GetKeyUp(KeyCode.LeftShift))
        {
            ToCharacterSelect();
        }

    }

    public void ToCharacterSelect()
    {
        SceneManager.LoadScene("CharacterSelect");
    }
}
