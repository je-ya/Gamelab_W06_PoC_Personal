using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class InternetSearchText : MonoBehaviour
{
    TextMeshProUGUI text;
    string inputText;

    public GameObject SearchWindow;
    public List<GameObject> loadWindow;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        inputText = text.text;
    }

    public void ClearText()
    {
        text.text = "";
    }


    public void CheckTextAndLoadWindow()
    {
        inputText = text.text;
        if (inputText == "")
        {
            SearchWindow.SetActive(true);
            loadWindow[1].SetActive(false);
            loadWindow[0].SetActive(false);
        }
        else if (inputText == "apple")
        {
            loadWindow[1].SetActive(true);
            SearchWindow.SetActive(false);
        }
        else
        {
            loadWindow[0].SetActive(true);
            SearchWindow.SetActive(false);
        }


    }
}
