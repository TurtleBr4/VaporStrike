using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Transform playerParent;
    private Animator animator;
    public static MainMenu instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (ThemeScript.instance != null)
        {
            ThemeScript.instance.mode = 0;
            ThemeScript.instance.PlayAll();
        }
        animator = GetComponent<Animator>();
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Settings()
    {

    }
    public void Transition()
    {
        SceneManager.LoadScene("CharacterSelect");
    }
    public void Fight()
    {
        GameObject.Destroy(GameObject.Find("BlackBack"));
        GameObject.Destroy(GameObject.Find("BlackFront"));
        animator.SetBool("Fight", true);
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

            playerInput.gameObject.GetComponent<CursorVirt>().enabled = true;
        }
    }
}
