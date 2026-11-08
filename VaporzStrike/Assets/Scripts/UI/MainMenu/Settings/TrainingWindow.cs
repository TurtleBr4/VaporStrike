using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TrainingWindow : SubWindow
{
    public int xSel = 0;
    public PlayerData[] availableCharacters;
    public PlayerData trainingDummy;
    public TextMeshProUGUI characterHandle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        WindowControls(); 
        xSel = Mathf.Clamp(xSel, 0, availableCharacters.Length-1);
        characterHandle.text = availableCharacters[xSel].playerName;
        if (isActive)
        {

            foreach (var gamepad in Gamepad.all)
            {
                if (gamepad.buttonSouth.wasPressedThisFrame)
                {
                    switch (slotSel)
                    {
                        case 1:
                            GameManager.instance.pickPlayer(0, availableCharacters[xSel]);
                            GameManager.instance.pickPlayer(1, trainingDummy);
                            GameManager.instance.isTraining = true;
                            SceneManager.LoadScene("Training");
                            break;
                    }
                }
                if (gamepad.dpad.left.wasPressedThisFrame)
                {
                    switch (slotSel)
                    {
                        case 0:
                            xSel--;
                            break;
                    }
                }
                if (gamepad.dpad.right.wasPressedThisFrame)
                {
                    switch (slotSel)
                    {
                        case 0:
                            xSel++;
                            break;
                    }
                }
            }
        }
        else
        {
            GameManager.instance.pickPlayer(0,null);
            GameManager.instance.pickPlayer(1, null);
        }
    }
}
