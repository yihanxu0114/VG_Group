using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Sprite_left_right_shift : MonoBehaviour{
    public Sprite rightDavid;
    public Sprite leftDavid;

    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (rightDavid != null) sr.sprite = rightDavid; 
    }
    
    public void SetFacing(float x)
    {
        if (x > 0f && rightDavid != null) sr.sprite = rightDavid;
        else if (x < 0f && leftDavid != null) sr.sprite = leftDavid;
    }
}