using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class ApplyButton : MonoBehaviour
{
    DrawLine drawLine;

    public List<GameObject> list;


    Button targetButton;
    enum ButtonState 
    { 
        None,
        ChangePPTCanvasBG,
        ApplyText
    }
    ButtonState currentState = ButtonState.None;

    private void Start()
    {
        targetButton = GetComponent<Button>();

        targetButton.onClick.AddListener(OnButtonClick);
        currentState = ButtonState.None;

        drawLine = FindAnyObjectByType<DrawLine>();

    }

    public void SetStateChangeBG()
    {
        currentState = ButtonState.ChangePPTCanvasBG;
        Debug.Log("Button set to State A");
    }

    // 상태 B로 설정
    public void SetStateApplyText()
    {
        currentState = ButtonState.ApplyText;
        Debug.Log("Button set to State B");
    }


    public void SetStateNone()
    {
        currentState = ButtonState.None;
        Debug.Log("Button set to State None");
    }


    private void OnButtonClick()
    {
        {
            switch (currentState)
            {
                case ButtonState.ChangePPTCanvasBG:
                    foreach (GameObject obj in list)
                    {
                        drawLine.ApplyCapturedImageToSprite(obj);
                    }
                    break;
                case ButtonState.ApplyText:
                    TextBoxSelector textBoxSelector = FindAnyObjectByType<TextBoxSelector>();
                    textBoxSelector.ChangeSelectedText();
                    break;
                default:
                    Debug.Log("No state set");
                    break;
            }
        }
    }    

}
