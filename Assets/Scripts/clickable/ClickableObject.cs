using UnityEngine;

public class ClickableObject : MonoBehaviour, IClickableObject
{
    public void OnNPCRightClick() 
    {
        Debug.Log($"{gameObject.name}이(가) 오른쪽 클릭됨!");
    }
}
