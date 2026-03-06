using UnityEngine;

public class HotbarSystem : MonoBehaviour
{
    public HotbarUI hotbarUI;
    public GearManager gearManager;
    public PlayerInventory inv;

    public int gearSlotCount = 5;
    public int totalSlots = 10;

    public Sprite portalIcon;
    public Sprite bomb2Icon;
    public Sprite bomb3Icon;
    public Sprite bomb4Icon;
    public Sprite bomb5Icon;

    public Sprite goldIcon;
    public Sprite diamondIcon;
    public Sprite rubyIcon;
    public Sprite emeraldIcon;

    int _gold;
    int _diamond;
    int _ruby;
    int _emerald;

    int _bomb2;
    int _bomb3;
    int _bomb4;
    int _bomb5;

    bool _hasPortal;
    bool _hasBomb2;
    bool _hasBomb3;
    bool _hasBomb4;
    bool _hasBomb5;

    void Start()
    {
        if (hotbarUI == null) hotbarUI = FindObjectOfType<HotbarUI>();

        Debug.Log("[HotbarSystem] portalIcon = " + (portalIcon == null ? "NULL" : portalIcon.name));
        Debug.Log("[HotbarSystem] bomb2Icon = " + (bomb2Icon == null ? "NULL" : bomb2Icon.name));
        Debug.Log("[HotbarSystem] bomb3Icon = " + (bomb3Icon == null ? "NULL" : bomb3Icon.name));
        Debug.Log("[HotbarSystem] bomb4Icon = " + (bomb4Icon == null ? "NULL" : bomb4Icon.name));
        Debug.Log("[HotbarSystem] bomb5Icon = " + (bomb5Icon == null ? "NULL" : bomb5Icon.name));

        hotbarUI.ClearAll();
        hotbarUI.SetSlot(0, portalIcon, 1);
        hotbarUI.SetSlot(1, bomb2Icon, 5);
        hotbarUI.SetSlot(2, bomb3Icon, 5);
        hotbarUI.SetSlot(3, bomb4Icon, 5);
        hotbarUI.SetSlot(4, bomb5Icon, 5);
    }

    void Update()
    {
        RefreshAll(false);
    }

    void RefreshAll(bool force)
    {
        if (hotbarUI == null) return;
        if (gearManager == null) gearManager = FindObjectOfType<GearManager>();
        if (inv == null) inv = FindObjectOfType<PlayerInventory>();

        int gold = inv != null ? inv.goldFragments : 0;
        int diamond = inv != null ? inv.diamondFragments : 0;
        int ruby = inv != null ? inv.rubyFragments : 0;
        int emerald = inv != null ? inv.emeraldFragments : 0;

        bool hasPortal = gearManager != null && gearManager.portalPrefab != null;

        int bomb2 = gearManager != null ? gearManager.GetBombCount(gearManager.bombName2) : 0;
        int bomb3 = gearManager != null ? gearManager.GetBombCount(gearManager.bombName3) : 0;
        int bomb4 = gearManager != null ? gearManager.GetBombCount(gearManager.bombName4) : 0;
        int bomb5 = gearManager != null ? gearManager.GetBombCount(gearManager.bombName5) : 0;

        bool hasBomb2 = gearManager != null && gearManager.dynamitePrefab != null && bomb2 > 0;
        bool hasBomb3 = gearManager != null && gearManager.bombSlot3 != null && bomb3 > 0;
        bool hasBomb4 = gearManager != null && gearManager.bombSlot4 != null && bomb4 > 0;
        bool hasBomb5 = gearManager != null && gearManager.bombSlot5 != null && bomb5 > 0;

        if (!force &&
            gold == _gold &&
            diamond == _diamond &&
            ruby == _ruby &&
            emerald == _emerald &&
            bomb2 == _bomb2 &&
            bomb3 == _bomb3 &&
            bomb4 == _bomb4 &&
            bomb5 == _bomb5 &&
            hasPortal == _hasPortal &&
            hasBomb2 == _hasBomb2 &&
            hasBomb3 == _hasBomb3 &&
            hasBomb4 == _hasBomb4 &&
            hasBomb5 == _hasBomb5)
            return;

        _gold = gold;
        _diamond = diamond;
        _ruby = ruby;
        _emerald = emerald;

        _bomb2 = bomb2;
        _bomb3 = bomb3;
        _bomb4 = bomb4;
        _bomb5 = bomb5;

        _hasPortal = hasPortal;
        _hasBomb2 = hasBomb2;
        _hasBomb3 = hasBomb3;
        _hasBomb4 = hasBomb4;
        _hasBomb5 = hasBomb5;

        hotbarUI.ClearAll();

        hotbarUI.SetSlot(0, hasPortal ? portalIcon : null, hasPortal ? 1 : 0);
        hotbarUI.SetSlot(1, hasBomb2 ? bomb2Icon : null, bomb2);
        hotbarUI.SetSlot(2, hasBomb3 ? bomb3Icon : null, bomb3);
        hotbarUI.SetSlot(3, hasBomb4 ? bomb4Icon : null, bomb4);
        hotbarUI.SetSlot(4, hasBomb5 ? bomb5Icon : null, bomb5);

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
}