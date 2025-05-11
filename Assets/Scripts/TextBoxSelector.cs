using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class TextBoxSelector : MonoBehaviour
{
    [SerializeField] private TMP_Text textBox1; // 첫 번째 텍스트 박스
    [SerializeField] private TMP_Text textBox2; // 두 번째 텍스트 박스
    private TMP_Text selectedTextBox; // 현재 선택된 텍스트 박스
    private Color defaultColor = Color.black; // 기본 색상
    private Color selectedColor = Color.red; // 선택된 색상

    CollectText collectText;

    void Start()
    {
        // 초기화: 두 텍스트 박스 모두 기본 색상으로 설정
        textBox1.color = defaultColor;
        textBox2.color = defaultColor;

        // 클릭 이벤트 추가 (EventTrigger 사용)
        AddClickListener(textBox1, () => SelectTextBox(textBox1));
        AddClickListener(textBox2, () => SelectTextBox(textBox2));

        collectText = FindAnyObjectByType<CollectText>();
    }

    // 텍스트 박스 선택
    private void SelectTextBox(TMP_Text textBox)
    {
        // 이전에 선택된 텍스트 박스가 있으면 기본 색상으로 되돌림
        if (selectedTextBox != null)
        {
            selectedTextBox.color = defaultColor;
        }

        // 새로운 텍스트 박스를 선택하고 빨간색으로 변경
        selectedTextBox = textBox;
        selectedTextBox.color = selectedColor;
    }

    // 선택된 텍스트 박스의 텍스트를 "abc"로 변경하는 함
    public void ChangeSelectedText()
    {
        string getText;
        getText = collectText.SetTextBox();
        selectedTextBox.text = getText;
    }

    // 클릭 이벤트 리스너 추가
    private void AddClickListener(TMP_Text textBox, UnityEngine.Events.UnityAction onClick)
    {
        EventTrigger trigger = textBox.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = textBox.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((data) => onClick.Invoke());
        trigger.triggers.Add(entry);
    }
}