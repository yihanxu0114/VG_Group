using UnityEngine;

public class GearManager : MonoBehaviour
{
    [Header("Slot 1 - Dynamite")]
    public Dynamite dynamitePrefab;

    [Header("Slot 2 - Portal")]
    public Portal portalPrefab;

    public void UseCurrentGear(int slot)
    {
        switch (slot)
        {
            case 1:
                PlaceDynamite();
                break;
            case 2:
                PlacePortal();
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
      

        Instantiate(dynamitePrefab, transform.position + Vector3.up * 1.3f, Quaternion.identity);
    }

    private void PlacePortal()
    {
        if (portalPrefab == null)
        {
            Debug.LogWarning("GearManager: portalPrefab not assigned!");
            return;
        }

      
        if (PortalSystem.Instance != null && PortalSystem.Instance.Count() >= 2)
        {
            Debug.Log("Already has 2 portals.");
            return;
        }

        Instantiate(portalPrefab, transform.position + Vector3.up * 1.3f, Quaternion.identity);
       
    }
}