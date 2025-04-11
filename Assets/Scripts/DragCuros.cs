using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;

    void Update()
    {
        // 마우스 왼쪽 버튼을 눌렀을 때
        if (Input.GetMouseButtonDown(0))
        {
            // 2D Raycast를 위해 월드 좌표로 변환된 마우스 위치 사용
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log("Raycast가 충돌한 오브젝트: " + hit.collider.gameObject.name);

                // 클릭한 오브젝트가 이 오브젝트인지 확인
                if (hit.transform == transform)
                {
                    Debug.Log("마우스가 이 오브젝트 위에 있음: " + gameObject.name);
                    isDragging = true;
                    offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
                }
                else
                {
                    Debug.Log("다른 오브젝트를 클릭함: " + hit.transform.name);
                }
            }
            else
            {
                Debug.Log("Raycast가 아무것도 감지하지 못함");
            }
        }

        // 마우스 버튼을 놓으면 드래그 종료
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            Debug.Log("드래그 종료");
        }

        // 드래그 중일 때 오브젝트 위치 업데이트
        if (isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            transform.position = mousePosition + offset;
            Debug.Log("드래그 중 - 오브젝트 위치: " + transform.position);
        }
    }
}