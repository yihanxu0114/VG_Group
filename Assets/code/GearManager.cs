using UnityEngine;

public class GearManager : MonoBehaviour
{
    [Header("Bomb Names")]
    public string bombName2 = "Bomb";
    public string bombName3 = "BlueBomb";
    public string bombName4 = "RedBomb";
    public string bombName5 = "GrayBomb";

    [Header("Bullet Names")]
    public string bulletName6 = "Bullet6";
    public string bulletName7 = "Bullet7";
    public string bulletName8 = "Bullet8";

    [Header("Bomb / Portal Prefabs")]
    public Portal portalPrefab;
    public Dynamite dynamitePrefab;
    public Dynamite bombSlot3;
    public Dynamite bombSlot4;
    public Dynamite bombSlot5;

    [Header("Gun Bullet Prefabs")]
    public GameObject bulletSlot6;
    public GameObject bulletSlot7;
    public GameObject bulletSlot8;

    [Header("Aim / Shoot")]
    public Transform aimPivot;

    [Header("Gun Shoot Settings")]
    public float shootCooldown6 = 0.2f;
    public float shootCooldown7 = 0.15f;
    public float shootCooldown8 = 0.4f;

    [Header("Placement Settings")]
    public float dynamiteForwardOffset = 1.0f;
    public float dynamiteUpOffset = 0.2f;
    public float portalOffset = 0.05f;

    [Header("Bomb Settings")]
    public int dynamiteMaxCount = 30;
    public float dynamitePlaceCooldown = 0.8f;

    float _nextDynamiteTime = 0f;
    float _nextShoot6Time = 0f;
    float _nextShoot7Time = 0f;
    float _nextShoot8Time = 0f;

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
        else if (slot == 6) Shoot(bulletSlot6, bulletName6, ref _nextShoot6Time, shootCooldown6);
        else if (slot == 7) Shoot(bulletSlot7, bulletName7, ref _nextShoot7Time, shootCooldown7);
        else if (slot == 8) Shoot(bulletSlot8, bulletName8, ref _nextShoot8Time, shootCooldown8);
        else Debug.Log($"No gear assigned for slot {slot}");
    }
    void Shoot(GameObject bulletPrefab, string bulletName, ref float nextShootTime, float cooldown)
    {
        if (bulletPrefab == null)
        {
            Debug.Log("bulletPrefab is null");
            return;
        }

        if (aimPivot == null)
        {
            Debug.Log("aimPivot is null");
            return;
        }

        if (Time.time < nextShootTime)
        {
            return;
        }

        if (!TryConsumeItem(bulletName, 1))
        {
            Debug.Log("No ammo: " + bulletName);
            return;
        }

        GameObject newProjectile = Instantiate(bulletPrefab);
        newProjectile.transform.position = transform.position + Vector3.up * 0.5f;
        newProjectile.transform.rotation = aimPivot.rotation;

        Projectile projectile = newProjectile.GetComponent<Projectile>();
        if (projectile != null)
        {
            Collider2D[] playerCols = GetComponentsInChildren<Collider2D>(true);
            projectile.InitIgnorePlayer(playerCols, projectile.noCollisionTime);
        }

        nextShootTime = Time.time + cooldown;

        Debug.Log("Shot bullet: " + bulletPrefab.name);
    }

    public int GetItemCount(string itemName)
    {
        if (_inv == null || _inv.myItems == null) return 0;

        foreach (var item in _inv.myItems)
        {
            if (item != null && item.itemName == itemName)
                return item.count;
        }

        return 0;
    }

    bool TryConsumeItem(string itemName, int amount = 1)
    {
        if (_inv == null || _inv.myItems == null) return false;

        foreach (var item in _inv.myItems)
        {
            if (item != null && item.itemName == itemName && item.count >= amount)
            {
                item.count -= amount;
                _inv.UpdateUI();
                return true;
            }
        }

        return false;
    }

    public bool AddItem(string itemName, int amount)
    {
        if (amount <= 0 || _inv == null || _inv.myItems == null) return false;

        foreach (var item in _inv.myItems)
        {
            if (item != null && item.itemName == itemName)
            {
                item.count += amount;
                _inv.UpdateUI();
                return true;
            }
        }

        return false;
    }

    bool CanPlaceBomb(string bombName)
    {
        return GetItemCount(bombName) > 0 && Time.time >= _nextDynamiteTime;
    }

    void PlaceBomb(Dynamite prefab, string bombName)
    {
        Debug.Log("Try place bomb: " + bombName);

        if (prefab == null)
        {
            Debug.Log("prefab is null: " + bombName);
            return;
        }

        if (!CanPlaceBomb(bombName))
        {
            Debug.Log("CanPlaceBomb false: " + bombName);
            return;
        }

        if (!TryConsumeItem(bombName, 1))
        {
            Debug.Log("TryConsumeItem false: " + bombName);
            return;
        }

        float facing = 1f;

        if (aimPivot != null)
        {
            facing = aimPivot.right.x < 0 ? -1f : 1f;
        }
        else if (_sr != null && _sr.flipX)
        {
            facing = -1f;
        }

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