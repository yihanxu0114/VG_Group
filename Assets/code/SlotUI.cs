using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text countText;
    public GameObject highlight;

    public void Set(Sprite sprite, int count)
    {
        if (icon != null)
        {
            icon.sprite = sprite;
            icon.enabled = (sprite != null);
        }

        if (countText != null)
            countText.text = (sprite != null && count > 1) ? $"x{count}" : "";
    }

    public void Clear()
    {
        Set(null, 0);
        if (highlight != null) highlight.SetActive(false);
    }

    public void SetSelected(bool on)
    {
        if (highlight != null) highlight.SetActive(on);
    }
}