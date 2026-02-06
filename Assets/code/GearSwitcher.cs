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

        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log($"[GearSwitcher] Use pressed. slot={selectedSlot}");
            gearManager.UseCurrentGear(selectedSlot); 
        }
    }

    private void SetSlot(int slot)
    {
        slot = Mathf.Clamp(slot, 1, 4);
        if (slot == selectedSlot) return;

        selectedSlot = slot;
        Debug.Log($"[GearSwitcher] Selected slot = {selectedSlot}");
    }
}