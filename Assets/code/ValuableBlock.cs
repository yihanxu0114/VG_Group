using UnityEngine;

public class ValuableBlock : MonoBehaviour
{
    // This script is attached to the gold and diamond blocks. It simply holds a variable that indicates
    // whether the block is gold or diamond, which will be used by the digging logic to determine how many points
    // to award the player for digging through it.
    public enum Type { Gold, Diamond }
    public Type blockType;
}