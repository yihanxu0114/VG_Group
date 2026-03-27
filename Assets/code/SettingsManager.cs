using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("UI References")]
    public Slider brightnessSlider;
    public Slider volumeSlider;
    public Image brightnessOverlay;

    private const string BrightnessKey = "Brightness";
    private const string VolumeKey = "Volume";

    void Start()
    {
        float savedBrightness = PlayerPrefs.GetFloat(BrightnessKey, 1f);
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);

        if (brightnessSlider != null)
        {
            brightnessSlider.value = savedBrightness;
            brightnessSlider.onValueChanged.AddListener(SetBrightness);
        }

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        SetBrightness(savedBrightness);
        SetVolume(savedVolume);
    }

    public void SetBrightness(float value)
    {
        value = Mathf.Clamp01(value);

        if (brightnessOverlay != null)
        {
            Color c = brightnessOverlay.color;

            // value = 1 -> alpha = 0   (正常亮度)
            // value = 0 -> alpha = 0.7 (更暗)
            c.a = (1f - value) * 0.7f;

            brightnessOverlay.color = c;
        }

        PlayerPrefs.SetFloat(BrightnessKey, value);
        PlayerPrefs.Save();
    }

    public void SetVolume(float value)
    {
        value = Mathf.Clamp01(value);
        AudioListener.volume = value;

        PlayerPrefs.SetFloat(VolumeKey, value);
        PlayerPrefs.Save();
    }
}