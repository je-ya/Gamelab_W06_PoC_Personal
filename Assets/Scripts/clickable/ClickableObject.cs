using UnityEngine;

public class ClickableObject : MonoBehaviour, IClickableObject
{
    public void OnNpcRightClick() 
    {
        Debug.Log($"{gameObject.name}이(가) 오른쪽 클릭됨!");
    }


    public void OnNpcLeftClick()
    {

    }


    public void OnNpcDoubleClick()
    {

    }

    public void OnNpcDrag()
    {

    }
}
