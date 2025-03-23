using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Timeline;
using UnityEngine.EventSystems;
using System.Collections;

public class SettingsManager : MonoBehaviour
{
    public CanvasGroup settingsCanvasGroup;
    public float fadeDuration = 0.5f; // Duration for the fade effect
    
    [Header("Audio Settings")]
    public Slider masterVolumeSlider;
    public TMPro.TextMeshProUGUI masterVolumeText;
    public Slider musicVolumeSlider;
    public TMPro.TextMeshProUGUI musicVolumeText;
    public Slider sfxVolumeSlider;
    public TMPro.TextMeshProUGUI sfxVolumeText;

    [Header("Graphics Settings")]
    public Slider brightnessSlider;
    public Image brightnessOverlay;
    public TMPro.TextMeshProUGUI brightnessText;
    public Slider gammaSlider;
    public Material gammaMaterial;
    public TMPro.TextMeshProUGUI gammaText;

    private const string GammaProperty = "_Gamma";

    private void Start()
    {
        settingsCanvasGroup.gameObject.SetActive(false);

        // Load saved settings
        LoadSettings();
        
        // Add listeners
        masterVolumeSlider.onValueChanged.AddListener((value) => {SetMasterVolume(value); });
        musicVolumeSlider.onValueChanged.AddListener((value) => {SetMusicVolume(value); });
        sfxVolumeSlider.onValueChanged.AddListener((value) => {SetSFXVolume(value); });
        brightnessSlider.onValueChanged.AddListener((value) => {SetBrightness(value); });
        gammaSlider.onValueChanged.AddListener((value) => {SetGamma(value); });
    }

    void Update()
    {
        // Toggle the pause menu when pressing the Escape key
        if (Input.GetKeyDown(KeyCode.Escape) && CheckIfSettings())
        {
            ApplySettings();
        }
    }

    private void SetMasterVolume(float value)
    {
        if (MusicTimeline.instance != null)
        {
            MusicTimeline.instance.SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 1.0f) * value);
        }

        PlayerPrefs.SetFloat("MasterVolume", value);

        // Update the text to show the new volume value
        if (masterVolumeText != null)
        {
            masterVolumeText.text = (value * 100).ToString("F0");
        }
    }

    private void SetMusicVolume(float value)
    {
        if (MusicTimeline.instance != null)
        {
            MusicTimeline.instance.SetMusicVolume(PlayerPrefs.GetFloat("MasterVolume", 1.0f) * value);
        }
        PlayerPrefs.SetFloat("MusicVolume", value);

        // Update the text to show the new volume value
        if (musicVolumeText != null)
        {
            musicVolumeText.text = (value * 100).ToString("F0");
        }
    }

    private void SetSFXVolume(float value)
    {
        // TODO: implement the actual setting
        PlayerPrefs.SetFloat("SFXVolume", value);

        // Update the text to show the new volume value
        if (sfxVolumeText != null)
        {
            sfxVolumeText.text = (value * 100).ToString("F0");
        }
    }

    private void SetBrightness(float value)
    {
        brightnessOverlay.color = new Color(0, 0, 0, 1 - value);
        PlayerPrefs.SetFloat("Brightness", value);

        // Update the text to show the new volume value
        if (brightnessText != null)
        {
            brightnessText.text = (value * 100).ToString("F0");
        }
    }

    private void SetGamma(float value)
    {
        if (gammaMaterial != null)
        {
            gammaMaterial.SetFloat(GammaProperty, value);
        }

        if (gammaText != null)
        {
            gammaText.text = $"{value:F2}";
        }

        PlayerPrefs.SetFloat("Gamma", value);
    }

    private void LoadSettings()
    {
        // Load master volume
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        SetMasterVolume(masterVolume);
        masterVolumeSlider.value = masterVolume;

        // Load music volume
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        SetMusicVolume(musicVolume);
        musicVolumeSlider.value = musicVolume;

        // Load SFX volume
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        SetSFXVolume(sfxVolume);
        sfxVolumeSlider.value = sfxVolume;

        // Load brightness
        float brightness = PlayerPrefs.GetFloat("Brightness", 1.0f);
        SetBrightness(brightness);
        brightnessSlider.value = brightness;

        // Load gamma
        float gamma = PlayerPrefs.GetFloat("Gamma", 1.0f);
        SetGamma(gamma);
        gammaSlider.value = gamma;
    }

    public void OpenSettings()
    {
        StartCoroutine(SceneFadeManager.Instance.FadeCanvasGroup(settingsCanvasGroup, 0, 1, fadeDuration));
        settingsCanvasGroup.gameObject.SetActive(true);
    }

    public bool CheckIfSettings()
    {
        return settingsCanvasGroup.gameObject.activeSelf;
    }

    public void ApplySettings()
    {
        PlayerPrefs.Save();
        
        StartCoroutine(FadeOutSettings());
    }

    private IEnumerator FadeOutSettings()
    {
        yield return SceneFadeManager.Instance.FadeCanvasGroup(settingsCanvasGroup, 1, 0, fadeDuration);

        // Deactivate and disable raycasts
        settingsCanvasGroup.gameObject.SetActive(false);
        settingsCanvasGroup.blocksRaycasts = false;
        
        // Reset selected UI element
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ResetSettings()
    {
        PlayerPrefs.DeleteAll();
        LoadSettings();
        Debug.Log("Settings reset to default.");
    }
}
