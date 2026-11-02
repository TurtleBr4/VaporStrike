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
    public int cooldown = 0;

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

    //Colors
    private Color selectedCol = new Color(1f, 0.7686f, 0.1922f, 1f);
    private Color notSelectedCol = new Color(0.349f, 0.349f, 0.349f, 1);
    public bool isSubmenu = false;
    public SubWindow[] settingsSubwindows = new SubWindow[4];
    public SubWindow[] moreSubwindows = new SubWindow[4];

    [Header("Settings Menu")]
    public int settingsVal = 1;
    public TextMeshProUGUI audioTxt;
    public TextMeshProUGUI graphicsTxt;
    public TextMeshProUGUI controlsTxt;
    public TextMeshProUGUI gameplayTxt;

    [Header("More Menu")]
    public int moreVal = 1;
    public TextMeshProUGUI trainingTxt;
    public TextMeshProUGUI classicTxt;
    public TextMeshProUGUI arcadeTxt;
    public TextMeshProUGUI strikerpediaTxt;

    public static MainMenuScroll instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rt = GetComponent<RectTransform>();
        selectionVal = 1;
        menuNum = 1;
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        rt.anchoredPosition = new Vector2(currScroll, 120);
        if (cooldown > 0)
            cooldown--;

        settingsVal = Mathf.Clamp(settingsVal, 1, 4);
        moreVal = Mathf.Clamp(moreVal, 1, 4);

        if (!isSubmenu)
        {
            switch (menuNum)
            {

                case 0:
                    //Settings
                    foreach (var gamepad in Gamepad.all)
                    {
                        if (gamepad.buttonEast.wasPressedThisFrame && cooldown == 0)
                        {
                            menuNum = 1;
                            cooldown = 25;
                        }
                        if (gamepad.buttonSouth.wasPressedThisFrame && cooldown == 0)
                        {
                            SelectSubmenu(settingsVal);
                        }

                        if (gamepad.dpad.up.wasPressedThisFrame)
                        {
                            settingsVal--;
                        }
                        if (gamepad.dpad.down.wasPressedThisFrame)
                        {
                            settingsVal++;
                        }
                    }
                    RunSettings();
                    targetScroll = 2000;
                    break;

                case 1:
                    //Main Menu
                    settingsVal = 0;
                    moreVal = 0;

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
                            ThemeScript.instance.StopAll();
                            SceneManager.LoadScene("TitleScreen");
                        }

                        if (gamepad.buttonSouth.wasPressedThisFrame && cooldown == 0)
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

                            settingsImage.color = new Color(0, 0, 0, 0.5f);
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
                        if (gamepad.buttonEast.wasPressedThisFrame && cooldown == 0)
                        {
                            menuNum = 1;
                            cooldown = 25;
                        }
                        if (gamepad.buttonSouth.wasPressedThisFrame && cooldown == 0)
                        {
                            SelectSubmenu(moreVal);
                        }

                        if (gamepad.dpad.up.wasPressedThisFrame)
                        {
                            moreVal--;
                        }
                        if (gamepad.dpad.down.wasPressedThisFrame)
                        {
                            moreVal++;
                        }
                    }
                    RunMore();
                    targetScroll = -2000; break;
            }
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

    void SelectSubmenu(int submenuVal)
    {

        if(menuNum == 0 && settingsSubwindows[submenuVal - 1] != null)
        {
            isSubmenu = true;
            settingsSubwindows[submenuVal-1].isActive = true;
        }

        if (menuNum == 2 && moreSubwindows[submenuVal - 1] != null)
        {
            isSubmenu = true;
            moreSubwindows[submenuVal - 1].isActive = true;
        }
    }
    void RunSettings()
    {
        switch (settingsVal)
        {

            case 1:
                //Audio Settings
                flavorTxt.text = "Manage your game settings here.";

                audioTxt.color = selectedCol;
                graphicsTxt.color = notSelectedCol;
                controlsTxt.color = notSelectedCol;
                gameplayTxt.color = notSelectedCol;
                break;
            case 2:
                //Audio Settings
                flavorTxt.text = "Manage your game settings here.";

                audioTxt.color = notSelectedCol;
                graphicsTxt.color = selectedCol;
                controlsTxt.color = notSelectedCol;
                gameplayTxt.color = notSelectedCol;
                break;
            case 3:
                //Audio Settings
                flavorTxt.text = "Manage your game settings here.";

                audioTxt.color = notSelectedCol;
                graphicsTxt.color = notSelectedCol;
                controlsTxt.color = selectedCol;
                gameplayTxt.color = notSelectedCol;
                break;
            case 4:
                //Audio Settings
                flavorTxt.text = "Manage your game settings here.";

                audioTxt.color = notSelectedCol;
                graphicsTxt.color = notSelectedCol;
                controlsTxt.color = notSelectedCol;
                gameplayTxt.color = selectedCol;
                break;


        }
    }
    void RunMore()
    {
        switch (moreVal)
        {

            case 1:
                //Training Mode
                flavorTxt.text = "Manage your game settings here.";

                trainingTxt.color = selectedCol;
                classicTxt.color = notSelectedCol;
                arcadeTxt.color = notSelectedCol;
                strikerpediaTxt.color = notSelectedCol;
                break;
            case 2:
                //Classic Mode
                flavorTxt.text = "Manage your game settings here.";
                trainingTxt.color = notSelectedCol;
                classicTxt.color = selectedCol;
                arcadeTxt.color = notSelectedCol;
                strikerpediaTxt.color = notSelectedCol;
                break;
            case 3:
                //Arcade Mode
                flavorTxt.text = "Manage your game settings here.";

                trainingTxt.color = notSelectedCol;
                classicTxt.color = notSelectedCol;
                arcadeTxt.color = selectedCol;
                strikerpediaTxt.color = notSelectedCol;
                break;
            case 4:
                //Strikerpedia Wiki
                flavorTxt.text = "Manage your game settings here.";

                trainingTxt.color = notSelectedCol;
                classicTxt.color = notSelectedCol;
                arcadeTxt.color = notSelectedCol;
                strikerpediaTxt.color = selectedCol;
                break;


        }
    }








}