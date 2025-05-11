using UnityEngine;

public class ButtonC : MonoBehaviour
{
    AI ai;


    public void OnButtonClick()
    {
        ai = FindAnyObjectByType<AI>();
        Canvas canvas;
        canvas =        ai.gameObject.GetComponent<Canvas>();
        if(canvas.enabled ==true)
        canvas.enabled = false;
    }
}
