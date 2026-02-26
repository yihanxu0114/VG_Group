using UnityEngine;

public class GearManager : MonoBehaviour
{
    [Header("Slot 1 - Dynamite")]
    public Dynamite dynamitePrefab;

    [Header("Slot 2 - Portal")]
    public Portal portalPrefab;

    [Header("Slot 3-5 - Extra Bombs")]
    public Dynamite bombSlot3;
    public Dynamite bombSlot4;
    public Dynamite bombSlot5;

    [Header("Placement Offsets")]
    public float dynamiteForwardOffset = 1.0f;
    public float dynamiteUpOffset = 0.2f;
    public float portalOffset = 0.05f;

    [Header("Dynamite Limits")]
    public int dynamiteMaxCount = 30;
    public float dynamitePlaceCooldown = 0.8f;

    private float _nextDynamiteTime = 0f;
    private SpriteRenderer _sr;
    private PlayerInventory _inv;

    private void Awake()
    {
        _sr = GetComponentInChildren<SpriteRenderer>();
        _inv = GetComponent<PlayerInventory>();
    }

    public void UseCurrentGear(int slot)
    {
        switch (slot)
        {
            case 1: PlaceBomb(dynamitePrefab); break;
            case 2: PlacePortal(); break;
            case 3: PlaceBomb(bombSlot3); break;
            case 4: PlaceBomb(bombSlot4); break;
            case 5: PlaceBomb(bombSlot5); break;
            default: Debug.Log($"No gear assigned for slot {slot}"); break;
        }
    }

    // ===== Inventory-backed bomb count (teammate logic kept) =====

    public bool AddDynamite(int amount)
    {
        if (amount <= 0 || _inv == null) return false;

        var item = FindBombItem();
        if (item == null) return false;

        int before = item.count;
        item.count = Mathf.Clamp(item.count + amount, 0, dynamiteMaxCount);
        if (item.count != before) _inv.UpdateUI();
        return item.count > before;
    }

    public int GetDynamiteCount()
    {
        var item = FindBombItem();
        return item != null ? item.count : 0;
    }

    public bool CanPlaceDynamite() => GetDynamiteCount() > 0 && Time.time >= _nextDynamiteTime;

    private void PlaceBomb(Dynamite prefab)
    {
        if (prefab == null)
        {
            Debug.LogWarning("GearManager: bomb prefab not assigned for this slot!");
            return;
        }

        if (!CanPlaceDynamite())
        {
            Debug.Log("Bomb is on cooldown or you have no bombs.");
            return;
        }

        var item = FindBombItem();
        if (item == null || item.count <= 0) return;

        // consume one bomb
        item.count--;
        _inv.UpdateUI();

        float facing = (_sr != null && _sr.flipX) ? -1f : 1f;
        Vector3 spawnPos =
            transform.position +
            Vector3.right * (facing * dynamiteForwardOffset) +
            Vector3.up * dynamiteUpOffset;

        Dynamite dyn = Instantiate(prefab, spawnPos, Quaternion.identity);

        Collider2D[] playerCols = GetComponentsInChildren<Collider2D>(true);
        dyn.InitIgnorePlayer(playerCols, dyn.noCollisionTime);

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

    // Helper: locate the "Bomb" item in inventory
    private dynamic FindBombItem()
    {
        if (_inv == null || _inv.myItems == null) return null;

        foreach (var item in _inv.myItems)
        {
            if (item != null && item.itemName == "Bomb")
                return item;
        }
        return null;
    }
}