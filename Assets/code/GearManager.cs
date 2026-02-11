using UnityEngine;

public class GearManager : MonoBehaviour
{
    [Header("Slot 1 - Dynamite")]
    public Dynamite dynamitePrefab;

    public void UseCurrentGear(int slot)
    {
        switch (slot)
        {
            case 1:
                PlaceDynamite();
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

    private void PlaceDynamite()
    {
        if (dynamitePrefab == null)
        {
            Debug.LogWarning("GearManager: dynamitePrefab not assigned!");
            return;
        }

        Instantiate(dynamitePrefab, transform.position+ Vector3.up * 1.3f, Quaternion.identity);
    }
}