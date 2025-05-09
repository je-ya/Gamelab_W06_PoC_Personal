using UnityEngine;

public class DrawButton : MonoBehaviour
{
    public Canvas drawCanvas;
    GameObject image;

    bool ButtonEnable = false;

    private void Start()
    {
        image = FindAnyObjectByType<DrawLine>().gameObject;
    }


    public void OnButtonClick()
    {
        if (!ButtonEnable)
        {
            drawCanvas.enabled = true;
            image.GetComponent<DrawLine>().enabled = true;
            ButtonEnable = true;
        }
        else {

            drawCanvas.enabled = false;
            image.GetComponent<DrawLine>().enabled = false;
            ButtonEnable = false;
        }

    }
}
