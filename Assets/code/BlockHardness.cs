using UnityEngine;

public class BlockHardness : MonoBehaviour
{
    [Header("Digging Setting")]
    // how many time it takes to dig through this block with a pickaxe. If the block does not have this component, we will use the default digTime in moving_logic instead.
    public float digTime = 0.5f;

    // can it be dug through with a pickaxe? If false, the player will not be able to dig through this block at all, and it will be treated as an indestructible block.
    public bool isUndiggable = false;
}