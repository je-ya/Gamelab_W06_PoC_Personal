using UnityEngine;

public class CharacterClick : MonoBehaviour
{
    bool Loadcanvas = false;
    Canvas AIcanvs;

    void Start()
    {
        AIcanvs = FindAnyObjectByType<AI>().gameObject.GetComponent<Canvas>();
    }

    public void OnMouseDown()
    {

        if (!Loadcanvas)
        {

            Loadcanvas = true;
            AIcanvs.enabled = true;
        }
        else
        {
            Loadcanvas = false;
            AIcanvs.enabled = false;
        }
    }
}
