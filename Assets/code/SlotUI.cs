using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text countText;

    public Color normalIconColor = Color.white;
    public Color selectedIconColor = new Color(1f, 0.78f, 0.28f, 1f);

    bool isSelected = false;

    public void Set(Sprite sprite, int count)
    {
        if (icon != null)
        {
            icon.sprite = sprite;
            icon.enabled = sprite != null;
            icon.color = isSelected ? selectedIconColor : normalIconColor;
        }

        if (countText != null)
        {
            countText.text = (sprite != null && count > 1) ? $"x{count}" : "";
        }
    }

    public void Clear()
    {
        Set(null, 0);
    }

    public void SetSelected(bool on)
    {
        isSelected = on;

        if (icon != null && icon.enabled)
        {
            icon.color = on ? selectedIconColor : normalIconColor;
        }
    }
}