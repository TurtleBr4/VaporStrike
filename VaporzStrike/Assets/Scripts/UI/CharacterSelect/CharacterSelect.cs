using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Back();
        }
    }
    public void MatchStart()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
