using UnityEngine;

public class GearManager : MonoBehaviour
{
    public void UseCurrentGear(int slot)
    {
        switch (slot)
        {
            case 1:
                Debug.Log("Use Gear 1 (future: Dynamite)");
                break;
            case 2:
                Debug.Log("Use Gear 2 (future: Metal Detector)");
                break;
            case 3:
                Debug.Log("Use Gear 3");
                break;
            case 4:
                Debug.Log("Use Gear 4");
                break;
        }
    }
}