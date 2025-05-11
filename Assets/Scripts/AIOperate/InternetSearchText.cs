using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class InternetSearchText : MonoBehaviour
{
    TextMeshProUGUI text;
    string inputText;

    public GameObject SearchWindow;
    public List<GameObject> loadWindow;

    SetInternetText setText;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        setText = FindAnyObjectByType<SetInternetText>();

        inputText = text.text;
    }

    public void ClearText()
    {
        text.text = "";
    }


    public void CheckTextAndLoadWindow()
    {
        bool isContain;
        

        inputText = text.text;
        isContain = setText.InputText(inputText);

        if (inputText == "")
        {
            SearchWindow.SetActive(true);
            loadWindow[1].SetActive(false);
            loadWindow[0].SetActive(false);
        }
        else if (isContain)
        {
            loadWindow[1].SetActive(true);
            loadWindow[0].SetActive(false);
            SearchWindow.SetActive(false);
            setText.GetText(inputText);


        }
        else
        {
            loadWindow[0].SetActive(true);
            SearchWindow.SetActive(false);
            loadWindow[1].SetActive(false);
        }


    }
}
