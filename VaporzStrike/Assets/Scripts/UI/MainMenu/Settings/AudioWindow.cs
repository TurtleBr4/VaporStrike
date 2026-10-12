using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SubWindow : MonoBehaviour
{
    public bool isActive = false;
    public int slotSel = 0;

    public Image[] slots = new Image[3];
    public GameObject container;

    public void WindowControls()
    {

        if (isActive)
        {
            foreach (var gamepad in Gamepad.all)
            {
                if (gamepad.buttonEast.wasPressedThisFrame)
                {
                    MainMenuScroll.instance.isSubmenu = false;
                    MainMenuScroll.instance.cooldown = 12;
                    isActive = false;
                }

                if (gamepad.dpad.up.wasPressedThisFrame)
                {
                    slotSel--;
                }
                if (gamepad.dpad.down.wasPressedThisFrame)
                {
                    slotSel++;
                }
            }

            container.SetActive(true);

            slotSel = Mathf.Clamp(slotSel, 0, slots.Length - 1);
            for (int i = 0; i < slots.Length; i++)
            {

                if (i == slotSel)
                {
                    slots[i].enabled = true;
                }
                else
                {
                    slots[i].enabled = false;
                }
            }

        }
        else
        {
            slotSel = 0;
            container.SetActive(false);
        }
    }
}
public class AudioWindow : SubWindow
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        WindowControls();

        masterSlider.value = SettingsManager.instance.masterVolume;
        bgmSlider.value = SettingsManager.instance.bgm;
        sfxSlider.value = SettingsManager.instance.sfx;

        if (isActive)
        {
            
            foreach (var gamepad in Gamepad.all)
            {
                if (gamepad.dpad.right.wasPressedThisFrame)
                {
                    switch (slotSel)
                    {
                        case 0:
                            SettingsManager.instance.IncrementMaster(0.1f);
                            break;
                        case 1:
                            SettingsManager.instance.IncrementBGM(0.1f);
                            break;
                        case 2:
                            SettingsManager.instance.IncrementSFX(0.1f);
                            break;
                    }

                }
                if (gamepad.dpad.left.wasPressedThisFrame)
                {
                    switch (slotSel)
                    {
                        case 0:
                            SettingsManager.instance.IncrementMaster(-0.1f);
                            break;
                        case 1:
                            SettingsManager.instance.IncrementBGM(-0.1f);
                            break;
                        case 2:
                            SettingsManager.instance.IncrementSFX(-0.1f);
                            break;
                    }
                }
            }
        }

    }
}
