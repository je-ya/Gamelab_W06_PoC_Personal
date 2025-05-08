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
        image = transform.parent.GetComponent<Image>();
    }


    public void OnButtonClick()
    {
        image.enabled = false;
        icon.color = Color.red;
    }


}
