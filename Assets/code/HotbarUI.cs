using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotbarUI : MonoBehaviour
{
    [Header("UI Container (holds Slot0~Slot9)")]
    public Transform slotContainer;

    [System.Serializable]
    public class SlotUI
    {
        public Image icon;
        public TMP_Text countText;
        public GameObject highlight;
    }

    [Header("Runtime collected slots")]
    [SerializeField] private List<SlotUI> slotUIs = new();

    [Header("State")]
    public int selectedIndex = 0;

    // ✅ 只有一个 SlotCount
    public int SlotCount => slotUIs?.Count ?? 0;

    void Awake()
    {
        RebuildSlots();
        SetSelected(selectedIndex);
    }

    public void RebuildSlots()
    {
        slotUIs.Clear();

        Transform container = slotContainer;

        // 如果没手动指定，就尝试用第一个子物体当容器
        if (container == null)
        {
            if (transform.childCount > 0) container = transform.GetChild(0);
        }

        if (container == null) return;

        for (int i = 0; i < container.childCount; i++)
        {
            Transform slot = container.GetChild(i);

            Image icon = null;
            TMP_Text count = null;
            GameObject highlight = null;

            var iconTf = slot.Find("Icon");
            if (iconTf != null) icon = iconTf.GetComponent<Image>();

            var countTf = slot.Find("Count");
            if (countTf != null) count = countTf.GetComponent<TMP_Text>();

            var hlTf = slot.Find("Highlight");
            if (hlTf != null) highlight = hlTf.gameObject;

            slotUIs.Add(new SlotUI { icon = icon, countText = count, highlight = highlight });

            if (icon != null) icon.enabled = false;
            if (count != null) count.text = "";
            if (highlight != null) highlight.SetActive(false);
        }
    }

    public void SetSelected(int index)
    {
        if (slotUIs.Count == 0) return;

        selectedIndex = Mathf.Clamp(index, 0, slotUIs.Count - 1);
        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (slotUIs[i].highlight != null)
                slotUIs[i].highlight.SetActive(i == selectedIndex);
        }
    }

    public void SetSlot(int index, Sprite iconSprite, int count)
    {
        if (index < 0 || index >= slotUIs.Count) return;

        var s = slotUIs[index];

        if (s.icon != null)
        {
            s.icon.sprite = iconSprite;
            s.icon.enabled = (iconSprite != null);
        }

        if (s.countText != null)
        {
            s.countText.text = (count <= 1 ? "" : count.ToString());
        }
    }

    public void ClearSlot(int index) => SetSlot(index, null, 0);

    public void ClearAll()
    {
        for (int i = 0; i < slotUIs.Count; i++)
            ClearSlot(i);

        SetSelected(selectedIndex);
    }
}
