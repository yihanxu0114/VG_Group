using System.Collections.Generic;
using UnityEngine;

public class HotbarUI : MonoBehaviour
{
    public Transform slotContainer;

    [SerializeField] private List<SlotUI> slots = new();

    public int SlotCount => slots.Count;

    void Awake()
    {
        Rebuild();
    }

    [ContextMenu("Rebuild Slots")]
    public void Rebuild()
    {
        slots.Clear();
        if (slotContainer == null) return;

        for (int i = 0; i < slotContainer.childCount; i++)
        {
            var s = slotContainer.GetChild(i).GetComponent<SlotUI>();
            if (s != null) slots.Add(s);
        }
    }

    public void ClearAll()
    {
        for (int i = 0; i < slots.Count; i++) slots[i].Clear();
    }

    public void SetSlot(int index, Sprite sprite, int count)
    {
        if (index < 0 || index >= slots.Count) return;
        slots[index].Set(sprite, count);
    }

    public void SetSelected(int index)
    {
        for (int i = 0; i < slots.Count; i++)
            slots[i].SetSelected(i == index);
    }
}