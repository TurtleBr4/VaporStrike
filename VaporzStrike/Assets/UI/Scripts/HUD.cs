using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
public class HUD : MonoBehaviour
{
    public bool isPaused = false;


    public TextMeshProUGUI player1Name;
    public TextMeshProUGUI player2Name;

    public Image player1Img;
    public Image player2Img;

    public Slider p1Percent;
    public Slider p2Percent;

    public TextMeshProUGUI p1Stocks;
    public TextMeshProUGUI p2Stocks;

    public static HUD instance;

    public TextMeshProUGUI timerText;
    public GameObject timer;
    public InputActionReference returnInput;

    public GameObject matchResultCanvas;
    public TextMeshProUGUI matchResultTxt;

    public GameObject countdown;

    [Header("Pause Menu")]
    public GameObject pauseCanvas;
    public int pauseNavigation = 0;
    public bool isSelected = false;
    public GameObject normalModePanel;
    public GameObject howToPlayPanel;
    public Toggle[] pauseToggles;
    public GameObject[] trainingModePanels;
    private int cooldown = 0;





    [Header("Training Menu")]
    public GameObject trainingCanvas;
    public bool isTrainingSettings = false;
    private Vector3 trainingOffset = new Vector3(-7.85f, 0, -10);
    public int trainingNavigation = 0;
    public bool isSelectedTraining = false;
    public Toggle[] pauseTogglesTraining;
    public TextMeshProUGUI selectionTexts;
    public float trainingSpeed = 1f;
    private int selectedItem = 0;
    public enum PracticeControl
    {
        None,
        AI,
        Player_Two
    }
    public PracticeControl control;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //player1 = GameObject.Find("TestPlayer");
        GameManager.instance.Init();
        GameManager.instance.changeState(gameState.Game);
       //GameManager.instance.matchTimer = 200;
        GameManager.instance.isMatch = true;
        
    }
    private void Awake()
    {
        countdown.gameObject.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        instance = this;

        player1Name.text = GameManager.instance.GetPlayer(1).playerName; //game manager shenanagains, sorry
        player2Name.text = GameManager.instance.GetPlayer(0).playerName;

        player1Img.sprite = GameManager.instance.GetPlayer(1).mugshot;
        player2Img.sprite = GameManager.instance.GetPlayer(0).mugshot;

        pauseCanvas.SetActive(isPaused);
        trainingCanvas.SetActive(isTrainingSettings);
        if (isTrainingSettings)
        {
            Camera.main.GetComponent<CameraController>().offset = trainingOffset;
        }
        else
        {
            Camera.main.GetComponent<CameraController>().offset = new Vector3(0,0,-10);
        }

        if(GameManager.instance.isTraining) ItemManager.instance.itemsEnabled = false;

        if (Gamepad.current.startButton.wasPressedThisFrame && !isTrainingSettings)
        {
            isPaused = !isPaused;
        }
        if (Gamepad.current.selectButton.wasPressedThisFrame && !isPaused && GameManager.instance.isTraining)
        {
            isTrainingSettings = !isTrainingSettings;
        }
        timer.SetActive(!GameManager.instance.isTraining);
        if (isPaused && Gamepad.current.selectButton.wasPressedThisFrame) 
        {
            if (GameManager.instance.isTraining)
            {
                isPaused = false;
                Time.timeScale = 1f;
                GameManager.instance.changeState(gameState.Menu);
                GameManager.instance.isMatch = false;
                GameManager.instance.isTraining = false;
                SceneManager.LoadScene("MainMenu");
            }
            else
            {
                ToCharacterSelect();
            }

        }
        timerText.text = GameManager.instance.DisplayTime();

        if(((!GameManager.instance.isMatch) && (GameManager.instance.matchResult != ResultType.Start)))
        {
            matchResultCanvas.gameObject.SetActive(true);
            Time.timeScale *= (Time.timeScale*0.1f);

            if (Gamepad.current.startButton.wasPressedThisFrame) {
                isPaused = false;
                Time.timeScale = 1f; 
                GameManager.instance.changeState(gameState.Menu);
                GameManager.instance.isMatch = false;
                SceneManager.LoadScene("CharacterSelect");

            }
        }
        else
        {
            matchResultCanvas.gameObject.SetActive(false);
            Time.timeScale = (isPaused ? 0.0f : 1.0f);
        }
        if (cooldown > 0)
            cooldown--;

        pauseNavigation = Mathf.Clamp(pauseNavigation, 0, 2); trainingNavigation = Mathf.Clamp(trainingNavigation, 0, 2); 
        if (isPaused)
        {
            for (int i = 0; i < pauseToggles.Length; i++)
            {
                if (pauseToggles[i] != null && i == pauseNavigation)
                {
                    pauseToggles[i].SetIsOnWithoutNotify(true);
                }
                else
                {
                    pauseToggles[i].SetIsOnWithoutNotify(false);
                }
            }
            foreach (var gamepad in Gamepad.all)
                {
                    if (gamepad.buttonEast.wasPressedThisFrame && cooldown == 0)
                    {
                        if (isSelected)
                        {
                            isSelected = false;
                            cooldown = 25;
                            PauseUnselect();
                        }
                        else
                        {
                            isSelected = false;
                            cooldown = 25;
                            Resume();
                        }
                    }
                    if (gamepad.buttonSouth.wasPressedThisFrame && cooldown == 0)
                    {
                        isSelected = true;
                        PauseSelect(pauseNavigation);
                        cooldown = 25;
                    }

                    if (gamepad.dpad.up.wasPressedThisFrame && !isSelected)
                    {
                        pauseNavigation--;
                    }
                    if (gamepad.dpad.down.wasPressedThisFrame && !isSelected)
                {
                        pauseNavigation++;
                    }
                }
        }
        if (isTrainingSettings)
        {
            for (int i = 0; i < pauseTogglesTraining.Length; i++)
            {
                if (pauseTogglesTraining[i] != null && i == trainingNavigation)
                {
                    pauseTogglesTraining[i].SetIsOnWithoutNotify(true);
                }
                else
                {
                    pauseTogglesTraining[i].SetIsOnWithoutNotify(false);
                }
            }
            foreach (var gamepad in Gamepad.all)
            {
                if (gamepad.buttonEast.wasPressedThisFrame && cooldown == 0)
                {
                    if (isSelected)
                    {
                        isSelectedTraining = false;
                        cooldown = 25;
                        PauseUnselect();
                    }
                    else
                    {
                        isSelectedTraining = false;
                        cooldown = 25;
                        Resume();
                    }
                }
                if (gamepad.buttonSouth.wasPressedThisFrame && cooldown == 0)
                {
                    isSelectedTraining = true;
                    TrainingSelect(trainingNavigation);
                    cooldown = 25;
                }

                if (gamepad.dpad.up.wasPressedThisFrame && !isSelectedTraining)
                {
                    trainingNavigation--;
                }
                if (gamepad.dpad.down.wasPressedThisFrame && !isSelectedTraining)
                {
                    trainingNavigation++;
                }
                if (gamepad.dpad.left.wasPressedThisFrame && !isSelectedTraining)
                {
                    Increment(trainingNavigation, -1);
                }
                if (gamepad.dpad.right.wasPressedThisFrame && !isSelectedTraining)
                {
                    Increment(trainingNavigation, 1);
                }
            }
        }

        control = (PracticeControl)Math.Clamp((int)control, 0, 2);
        selectedItem = Mathf.Clamp(selectedItem, 0, GameManager.instance.GetLootPool().Length-1);
        trainingSpeed = Mathf.Clamp(trainingSpeed, 0, 2f);
        selectionTexts.text = $"Game Speed: {trainingSpeed}x\r\n\r\nPractice Control: {control.ToString()}\r\n\r\nSpawn Item: {GameManager.instance.GetLootPool()[selectedItem].itemName}";
        switch (GameManager.instance.matchResult)
            {
                case ResultType.NoContest:
                    matchResultTxt.text = "No Contest!";
                    break;
                case ResultType.Player1:
                    matchResultTxt.text = "Player 1 Wins!";
                    break;
                case ResultType.Player2:
                    matchResultTxt.text = "Player 2 Wins!";
                    break;
            }
    }
    public void Increment(int index, int amount)
    {
        switch (index)
        {

            case 0:
                trainingSpeed += amount * 0.25f;
                break;
            case 1:
                control += amount;
                break;
            case 2:
                selectedItem += amount;
                break;

        }
    }
    public void PauseSelect(int val)
    {
        switch (val) {

            case 0:
                Resume();
                break;
            case 1:
                ShowHowToPlay();
                break;
            case 2:
                if (GameManager.instance.isTraining)
                {
                    ToMainMenu();
                }
                else
                {
                    ToCharacterSelect();
                }
                    break;

        }
    }
    public void TrainingSelect(int val)
    {
        switch (val)
        {

            case 0:
                //Resume();
                isSelectedTraining = false;
                break;
            case 1:
                //Resume();
                isSelectedTraining = false;
                break;
            case 2:
                ItemManager.instance.SpawnItem(GameManager.instance.GetLootPool()[selectedItem]);
                isSelectedTraining = false;
                break;

        }
    }
    public void PauseUnselect()
    {
        ShowMainPause();
    }
    public void ShowHowToPlay()
    {
        normalModePanel.SetActive(false);
        howToPlayPanel.SetActive(true);
    }
    public void ShowMainPause()
    {
        normalModePanel.SetActive(true);
        howToPlayPanel.SetActive(false);
    }
    public void Resume()
    {
        isSelected = false;
        pauseNavigation = 0;
        isPaused = false;

        isTrainingSettings = false;
        isSelectedTraining = false;
    }
    public void ToCharacterSelect()
    {
        GameManager.instance.isTraining = false;
        isPaused = false;
        Time.timeScale = 1f;
        GameManager.instance.changeState(gameState.Menu);
        GameManager.instance.isMatch = false;
        SceneManager.LoadScene("CharacterSelect");
    }
    public void ToMainMenu()
    {
        GameManager.instance.isTraining = false;
        isPaused = false;
        Time.timeScale = 1f;
        GameManager.instance.changeState(gameState.Menu);
        GameManager.instance.isMatch = false;
        SceneManager.LoadScene("MainMenu");
    }
}
