using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void MatchStart()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
