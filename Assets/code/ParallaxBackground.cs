using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform cameraTransform;   
    
    public Vector2 parallaxEffectMultiplier = new Vector2(0.3f, 1.0f); 

    private Vector3 lastCameraPos;

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        lastCameraPos = cameraTransform.position;
    }

    void Update()
    {
        Vector3 delta = cameraTransform.position - lastCameraPos;

        // ✅ 方向反转（由 -= 改为 +=）
        transform.position += new Vector3(
            delta.x * parallaxEffectMultiplier.x, 
            delta.y * parallaxEffectMultiplier.y, 
            0
        );

        lastCameraPos = cameraTransform.position;
    }
}