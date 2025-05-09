using UnityEngine;
using UnityEngine.UI;

public class SearchButton : MonoBehaviour
{

    public GameObject internetWindow;
    SpriteRenderer icon;
    Image image;

    bool ButtonEnable = false;


    private void Start()
    {
        icon = FindAnyObjectByType<InternetIcon>().gameObject.GetComponent<SpriteRenderer>();
        image = FindAnyObjectByType<AIPanel>().gameObject.GetComponent<Image>();
    }


    public void OnButtonClick()
    {
        if (!ButtonEnable)
        {
            image.enabled = false;
            icon.color = Color.red;
            ButtonEnable = true;
        }
        else {
        
            image.enabled = true;
            icon.color = Color.white;
            ButtonEnable = false;
        }

    }


}
