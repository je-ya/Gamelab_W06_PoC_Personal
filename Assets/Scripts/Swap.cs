using UnityEngine;

public class SwapTrigger2DOnClick : MonoBehaviour
{
    public Transform objectA;
    public Transform objectB;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 클릭
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.transform == transform)
            {
                SwapPositions();
            }
        }
    }

    void SwapPositions()
    {
        Vector3 temp = objectA.position;
        objectA.position = objectB.position;
        objectB.position = temp;
    }
}
