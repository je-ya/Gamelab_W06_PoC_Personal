using UnityEngine;

public class InternetIcon : MonoBehaviour
{
    SpriteRenderer sprite;
    public void spriteColorChange()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = Color.white;
    }
}
