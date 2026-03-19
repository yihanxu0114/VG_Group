using UnityEngine;

public class AimPivotController : MonoBehaviour
{
    public Camera targetCamera;

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    void Update()
    {
        if (targetCamera == null) return;

        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = targetCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        Vector3 dir = mouseWorldPos - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}