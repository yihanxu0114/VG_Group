using UnityEngine;

public class PortalSystem : MonoBehaviour
{
    public static PortalSystem Instance { get; private set; }

    [Header("Current Portals (runtime)")]
    public Portal portalA;
    public Portal portalB;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Register(Portal p)
    {
       
        if (portalA == p || portalB == p) return;

        if (portalA == null) portalA = p;
        else if (portalB == null) portalB = p;
        else
        {
           
            Destroy(p.gameObject);
            return;
        }

        LinkIfReady();
    }

    public void Unregister(Portal p)
    {
        if (portalA == p) portalA = null;
        if (portalB == p) portalB = null;

        
        if (portalA != null) portalA.linkedPortal = null;
        if (portalB != null) portalB.linkedPortal = null;

        LinkIfReady();
    }

    private void LinkIfReady()
    {
        if (portalA != null && portalB != null)
        {
            portalA.linkedPortal = portalB;
            portalB.linkedPortal = portalA;
        }
    }

    public int Count()
    {
        int c = 0;
        if (portalA != null) c++;
        if (portalB != null) c++;
        return c;
    }
}