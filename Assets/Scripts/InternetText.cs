using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InternetText : MonoBehaviour
{
    TextMeshProUGUI Mytext;
    string text;
    Button insertButton; // 클릭할 버튼
    public TextMeshProUGUI serchText;

    void Start()
    {
        Mytext = GetComponent<TextMeshProUGUI>();
        insertButton = GetComponent<Button>();
        text = Mytext.text;
        //serchText = FindAnyObjectByType<InternetSearchText>().gameObject.GetComponent<TextMeshProUGUI>();
        insertButton.onClick.AddListener(ButtonClick);
    }

    public void ButtonClick()
    {
        serchText.text += text;
    }
}
