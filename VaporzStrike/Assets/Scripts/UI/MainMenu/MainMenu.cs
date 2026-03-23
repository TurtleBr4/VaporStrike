using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Transform playerParent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Settings()
    {

    }
    public void Fight()
    {
        SceneManager.LoadScene("CharacterSelect");
    }
    public void More()
    {

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
