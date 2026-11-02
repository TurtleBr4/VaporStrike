using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class CharacterSelect : MonoBehaviour
{
    public TextMeshProUGUI player1Name;
    public TextMeshProUGUI player2Name;
    public Image player1Img;
    public Image player2Img;

    public LevelData chosenLevel;
    public string selectedLevel;

    public static CharacterSelect instance;

    public GameObject characterPanel;
    public GameObject StagePanel;

    public InputActionReference cancel;

    public TV tvScreen;
    public enum SelectMode
    {
        Character,
        Stage,
        Settings
    }
    public SelectMode selectMode = SelectMode.Character;
    public void Start()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Update()
    {
        if (Gamepad.current != null && Gamepad.current.selectButton.wasPressedThisFrame)
        {
            Back();
        }


        if (GameManager.instance.GetPlayer(0))
        {
            player1Name.text = GameManager.instance.GetPlayer(0).playerName;

            player1Img.sprite = GameManager.instance.GetPlayer(0).mugshot;
        }
        if (GameManager.instance.GetPlayer(1))
        {
            player2Name.text = GameManager.instance.GetPlayer(1).playerName;
            player2Img.sprite = GameManager.instance.GetPlayer(1).mugshot;
        }

        switch (selectMode) { 
                case SelectMode.Character:
                    StagePanel.SetActive(false);
                    selectedLevel = "";
                    ThemeScript.instance.mode = 1;

                if (cancel.action.triggered)
                {
                    Back();
                }
                break;
                case SelectMode.Stage:
                    StagePanel.SetActive(true);
                if(selectedLevel != null && selectedLevel != ""){
                    ThemeScript.instance.mode = 2;
                }
                else
                {
                    ThemeScript.instance.mode = 1;
                }

                if (cancel.action.triggered)
                {
                    selectMode = SelectMode.Character;
                    tvScreen.ResetScreen();
                }
                    break;
        
        }
    }
    public void MatchStart()
    {

        if(GameManager.instance.GetPlayer(0)!= null && GameManager.instance.GetPlayer(1) != null)
        {

            if (selectedLevel != null && selectedLevel!="") {
                Destroy(GameObject.Find("Theme"));
                SceneManager.LoadScene(selectedLevel);
            }
            else
            {
                Debug.Log("Must select characters!");
                selectMode = SelectMode.Stage;
            }
        }
        else
        {
            Debug.Log("Must select characters!");
        }


        //SceneManager.LoadScene("SampleScene");
    }
    public void Back()
    {
        GameManager.instance.Reset();
        SceneManager.LoadScene("MainMenu");
    }

    [SerializeField] private Transform playerParent; // Assign your container here
    private PlayerInputManager playerInputManager;

    void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        playerInputManager.onPlayerJoined += OnPlayerJoined;
    }

    public void SelectLevel(string levelHandle)
    {
        selectedLevel = levelHandle;
    }
    private void OnPlayerJoined(PlayerInput playerInput)
    {
        if (playerParent != null)
        {
            // Set the new player as a child of the container
            playerInput.transform.SetParent(playerParent);

            // Optionally reset local position/rotation
            playerInput.transform.localPosition = Vector3.zero;
            playerInput.transform.localRotation = Quaternion.identity;

            playerInput.gameObject.GetComponent<CursorVirt>().enabled = true;
            playerInput.gameObject.GetComponent<CursorVirt>().SetID(playerInput.playerIndex);
        }
    }
    public void LoadLevelOne()
    {
        chosenLevel.levelID = 1;
        
        SceneManager.LoadScene("Battlefield");
    }
    public void LoadLevelTwo()
    {
        chosenLevel.levelID = 2;
        
        SceneManager.LoadScene("Battlefield");
    }
}
