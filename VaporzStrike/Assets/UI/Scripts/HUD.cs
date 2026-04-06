using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;
public class HUD : MonoBehaviour
{
    public bool isPaused = false;
    public GameObject pauseCanvas;
    public GameObject player1;
    public TextMeshProUGUI player1Name;
    public TextMeshProUGUI player2Name;

    public TextMeshProUGUI p1Percent;
    public TextMeshProUGUI p2Percent;

    public static HUD instance;

    public TextMeshProUGUI timerText;

    public InputActionReference returnInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //player1 = GameObject.Find("TestPlayer");
        GameManager.instance.Init();
        instance = this;
        GameManager.instance.isMatch = true;
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

        if(isPaused && returnInput.action.triggered) 
        {
            ToCharacterSelect();
        }
        timerText.text = GameManager.instance.DisplayTime();
    }

    public void ToCharacterSelect()
    {
        SceneManager.LoadScene("CharacterSelect");
    }
}
