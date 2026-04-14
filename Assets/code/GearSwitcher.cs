using UnityEngine;

public class GearSwitcher : MonoBehaviour
{
    private int selectedSlot = 1;
    private GearManager gearManager;
    private HotbarSystem hotbarSystem;

    public int GetSelectedSlot()
    {
        return selectedSlot;
    }

    void Awake()
    {
        if (gearManager == null)
            gearManager = GetComponent<GearManager>();

        if (hotbarSystem == null)
            hotbarSystem = FindObjectOfType<HotbarSystem>();
    }

    void Start()
    {
        if (hotbarSystem != null)
        {
            hotbarSystem.SetSelectedSlot(selectedSlot);
        }
    }

    void Update()
    {
        if (PauseMenuController.IsPaused)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) SetSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetSlot(4);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SetSlot(5);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SetSlot(6);
        if (Input.GetKeyDown(KeyCode.Alpha7)) SetSlot(7);
        if (Input.GetKeyDown(KeyCode.Alpha8)) SetSlot(8);
        if (Input.GetKeyDown(KeyCode.Alpha9)) SetSlot(9);

        // extra 4 slots
        if (Input.GetKeyDown(KeyCode.Z)) SetSlot(10);
        if (Input.GetKeyDown(KeyCode.X)) SetSlot(11);
        if (Input.GetKeyDown(KeyCode.C)) SetSlot(12);
        if (Input.GetKeyDown(KeyCode.V)) SetSlot(13);

        if (Input.GetMouseButtonDown(0))
        {
            if (gearManager != null)
                gearManager.UseCurrentGear(selectedSlot);
        }
    }

    private void SetSlot(int slot)
    {
        slot = Mathf.Clamp(slot, 1, 13);
        if (slot == selectedSlot) return;

        selectedSlot = slot;
        Debug.Log($"[GearSwitcher] Selected slot = {selectedSlot}");

        if (hotbarSystem == null)
            hotbarSystem = FindObjectOfType<HotbarSystem>();

        if (hotbarSystem != null)
        {
            hotbarSystem.SetSelectedSlot(selectedSlot);
        }
    }
}