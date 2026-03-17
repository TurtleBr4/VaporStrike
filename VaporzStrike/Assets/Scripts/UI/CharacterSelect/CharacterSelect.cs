using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class CharacterSelect : MonoBehaviour
{
    public TextMeshProUGUI player1Name;
    public TextMeshProUGUI player2Name;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Back();
        }

        if (GameManager.instance.GetPlayer(0))
        {
            player1Name.text = GameManager.instance.GetPlayer(0).playerName;
        }
        if (GameManager.instance.GetPlayer(1))
        {
            player2Name.text = GameManager.instance.GetPlayer(1).playerName;
        }
    }
    public void MatchStart()
    {
        if(GameManager.instance.GetPlayer(0)!= null && GameManager.instance.GetPlayer(1) != null)
        {
            SceneManager.LoadScene("SampleScene");
        }
        else
        {
            Debug.Log("Must select characters!");
        }

    }
    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }

    [SerializeField] private Transform playerParent; // Assign your container here
    private PlayerInputManager playerInputManager;

    void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        playerInputManager.onPlayerJoined += OnPlayerJoined;
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

            playerInput.gameObject.GetComponent<Cursor>().enabled = true;
        }
    }
}
