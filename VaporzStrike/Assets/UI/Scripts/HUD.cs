using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class HUD : MonoBehaviour
{
    public bool isPaused = false;
    public GameObject pauseCanvas;
    public GameObject player1;
    public TextMeshProUGUI player1Name;
    public TextMeshProUGUI player2Name;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player1 = GameObject.Find("TestPlayer");
        GameManager.instance.Init();
    }

    // Update is called once per frame
    void Update()
    {
        player1Name.text = GameManager.instance.GetPlayer(0).playerName;
        player2Name.text = GameManager.instance.GetPlayer(1).playerName;
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
