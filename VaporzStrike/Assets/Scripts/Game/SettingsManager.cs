using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using ShadowQuality = UnityEngine.ShadowQuality;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioMixer audioMix;
    public float masterVolume = 1f;
    public float bgm = 1f;
    public float sfx = 1f;

    [Header ("Graphics Settings")]
    public bool fullscreen = false;
    public bool vSync = true;
    public bool shadows = true;
    public bool postProcsessing = true;

    public static SettingsManager instance;
    private UniversalAdditionalCameraData camDat;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            Debug.Log("Destroying");
        }
        else
        {
            Debug.Log("Creating Instance");
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLoad;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLoad;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        camDat = Camera.main.GetUniversalAdditionalCameraData();
    }

    // Update is called once per frame
    void Update()
    {
        masterVolume = Mathf.Clamp(masterVolume, 0.00001f, 1f);
        bgm = Mathf.Clamp(bgm, 0.00001f, 1f);
        sfx = Mathf.Clamp(sfx, 0.00001f, 1f);

        audioMix.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
        audioMix.SetFloat("BGMVolume", Mathf.Log10(bgm) * 20);
        audioMix.SetFloat("SFXVolume", Mathf.Log10(sfx) * 20);

        Screen.fullScreen = fullscreen;
        QualitySettings.vSyncCount = vSync ? 1: 0;
        QualitySettings.shadows = shadows ? ShadowQuality.All : ShadowQuality.Disable;
        camDat.renderPostProcessing = postProcsessing;
    }

    public void IncrementMaster(float amount)
    {
        masterVolume += amount;
    }
    public void IncrementBGM(float amount)
    {
        bgm += amount;
    }
    public void IncrementSFX(float amount)
    {
        sfx += amount;
    }

    public void ToggleFullscreen()
    {
        fullscreen = !fullscreen;
    }
    public void ToggleVsync()
    {
        vSync = !vSync;
    }
    public void ToggleShadows()
    {
        shadows = !shadows;
    }
    public void TogglePostProcessing()
    {
        postProcsessing = !postProcsessing;
    }

    private void OnLoad(Scene scene, LoadSceneMode mode)
    {
        camDat = Camera.main.GetUniversalAdditionalCameraData();
    }
}
