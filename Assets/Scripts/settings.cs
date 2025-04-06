using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class settings : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider volumeSlider;
    public Toggle fullscreenToggle;
    public Slider sensitivitySlider;

    [Header("Sensitivity Range")]
    public float minSensitivity = 0.1f;
    public float maxSensitivity = 10f;

    public static float CurrentSensitivity { get; private set; } = 1f;

    private void Start()
    {
        // Load saved values
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        bool savedFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        float savedSensitivity = PlayerPrefs.GetFloat("Sensitivity", 1f);

        // Set UI values
        volumeSlider.value = savedVolume;
        fullscreenToggle.isOn = savedFullscreen;
        sensitivitySlider.value = savedSensitivity;

        // Apply settings
        AudioListener.volume = savedVolume;
        Screen.fullScreen = savedFullscreen;
        CurrentSensitivity = savedSensitivity;

        // Set listeners
        volumeSlider.onValueChanged.AddListener(SetVolume);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    public void SetSensitivity(float sensitivity)
    {
        CurrentSensitivity = Mathf.Clamp(sensitivity, minSensitivity, maxSensitivity);
        PlayerPrefs.SetFloat("Sensitivity", CurrentSensitivity);
    }

    public void BackToMenu()
    {
        // Replace with the correct scene
        SceneManager.LoadScene("IntroScene");
    }
}
