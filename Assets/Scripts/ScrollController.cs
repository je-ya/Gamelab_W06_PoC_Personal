using UnityEngine;

public class ScrollController : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 100f; // 스크롤 속도
    [SerializeField] private float minY = -500f;       // 최소 Y 위치 (스크롤 하한)
    [SerializeField] private float maxY = 500f;        // 최대 Y 위치 (스크롤 상한)

    private void Update()
    {
        // 마우스 휠 입력 감지
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
}