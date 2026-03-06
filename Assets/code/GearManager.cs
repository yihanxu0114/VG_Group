using UnityEngine;

public class GearManager : MonoBehaviour
{
    public string bombName2 = "Bomb";
    public string bombName3 = "Bomb1";
    public string bombName4 = "Bomb2";
    public string bombName5 = "Bomb3";

    public Portal portalPrefab;
    public Dynamite dynamitePrefab;
    public Dynamite bombSlot3;
    public Dynamite bombSlot4;
    public Dynamite bombSlot5;

    public float dynamiteForwardOffset = 1.0f;
    public float dynamiteUpOffset = 0.2f;
    public float portalOffset = 0.05f;

    public int dynamiteMaxCount = 30;
    public float dynamitePlaceCooldown = 0.8f;

    float _nextDynamiteTime = 0f;
    SpriteRenderer _sr;
    PlayerInventory _inv;

    void Awake()
    {
        _sr = GetComponentInChildren<SpriteRenderer>();
        _inv = GetComponent<PlayerInventory>();
    }

    public void UseCurrentGear(int slot)
    {
        Debug.Log("UseCurrentGear slot = " + slot);

        if (slot == 1) PlacePortal();
        else if (slot == 2) PlaceBomb(dynamitePrefab, bombName2);
        else if (slot == 3) PlaceBomb(bombSlot3, bombName3);
        else if (slot == 4) PlaceBomb(bombSlot4, bombName4);
        else if (slot == 5) PlaceBomb(bombSlot5, bombName5);
        else Debug.Log($"No gear assigned for slot {slot}");
    }

    public int GetBombCount(string bombName)
    {
        if (_inv == null || _inv.myItems == null) return 0;
        foreach (var item in _inv.myItems)
            if (item != null && item.itemName == bombName)
                return item.count;
        return 0;
    }

    public bool AddBomb(string bombName, int amount)
    {
        if (amount <= 0 || _inv == null || _inv.myItems == null) return false;

        foreach (var item in _inv.myItems)
        {
            if (item != null && item.itemName == bombName)
            {
                int before = item.count;
                item.count = Mathf.Clamp(item.count + amount, 0, dynamiteMaxCount);
                if (item.count != before) _inv.UpdateUI();
                return item.count > before;
            }
        }
        return false;
    }

    bool CanPlaceBomb(string bombName)
    {
        return GetBombCount(bombName) > 0 && Time.time >= _nextDynamiteTime;
    }

    bool TryConsumeBomb(string bombName, int amount = 1)
    {
        if (_inv == null || _inv.myItems == null) return false;

        foreach (var item in _inv.myItems)
        {
            if (item != null && item.itemName == bombName && item.count >= amount)
            {
                item.count -= amount;
                _inv.UpdateUI();
                return true;
            }
        }
        return false;
    }
    void PlaceBomb(Dynamite prefab, string bombName)
    {
        Debug.Log("Try place bomb: " + bombName);

        if (prefab == null)
        {
            Debug.Log("prefab is null: " + bombName);
            return;
        }

        int count = GetBombCount(bombName);
        Debug.Log("bomb count = " + count);

        if (!CanPlaceBomb(bombName))
        {
            Debug.Log("CanPlaceBomb false: " + bombName);
            return;
        }

        if (!TryConsumeBomb(bombName, 1))
        {
            Debug.Log("TryConsumeBomb false: " + bombName);
            return;
        }

        float facing = (_sr != null && _sr.flipX) ? -1f : 1f;
        Vector3 spawnPos =
            transform.position +
            Vector3.right * (facing * dynamiteForwardOffset) +
            Vector3.up * dynamiteUpOffset;

        Dynamite dyn = Instantiate(prefab, spawnPos, Quaternion.identity);

        Collider2D[] playerCols = GetComponentsInChildren<Collider2D>(true);
        dyn.InitIgnorePlayer(playerCols, dyn.noCollisionTime);

        _nextDynamiteTime = Time.time + dynamitePlaceCooldown;

        Debug.Log("Bomb placed: " + bombName);
    }

    void PlacePortal()
    {
        Debug.Log("Try place portal");

        if (portalPrefab == null)
        {
            Debug.Log("portalPrefab is null");
            return;
        }

        if (PortalSystem.Instance != null && PortalSystem.Instance.Count() >= 2)
        {
            Debug.Log("Already has 2 portals");
            return;
        }

        Instantiate(portalPrefab, transform.position + Vector3.up * portalOffset, Quaternion.identity);
        Debug.Log("Portal placed");
    }
}