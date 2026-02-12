using UnityEngine;

public class BlockDynamite : MonoBehaviour
{
    [HideInInspector] public bool destroyedThisRun = false;

    public void Break()
    {
        Debug.Log($"💥 方块 {gameObject.name} 被破坏！");
        Debug.Log($"🔍 调用堆栈:\n{System.Environment.StackTrace}");
        gameObject.SetActive(false); 
    }
}