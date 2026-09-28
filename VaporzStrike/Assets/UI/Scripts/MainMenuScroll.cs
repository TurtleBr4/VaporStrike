using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScroll : MonoBehaviour
{
    public int menuNum = 0;
    public float targetScroll = 0;
    [SerializeField] private float currScroll = 0;
    private RectTransform rt;
    public int selectionVal = 1;
    private int cooldown = 0;

    [Header("Graphical Dimming")]
    public Image settingsImage;
    public Image strikeImage;
    public Image moreImage;

    [Header("Button Texts")]
    public TextMeshProUGUI settingsTxt;
    public TextMeshProUGUI strikeTxt;
    public TextMeshProUGUI moreTxt;

    [Header("Flavor Text")]
    public TextMeshProUGUI flavorTxt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rt = GetComponent<RectTransform>();
        selectionVal = 1;
        menuNum = 1;
    }

    // Update is called once per frame
    void Update()
    {
        rt.anchoredPosition = new Vector2(currScroll, 120);
        if (cooldown > 0)
            cooldown--;
        switch (menuNum)
        {

            case 0:
                //Settings
                foreach (var gamepad in Gamepad.all)
                {
                    if (gamepad.buttonEast.wasPressedThisFrame)
                    {
                        menuNum = 1;
                        cooldown = 25;
                    }
                }

                targetScroll = 2000; break;

            case 1:
                //Main Menu

                foreach (var gamepad in Gamepad.all)
                {
                    if (gamepad.dpad.right.wasPressedThisFrame && menuNum == 1)
                    {
                        selectionVal++;
                    }
                    if (gamepad.dpad.left.wasPressedThisFrame && menuNum == 1)
                    {
                        selectionVal--;
                    }

                    if (gamepad.buttonEast.wasPressedThisFrame && menuNum == 1 && cooldown == 0)
                    {
                        menuNum = 1;
                        SceneManager.LoadScene("TitleScreen");
                    }

                    if (gamepad.buttonSouth.wasPressedThisFrame)
                    {
                        switch (selectionVal)
                        {

                            case 0:
                                //Settings
                                menuNum = 0; break;
                            case 1:
                                //Main Menu
                                menuNum = 1;
                                SceneManager.LoadScene("CharacterSelect"); 
                                break;
                            case 2:
                                //More
                                menuNum = 2; break;
                        }

                        cooldown = 25;

                    }
                }

                switch (selectionVal)
                {

                    case 0:
                        //Settings
                        flavorTxt.text = "Manage your game settings here.";

                        settingsImage.color = new Color(0,0,0,0.5f);
                        strikeImage.color = new Color(0, 0, 0, 0);
                        moreImage.color = new Color(0, 0, 0, 0);

                        settingsTxt.color = new Color(1, 1, 1, 1f);
                        strikeTxt.color = new Color(0.2627f, 0.2627f, 0.2627f, 1);
                        moreTxt.color = new Color(0.2627f, 0.2627f, 0.2627f, 1);
                        break;
                    case 1:
                        //Main Menu
                        flavorTxt.text = "Settle the score with your friends!";

                        settingsImage.color = new Color(0, 0, 0, 0);
                        strikeImage.color = new Color(0, 0, 0, 0.5f);
                        moreImage.color = new Color(0, 0, 0, 0);

                        //Main Menu
                        settingsTxt.color = new Color(0.2627f, 0.2627f, 0.2627f, 1);
                        strikeTxt.color = new Color(1, 1, 1, 1f);
                        moreTxt.color = new Color(0.2627f, 0.2627f, 0.2627f, 1);
                        break;
                    case 2:
                        //More
                        flavorTxt.text = "Test your skill in Training Mode and more!";

                        settingsImage.color = new Color(0, 0, 0, 0);
                        strikeImage.color = new Color(0, 0, 0, 0);
                        moreImage.color = new Color(0, 0, 0, 0.5f);

                        //More
                        settingsTxt.color = new Color(0.2627f, 0.2627f, 0.2627f, 1);
                        strikeTxt.color = new Color(0.2627f, 0.2627f, 0.2627f, 1);
                        moreTxt.color = new Color(1, 1, 1, 1f);
                        break;
                }


                targetScroll = 0; break;
            case 2:
                //More
                foreach (var gamepad in Gamepad.all)
                {
                    if (gamepad.buttonEast.wasPressedThisFrame)
                    { 
                        menuNum = 1;
                        cooldown = 25;
                    }
                }
                    targetScroll = -2000; break;
        }

        currScroll = Mathf.Lerp(currScroll, targetScroll, 0.125f);
        if(selectionVal == -1)
        {
            selectionVal = 2;
        }
        if (selectionVal == 3)
        {
            selectionVal = 0;
        }
        
    }
}