using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("set the boundaries of the camera movement")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    private float camHalfWidth;
    private float camHalfHeight;

    void Start()
    {
        Camera cam = Camera.main;
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;
    }

    void LateUpdate()
    {
        float clampedX = Mathf.Clamp(transform.parent.position.x, minX + camHalfWidth,  maxX - camHalfWidth);
        float clampedY = Mathf.Clamp(transform.parent.position.y, minY + camHalfHeight, maxY - camHalfHeight);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}