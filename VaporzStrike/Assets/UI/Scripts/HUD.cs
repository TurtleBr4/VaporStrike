using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;
public class HUD : MonoBehaviour
{
    public bool isPaused = false;
    public GameObject pauseCanvas;

    public TextMeshProUGUI player1Name;
    public TextMeshProUGUI player2Name;

    public TextMeshProUGUI p1Percent;
    public TextMeshProUGUI p2Percent;

    public TextMeshProUGUI p1Stocks;
    public TextMeshProUGUI p2Stocks;

    public static HUD instance;

    public TextMeshProUGUI timerText;

    public InputActionReference returnInput;

    public GameObject matchResultCanvas;
    public TextMeshProUGUI matchResultTxt;

    public GameObject countdown;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //player1 = GameObject.Find("TestPlayer");
        GameManager.instance.Init();
        GameManager.instance.onGameStateChanged(gameState.Game);
        GameManager.instance.matchTimer = 200;
        instance = this;
        GameManager.instance.isMatch = true;
        
    }
    private void Awake()
    {
        countdown.gameObject.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        player1Name.text = GameManager.instance.GetPlayer(0).playerName;
        player2Name.text = GameManager.instance.GetPlayer(1).playerName;

        pauseCanvas.SetActive(isPaused);
        Time.timeScale = (isPaused ? 0.0f : 1.0f);

        if (Gamepad.current.startButton.wasPressedThisFrame)
        {
            isPaused = !isPaused;
        }

        if(isPaused && returnInput.action.triggered) 
        {
            ToCharacterSelect();
        }
        timerText.text = GameManager.instance.DisplayTime();

        if(((!GameManager.instance.isMatch) && (GameManager.instance.matchResult != GameManager.ResultType.Start)))
        {
            matchResultCanvas.gameObject.SetActive(true);
            Time.timeScale *= (Time.timeScale*0.1f);

            if (Gamepad.current.startButton.wasPressedThisFrame) {

                SceneManager.LoadScene("CharacterSelect");
                GameManager.instance.onGameStateChanged(gameState.Menu);
            }
        }
        else
        {
            matchResultCanvas.gameObject.SetActive(false);
            Time.timeScale = 1;
        }



            switch (GameManager.instance.matchResult)
            {
                case GameManager.ResultType.NoContest:
                    matchResultTxt.text = "No Contest!";
                    break;
                case GameManager.ResultType.Player1:
                    matchResultTxt.text = "Player 1 Wins!";
                    break;
                case GameManager.ResultType.Player2:
                    matchResultTxt.text = "Player 2 Wins!";
                    break;
            }
    }

    public void ToCharacterSelect()
    {
        SceneManager.LoadScene("CharacterSelect");
    }
}
