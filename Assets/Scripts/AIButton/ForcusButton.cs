using UnityEngine;
using UnityEngine.UI;

public class ForcusButton : MonoBehaviour
{
    public Canvas focusCanvas;
    bool ButtonEnable = false;


    public void OnButtonClick()
    {
        if (!ButtonEnable)
        {
            focusCanvas.enabled = true;
            ButtonEnable = true;
        }
        else { focusCanvas.enabled = false;
            ButtonEnable = false;
        }
    }
}
