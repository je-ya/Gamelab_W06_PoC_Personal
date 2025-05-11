using UnityEngine;

public class RainbowSprite : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private float speed = 1f; // 색상 변화 속도
    private bool colorChange = false;
    Color rainbowColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (colorChange)
        {
            // 시간에 따라 H값을 0~1 범위로 계산 (0~360도를 0~1로 정규화)
            float h = (Time.time * speed) % 1f;
            // HSV에서 RGB로 변환 (S=1, V=1로 고정하여 선명한 색상 유지)
            rainbowColor = Color.HSVToRGB(h, 1f, 1f);
            // 알파 값은 기존 스프라이트의 알파 값 유지
            rainbowColor.a = spriteRenderer.color.a;
            // 색상 적용
            spriteRenderer.color = rainbowColor;
        }
        else
        {
            rainbowColor = Color.white;
            spriteRenderer.color = rainbowColor;
        }
    }

    public void SetColorCjange()
    {
        colorChange = true;
    }
    public void EndColorChange()
    {
        colorChange = false;
    }
}