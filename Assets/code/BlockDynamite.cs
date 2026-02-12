using UnityEngine;

public class BlockDynamite : MonoBehaviour
{
    [HideInInspector] public bool destroyedThisRun = false;

    public void Break()
    {
        gameObject.SetActive(false); 
    }
    
}