using UnityEngine;
using TMPro;

public class RainbowText : MonoBehaviour
{
    public TMP_Text textComponent; // TextMeshPro 텍스트 컴포넌트
    public float speed = 1.0f; // 색상이 변하는 속도

    void Start()
    {
        string originalText = textComponent.text;
        string coloredText = "";

        // 각 글자에 대해 무지개색 적용
        for (int i = 0; i < originalText.Length; i++)
        {
            // 시간과 위치에 따라 색상 계산 (HSV 색상 모델 사용)
            float hue = (Time.time * speed + i * 0.1f) % 1.0f; // 0~1 사이의 값
            Color color = Color.HSVToRGB(hue, 1.0f, 1.0f); // 채도와 명도는 1로 고정
            string hexColor = ColorUtility.ToHtmlStringRGB(color); // 색상을 HTML 형식으로 변환
            coloredText += $"<color=#{hexColor}>{originalText[i]}</color>";
        }

        textComponent.text = coloredText;
    }
}