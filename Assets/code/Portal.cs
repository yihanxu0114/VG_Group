using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Link (auto by PortalSystem)")]
    public Portal linkedPortal;

    [Header("Exit")]
    public Transform exitPoint;

    [Header("Teleport")]
    public float cooldown = 0.15f;

    [Header("Animation")]
    public Sprite[] frames;
    public float frameRate = 12f;

    private SpriteRenderer _sr;
    private float _frameTimer;
    private int _currentFrame;

    private void Start()
    {
        PortalSystem.Instance?.Register(this);

        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;

        _sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (frames == null || frames.Length == 0 || _sr == null) return;

        _frameTimer += Time.deltaTime;
        if (_frameTimer >= 1f / frameRate)
        {
            _frameTimer = 0f;
            _currentFrame = (_currentFrame + 1) % frames.Length;
            _sr.sprite = frames[_currentFrame];
        }
    }

    private void OnDestroy()
    {
        if (PortalSystem.Instance != null)
            PortalSystem.Instance.Unregister(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[Portal] ENTER by {other.name}, tag={other.tag}");
        if (!other.CompareTag("Player")) return;
        if (linkedPortal == null || linkedPortal.exitPoint == null) return;

        var deb = other.GetComponent<Debouncing>();
        if (deb == null) deb = other.gameObject.AddComponent<Debouncing>();

        if (!deb.CanTeleport()) return;

        deb.Lock(cooldown);
        other.transform.position = linkedPortal.exitPoint.position;
    }
}