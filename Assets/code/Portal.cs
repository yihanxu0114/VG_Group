using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal linkedPortal;
    public Transform exitPoint;
    public float cooldown = 0.15f;
    public KeyCode interactKey = KeyCode.Q;
    public Sprite[] frames;
    public float frameRate = 12f;

    private SpriteRenderer _sr;
    private float _frameTimer;
    private int _currentFrame;
    private GameObject _playerInside;

    private void Start()
    {
        PortalSystem.Instance?.Register(this);

        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;

        _sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (frames != null && frames.Length > 0 && _sr != null)
        {
            _frameTimer += Time.deltaTime;
            if (_frameTimer >= 1f / frameRate)
            {
                _frameTimer = 0f;
                _currentFrame = (_currentFrame + 1) % frames.Length;
                _sr.sprite = frames[_currentFrame];
            }
        }

        if (_playerInside == null)
        {
            if (Input.GetKeyDown(interactKey))
                Debug.Log("[Portal] Pressed key but _playerInside is null");
            return;
        }

        if (!Input.GetKeyDown(interactKey)) return;

        Debug.Log("[Portal] Interact key pressed");

        if (linkedPortal == null)
        {
            Debug.Log("[Portal] linkedPortal is null");
            return;
        }

        if (linkedPortal.exitPoint == null)
        {
            Debug.Log("[Portal] linkedPortal.exitPoint is null");
            return;
        }

        var deb = _playerInside.GetComponent<Debouncing>();
        if (deb == null) deb = _playerInside.AddComponent<Debouncing>();

        if (!deb.CanTeleport())
        {
            Debug.Log("[Portal] Debounce blocked teleport");
            return;
        }

        deb.Lock(cooldown);
        Debug.Log("[Portal] Teleporting to " + linkedPortal.exitPoint.position);

        _playerInside.transform.position = linkedPortal.exitPoint.position;
    }

    private void OnDestroy()
    {
        if (PortalSystem.Instance != null)
            PortalSystem.Instance.Unregister(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("[Portal] raw enter: " + other.name);

        PlayerInventory inv = other.GetComponentInParent<PlayerInventory>();
        if (inv == null)
        {
            Debug.Log("[Portal] no PlayerInventory in parent");
            return;
        }

        _playerInside = inv.gameObject;
        Debug.Log("[Portal] Player entered: " + _playerInside.name);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("[Portal] raw exit: " + other.name);

        PlayerInventory inv = other.GetComponentInParent<PlayerInventory>();
        if (inv == null) return;

        if (_playerInside == inv.gameObject) _playerInside = null;
        Debug.Log("[Portal] Player exited: " + inv.gameObject.name);
    }
}