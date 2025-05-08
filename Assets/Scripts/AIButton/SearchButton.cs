using UnityEngine;
using UnityEngine.UI;

public class SearchButton : MonoBehaviour
{

    public GameObject internetWindow;
    SpriteRenderer icon;
    Image image;


    private void Start()
    {
        icon = FindAnyObjectByType<InternetIcon>().gameObject.GetComponent<SpriteRenderer>();
        image = FindAnyObjectByType<AIPanel>().gameObject.GetComponent<Image>();
    }


    public void OnButtonClick()
    {
        image.enabled = false;
        icon.color = Color.red;
    }


}
