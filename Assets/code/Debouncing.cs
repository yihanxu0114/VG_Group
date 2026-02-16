using UnityEngine;

public class Debouncing : MonoBehaviour
{
    private float unlockTime;

    public bool CanTeleport()
    {
        return Time.time >= unlockTime;
    }

    public void Lock(float seconds)
    {
        unlockTime = Time.time + Mathf.Max(0f, seconds);
    }
}