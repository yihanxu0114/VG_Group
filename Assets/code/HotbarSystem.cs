using UnityEngine;

public class HotbarSystem : MonoBehaviour
{
    public HotbarUI hotbarUI;
    public GearManager gearManager;
    public PlayerInventory inv;

    public int gearSlotCount = 8;
    public int totalSlots = 13;

    [Header("Gear Icons")]
    public Sprite portalIcon;
    public Sprite bomb2Icon;
    public Sprite bomb3Icon;
    public Sprite bomb4Icon;
    public Sprite bomb5Icon;
    public Sprite bullet6Icon;
    public Sprite bullet7Icon;
    public Sprite bullet8Icon;

    [Header("Material Icons")]
    public Sprite goldIcon;
    public Sprite diamondIcon;
    public Sprite rubyIcon;
    public Sprite emeraldIcon;

    int _gold, _diamond, _ruby, _emerald;
    int _bomb2, _bomb3, _bomb4, _bomb5;
    int _bullet6, _bullet7, _bullet8;
    bool _hasPortal;

    void Start()
    {
        if (hotbarUI == null) hotbarUI = FindObjectOfType<HotbarUI>();
        RefreshAll(true);
    }

    void Update()
    {
        RefreshAll(false);
    }

    private int GetItemCount(string targetName)
    {
        if (inv == null) return 0;

        foreach (var item in inv.myItems)
        {
            if (item.itemName == targetName) return item.count;
        }

        return 0;
    }

    void RefreshAll(bool force)
    {
        if (hotbarUI == null) return;
        if (gearManager == null) gearManager = FindObjectOfType<GearManager>();
        if (inv == null) inv = FindObjectOfType<PlayerInventory>();
        if (inv == null) return;

        int gold = inv.goldFragments;
        int diamond = inv.diamondFragments;
        int ruby = inv.rubyFragments;
        int emerald = inv.emeraldFragments;

        bool hasPortal = gearManager != null && gearManager.portalPrefab != null;

        int bomb2 = GetItemCount("Bomb");
        int bomb3 = GetItemCount("BlueBomb");
        int bomb4 = GetItemCount("RedBomb");
        int bomb5 = GetItemCount("GrayBomb");

        int bullet6 = GetItemCount("Bullet6");
        int bullet7 = GetItemCount("Bullet7");
        int bullet8 = GetItemCount("Bullet8");

        if (!force &&
            gold == _gold && diamond == _diamond && ruby == _ruby && emerald == _emerald &&
            bomb2 == _bomb2 && bomb3 == _bomb3 && bomb4 == _bomb4 && bomb5 == _bomb5 &&
            bullet6 == _bullet6 && bullet7 == _bullet7 && bullet8 == _bullet8 &&
            hasPortal == _hasPortal)
            return;

        _gold = gold;
        _diamond = diamond;
        _ruby = ruby;
        _emerald = emerald;
        _bomb2 = bomb2;
        _bomb3 = bomb3;
        _bomb4 = bomb4;
        _bomb5 = bomb5;
        _bullet6 = bullet6;
        _bullet7 = bullet7;
        _bullet8 = bullet8;
        _hasPortal = hasPortal;

        hotbarUI.ClearAll();

        hotbarUI.SetSlot(0, hasPortal ? portalIcon : null, hasPortal ? 1 : 0);
        hotbarUI.SetSlot(1, bomb2 > 0 ? bomb2Icon : null, bomb2);
        hotbarUI.SetSlot(2, bomb3 > 0 ? bomb3Icon : null, bomb3);
        hotbarUI.SetSlot(3, bomb4 > 0 ? bomb4Icon : null, bomb4);
        hotbarUI.SetSlot(4, bomb5 > 0 ? bomb5Icon : null, bomb5);

        hotbarUI.SetSlot(5, bullet6 > 0 ? bullet6Icon : null, bullet6);
        hotbarUI.SetSlot(6, bullet7 > 0 ? bullet7Icon : null, bullet7);
        hotbarUI.SetSlot(7, bullet8 > 0 ? bullet8Icon : null, bullet8);

        int write = gearSlotCount;
        write = WriteMat(write, goldIcon, gold);
        write = WriteMat(write, diamondIcon, diamond);
        write = WriteMat(write, rubyIcon, ruby);
        write = WriteMat(write, emeraldIcon, emerald);
    }

    int WriteMat(int index, Sprite icon, int count)
    {
        if (count <= 0) return index;
        if (index >= totalSlots) return index;

        hotbarUI.SetSlot(index, icon, count);
        return index + 1;
    }

    public void SetSelectedSlot(int slotNumber)
    {
        if (hotbarUI == null) return;
        hotbarUI.SetSelected(slotNumber - 1);
    }
}