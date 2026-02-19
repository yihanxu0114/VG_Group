using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotbarSystem : MonoBehaviour
{
    [Header("UI")]
    public HotbarUI hotbarUI;

    [Header("Icons")]
    public Sprite pickaxeIcon;
    public Sprite bombIcon;

    [Header("Config")]
    public int bombCount = 5;

    public enum ToolType { Pickaxe, Bomb }
    public ToolType selectedTool = ToolType.Pickaxe;

    void Start()
    {
        // slot 0: pickaxe (不显示数量)
        hotbarUI.SetSlot(0, pickaxeIcon, 1);

        // slot 1: bomb (显示数量)
        hotbarUI.SetSlot(1, bombIcon, bombCount);

        // 其它槽清空
        for (int i = 2; i < 10; i++)
            hotbarUI.SetSlot(i, null, 0);

        SelectSlot(0);
    }

    void Update()
    {
        int idx = ReadHotkeyIndex();
        if (idx != -1) SelectSlot(idx);
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
        hotbarUI.SetSelected(index);

        if (index == 0) selectedTool = ToolType.Pickaxe;
        else if (index == 1) selectedTool = ToolType.Bomb;
        // 其它格子先不做功能
    }

    // 让炸弹脚本在“成功扔出/放下炸弹”后调用这个
    public void ConsumeBomb(int amount = 1)
    {
        bombCount = Mathf.Max(0, bombCount - amount);
        hotbarUI.SetSlot(1, bombIcon, bombCount);
    }

    public bool HasBomb() => bombCount > 0;
}
