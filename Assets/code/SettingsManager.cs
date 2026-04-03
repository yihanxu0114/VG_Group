using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("Optional UI References")]
    public Slider brightnessSlider;
    public Slider volumeSlider;
    public Image brightnessOverlay;

    private const string BrightnessKey = "Brightness";
    private const string VolumeKey = "Volume";

    private float currentBrightness = 1f;
    private float currentVolume = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentBrightness = PlayerPrefs.GetFloat(BrightnessKey, 1f);
        currentVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        SetupUI();
        ApplyBrightness();
        ApplyVolume();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 每次切场景后，重新找当前场景里的 overlay
        FindBrightnessOverlayInScene();

        // 如果这个场景也有 slider，也可以重新绑定
        FindSlidersInScene();
        SetupUI();

        ApplyBrightness();
        ApplyVolume();
    }

    private void SetupUI()
    {
        if (brightnessSlider != null)
        {
            brightnessSlider.onValueChanged.RemoveListener(SetBrightness);
            brightnessSlider.value = currentBrightness;
            brightnessSlider.onValueChanged.AddListener(SetBrightness);
        }

        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(SetVolume);
            volumeSlider.value = currentVolume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    private void FindBrightnessOverlayInScene()
    {
        GameObject overlayObj = GameObject.Find("BrightnessOverlay");
        if (overlayObj != null)
        {
            brightnessOverlay = overlayObj.GetComponent<Image>();
        }
        else
        {
            brightnessOverlay = null;
        }
    }

    private void FindSlidersInScene()
    {
        Slider[] sliders = FindObjectsOfType<Slider>(true);

        brightnessSlider = null;
        volumeSlider = null;

        foreach (Slider s in sliders)
        {
            if (s.name == "BrightnessSlider")
            {
                brightnessSlider = s;
            }
            else if (s.name == "VolumeSlider")
            {
                volumeSlider = s;
            }
        }
    }

    public void SetBrightness(float value)
    {
        currentBrightness = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(BrightnessKey, currentBrightness);
        PlayerPrefs.Save();

        ApplyBrightness();
    }

    public void SetVolume(float value)
    {
        currentVolume = Mathf.Clamp01(value);
        AudioListener.volume = currentVolume;

        PlayerPrefs.SetFloat(VolumeKey, currentVolume);
        PlayerPrefs.Save();
    }

    public void ApplyBrightness()
    {
        if (brightnessOverlay != null)
        {
            Color c = brightnessOverlay.color;
            c.a = (1f - currentBrightness) * 0.7f;
            brightnessOverlay.color = c;
        }
    }

    public void ApplyVolume()
    {
        AudioListener.volume = currentVolume;
    }
}