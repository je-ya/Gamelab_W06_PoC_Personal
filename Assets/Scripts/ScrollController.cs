using UnityEngine;

public class ScrollController : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 100f; // 스크롤 속도
    [SerializeField] private float minY = -500f;       // 최소 Y 위치 (스크롤 하한)
    [SerializeField] private float maxY = 500f;        // 최대 Y 위치 (스크롤 상한)

    private void Update()
    {

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Raycast로 마우스 위치에 콜라이더가 있는지 확인
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == gameObject.transform.parent.gameObject)
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (scrollInput != 0)
            {
                // 부모 오브젝트의 Y 위치 이동
                Vector3 newPosition = transform.position + Vector3.up * scrollInput * scrollSpeed * Time.deltaTime;
                // Y 위치를 minY와 maxY 사이로 제한
                newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
                transform.position = newPosition;
            }
        }


        // 마우스 휠 입력 감지

    }


    
}