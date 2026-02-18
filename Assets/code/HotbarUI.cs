using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotbarUI : MonoBehaviour
{
    [System.Serializable]
    public class SlotUI
    {
        public Image icon;
        public TMP_Text countText;
        public GameObject highlight;
    }

    public List<SlotUI> slots = new();
    public int selectedIndex = 0;

    void Awake()
    {
        slots.Clear();
        foreach (Transform slot in transform)
        {
            var icon = slot.Find("Icon")?.GetComponent<Image>();
            var count = slot.Find("Count")?.GetComponent<TMP_Text>();
            var highlight = slot.Find("Highlight")?.gameObject; // 没有也没事

            slots.Add(new SlotUI { icon = icon, countText = count, highlight = highlight });

            // 保险：默认空
            if (icon) icon.enabled = false;
            if (count) count.text = "";
            if (highlight) highlight.SetActive(false);
        }
    }

    public void SetSelected(int index)
    {
        selectedIndex = Mathf.Clamp(index, 0, slots.Count - 1);
        for (int i = 0; i < slots.Count; i++)
            if (slots[i].highlight) slots[i].highlight.SetActive(i == selectedIndex);
    }

    public void SetSlot(int index, Sprite iconSprite, int count)
    {
        if (index < 0 || index >= slots.Count) return;
        var s = slots[index];

        if (s.icon)
        {
            s.icon.sprite = iconSprite;
            s.icon.enabled = (iconSprite != null);
        }

        if (s.countText)
            s.countText.text = (count <= 1 ? "" : count.ToString());
    }
}
