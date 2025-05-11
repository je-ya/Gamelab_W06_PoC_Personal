using TMPro;
using UnityEngine;

[ExecuteAlways] // 에디터에서도 작동
public class ChatBGBox : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    RectTransform imageRectTransform;
    public Vector2 padding = new Vector2(20, 20); // 텍스트 외부 여백

    void Start()
    {
        imageRectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (textComponent != null && imageRectTransform != null)
        {
            // 텍스트의 실제 출력 크기 가져오기
            float width = textComponent.preferredWidth;
            float height = textComponent.preferredHeight;

            // 이미지 크기 조정 (텍스트 크기 + 여백)
            imageRectTransform.sizeDelta = new Vector2(width + padding.x, height + padding.y);
        }
    }
}

