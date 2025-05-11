using TMPro;
using UnityEngine;

public class CollectText : MonoBehaviour
{
    TextMeshProUGUI collectedText;
    ApplyButton button;

    string colleted;

    private void Start()
    {
        collectedText = GetComponent<TextMeshProUGUI>();
    }

    public void addTextCollection(string text)
    {
        collectedText.text += text;
        colleted = collectedText.text;
    }

    public void ClearText()
    {
        collectedText.text = "";
        colleted = "";
        button = FindAnyObjectByType<ApplyButton>();
        button.SetStateNone();

    }

    public string SetTextBox()
    {
        return colleted;
    }
}
