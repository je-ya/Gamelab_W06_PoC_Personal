using UnityEngine;

public class DrawButton : MonoBehaviour
{
    public Canvas drawCanvas;
    GameObject image;

    private void Start()
    {
        image = FindAnyObjectByType<DrawLine>().gameObject;
    }


    public void OnButtonClick()
    {
        drawCanvas.enabled = true;
        image.GetComponent<DrawLine>().enabled = true;
    }
}
