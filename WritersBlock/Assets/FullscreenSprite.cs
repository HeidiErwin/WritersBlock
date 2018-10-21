using UnityEngine;

public class FullscreenSprite : MonoBehaviour
{

    void Awake()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        float cameraHeight = Camera.main.orthographicSize * 2;
        Vector2 cameraSize = new Vector2(Camera.main.aspect * cameraHeight, cameraHeight);
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        Vector2 scale = transform.localScale;
        scale *= cameraSize.y / spriteSize.y;

        transform.position = new Vector2(0, 0); // Optional
        transform.localScale = scale;
    }
}