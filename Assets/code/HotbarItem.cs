using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotbarItem : MonoBehaviour
{
    public string id;        // "Pickaxe", "Bomb", "Portal"...
    public Sprite icon;      // UI显示的图标
    public int count;        // 数量（工具一般=1，炸弹这种会变）
}
