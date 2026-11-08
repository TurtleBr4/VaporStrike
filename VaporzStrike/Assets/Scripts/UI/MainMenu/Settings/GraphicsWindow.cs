using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GraphicsWindow : SubWindow
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Toggle fullscreenToggle;
    public Toggle vSyncToggle;
    public Toggle shadowsToggle;
    public Toggle postProcessingToggle;



    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        WindowControls();

        fullscreenToggle.isOn = SettingsManager.instance.fullscreen;
        vSyncToggle.isOn = SettingsManager.instance.vSync;
        shadowsToggle.isOn = SettingsManager.instance.shadows;
        postProcessingToggle.isOn = SettingsManager.instance.postProcsessing;

        if (isActive)
        {

            foreach (var gamepad in Gamepad.all)
            {
                if (gamepad.buttonSouth.wasPressedThisFrame)
                {
                    switch (slotSel)
                    {
                        case 0:
                            SettingsManager.instance.ToggleFullscreen();
                            break;
                        case 1:
                            SettingsManager.instance.ToggleVsync();
                            break;
                        case 2:
                            SettingsManager.instance.ToggleShadows();
                            break;
                        case 3:
                            SettingsManager.instance.TogglePostProcessing();
                            break;
                    }
                }
            }
        }
    }
}
