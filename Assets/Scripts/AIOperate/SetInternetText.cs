using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;

public class SetInternetText : MonoBehaviour
{
    private List<string> fruits = new List<string> { "apple", "banana", "orange", "grape" };

    [SerializeField]
    List<TextMeshProUGUI> textMeshProUGUIs;

    string input;

    public bool InputText(string input)
    {
        if (fruits.Contains(input))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void GetText(string text)
    {
        input = text;
        CheckAndChangeText();
    }

    void CheckAndChangeText()
    {
        if (input == "apple")
        {
            textMeshProUGUIs[0].text = "사과";
            textMeshProUGUIs[1].text = "맛있다";
        }
        else if(input == "banana")
        {
            textMeshProUGUIs[0].text = "바나나";
            textMeshProUGUIs[1].text = "맛있다";
        }
        else if(input == "orange")
        {
            textMeshProUGUIs[0].text = "오렌지";
            textMeshProUGUIs[1].text = "맛있다";
        }
        else if (input == "grape")
        {
            textMeshProUGUIs[0].text = "포도";
            textMeshProUGUIs[1].text = "맛있다";
        }
        else if (input == "duck")
        {
            textMeshProUGUIs[0].text = "오리";
            textMeshProUGUIs[1].text = "귀엽다";
        }

    }

}
