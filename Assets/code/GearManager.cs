using UnityEngine;

public class GearManager : MonoBehaviour
{
    [Header("Slot 1 - Dynamite")]
    public Dynamite dynamitePrefab;

    [Header("Slot 2 - Portal")]
    public Portal portalPrefab;

    [Header("Placement Offsets")]
    public float dynamiteForwardOffset = 1.0f;
    public float dynamiteUpOffset = 0.2f;
    public float portalOffset = 0.05f;

    [Header("Dynamite Limits")]
    public int dynamiteMaxCount = 30;
    public int dynamiteCount = 15;
    public float dynamitePlaceCooldown = 0.8f;

    private float _nextDynamiteTime = 0f;
    private SpriteRenderer _sr;

    private void Awake()
    {
        _sr = GetComponentInChildren<SpriteRenderer>();
    }

    public void UseCurrentGear(int slot)
    {
        switch (slot)
        {
            case 1: PlaceDynamite(); break;
            case 2: PlacePortal(); break;
            case 3: Debug.Log("Use Gear 3"); break;
            case 4: Debug.Log("Use Gear 4"); break;
        }
    }

    // store interface
    public bool AddDynamite(int amount)
    {
        if (amount <= 0) return false;
        int before = dynamiteCount;
        dynamiteCount = Mathf.Clamp(dynamiteCount + amount, 0, dynamiteMaxCount);
        return dynamiteCount > before;
    }
    
    public int GetDynamiteCount() => dynamiteCount;
    public bool CanPlaceDynamite() => dynamiteCount > 0 && Time.time >= _nextDynamiteTime;

    private void PlaceDynamite()
    {
        if (dynamitePrefab == null)
        {
            Debug.LogWarning("GearManager: dynamitePrefab not assigned!");
            return;
        }

        if (dynamiteCount <= 0)
        {
            Debug.Log("No dynamite left. Buy more in shop.");
            return;
        }

        if (Time.time < _nextDynamiteTime)
        {
            float remain = _nextDynamiteTime - Time.time;
            Debug.Log($"Dynamite cooldown: {remain:F2}s");
            return;
        }

        float facing = (_sr != null && _sr.flipX) ? -1f : 1f;
        Vector3 spawnPos =
            transform.position +
            Vector3.right * (facing * dynamiteForwardOffset) +
            Vector3.up * dynamiteUpOffset;

        Dynamite dyn = Instantiate(dynamitePrefab, spawnPos, Quaternion.identity);

        Collider2D[] playerCols = GetComponentsInChildren<Collider2D>(true);
        dyn.InitIgnorePlayer(playerCols, dyn.noCollisionTime);


        dynamiteCount--;
        _nextDynamiteTime = Time.time + dynamitePlaceCooldown;
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

        Instantiate(portalPrefab, transform.position + Vector3.up * portalOffset, Quaternion.identity);
    }
}