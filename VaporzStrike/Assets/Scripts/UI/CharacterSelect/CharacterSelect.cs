using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour
{
    public LevelData chosenLevel;

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
        //SceneManager.LoadScene("SampleScene");
    }
    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadLevelOne()
    {
        chosenLevel.levelName = "Level1";
        //assign level prefab
        
        SceneManager.LoadScene("Battlefield");
    }
    public void LoadLevelTwo()
    {
        chosenLevel.levelName = "Level2";
        //assign level prefab
        
        SceneManager.LoadScene("Battlefield");
    }
}
