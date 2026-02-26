using UnityEngine;

public class GearSwitcher : MonoBehaviour
{
    private int selectedSlot = 1;
    private GearManager gearManager;

    void Awake()
    {
        if (gearManager == null)
            gearManager = GetComponent<GearManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetSlot(4);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SetSlot(5);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SetSlot(6);
        if (Input.GetKeyDown(KeyCode.Alpha7)) SetSlot(7);
        if (Input.GetKeyDown(KeyCode.Alpha8)) SetSlot(8);
        if (Input.GetKeyDown(KeyCode.Alpha9)) SetSlot(9);

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (gearManager != null)
                gearManager.UseCurrentGear(selectedSlot);
        }
    }

    private void SetSlot(int slot)
    {
        slot = Mathf.Clamp(slot, 1, 9);
        if (slot == selectedSlot) return;

        selectedSlot = slot;
        Debug.Log($"[GearSwitcher] Selected slot = {selectedSlot}");
    }
}