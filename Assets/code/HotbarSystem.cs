using System.Collections;
using UnityEngine;

public class HotbarSystem : MonoBehaviour
{
    public GearManager gearManager;

    [Header("UI")]
    public HotbarUI hotbarUI;

    [Header("Icons")]
    public Sprite pickaxeIcon;
    public Sprite bombIcon;

    [Header("Config")]
    public int bombCount = 5;

    public enum ToolType { Pickaxe, Bomb }
    public ToolType selectedTool = ToolType.Pickaxe;

    private int slotCountCached = 10; // 兜底：默认 10 格

    IEnumerator Start()
    {
        // 1) 自动找 UI（你也可以只靠手动拖）
        if (hotbarUI == null)
            hotbarUI = FindObjectOfType<HotbarUI>();

        if (hotbarUI == null)
        {
            Debug.LogError("HotbarSystem: hotbarUI is null. 请把 HotbarRoot(HotbarUI) 拖到 HotbarSystem 的 Hotbar UI 字段。");
            yield break;
        }

        // 2) 等一帧：确保 HotbarUI.Awake/Start 已经把 slots 收集好
        yield return null;

        // 3) 尝试从 HotbarUI 拿实际槽位数（如果你没加这个属性，就会走兜底 10）
        //    你如果不想改 HotbarUI，这段会安全兜底。
        try
        {
            slotCountCached = Mathf.Max(2, hotbarUI.SlotCount);
        }
        catch
        {
            slotCountCached = 10;
        }

        RefreshAllSlots();
        SelectSlot(0);
    }

    void Update()
    {
        int idx = ReadHotkeyIndex();
        if (idx != -1) SelectSlot(idx);
    }

    void RefreshAllSlots()
    {
        // slot 0: pickaxe (不显示数量)
        hotbarUI.SetSlot(0, pickaxeIcon, 1);

        // slot 1: bomb (显示数量)
        hotbarUI.SetSlot(1, bombIcon, bombCount);

        // 其它槽清空
        for (int i = 2; i < slotCountCached; i++)
            hotbarUI.SetSlot(i, null, 0);
    }

    int ReadHotkeyIndex()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) return 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) return 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) return 2;
        if (Input.GetKeyDown(KeyCode.Alpha4)) return 3;
        if (Input.GetKeyDown(KeyCode.Alpha5)) return 4;
        if (Input.GetKeyDown(KeyCode.Alpha6)) return 5;
        if (Input.GetKeyDown(KeyCode.Alpha7)) return 6;
        if (Input.GetKeyDown(KeyCode.Alpha8)) return 7;
        if (Input.GetKeyDown(KeyCode.Alpha9)) return 8;
        if (Input.GetKeyDown(KeyCode.Alpha0)) return 9;
        return -1;
    }

    public void SelectSlot(int index)
    {
        if (hotbarUI == null) return;

        // 防止越界（你只有 10 格时也 ok）
        index = Mathf.Clamp(index, 0, slotCountCached - 1);

        hotbarUI.SetSelected(index);

        if (index == 0) selectedTool = ToolType.Pickaxe;
        else if (index == 1) selectedTool = ToolType.Bomb;
        // 其它格子先不做功能

        
    }

    // 让炸弹脚本在“成功扔出/放下炸弹”后调用这个
    public void ConsumeBomb(int amount = 1)
    {
        bombCount = Mathf.Max(0, bombCount - amount);

        if (hotbarUI != null)
            hotbarUI.SetSlot(1, bombIcon, bombCount);
    }

    public bool HasBomb() => bombCount > 0;
}
