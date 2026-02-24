using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    public Image icon;

    public void SetItem(Sprite sprite)
    {
        if (sprite == null)
        {
            icon.enabled = false;
            return;
        }

        icon.enabled = true;
        icon.sprite = sprite;
    }

    public void Clear()
    {
        icon.enabled = false;
    }
}
