using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Link (auto by PortalSystem)")]
    public Portal linkedPortal;

    [Header("Exit")]
    public Transform exitPoint;

    [Header("Teleport")]
    public float cooldown = 0.15f;
    public KeyCode interactKey = KeyCode.E;

    [Header("Animation")]
    public Sprite[] frames;
    public float frameRate = 12f;

    private SpriteRenderer _sr;
    private float _frameTimer;
    private int _currentFrame;

    private Collider2D _playerInside;  

    private void Start()
    {
        PortalSystem.Instance?.Register(this);

        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;

        _sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // ===== anim =====
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

        // ===== interact teleport =====
        if (_playerInside == null) return;
        if (!Input.GetKeyDown(interactKey)) return;
        if (linkedPortal == null || linkedPortal.exitPoint == null) return;

        var deb = _playerInside.GetComponent<Debouncing>();
        if (deb == null) deb = _playerInside.gameObject.AddComponent<Debouncing>();
        if (!deb.CanTeleport()) return;

        deb.Lock(cooldown);

   
        _playerInside.transform.position = linkedPortal.exitPoint.position;
    }

    private void OnDestroy()
    {
        if (PortalSystem.Instance != null)
            PortalSystem.Instance.Unregister(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside = other;
        Debug.Log($"[Portal] Player entered: {other.name}");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (_playerInside == other) _playerInside = null;
        Debug.Log($"[Portal] Player exited: {other.name}");
    }
}