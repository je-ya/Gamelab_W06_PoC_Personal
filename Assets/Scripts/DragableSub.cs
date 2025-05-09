using UnityEngine;

public class DraggableSub : MonoBehaviour
{
    private bool _isDragging = false;


    public bool isDragging
    {
        get { return _isDragging; }
        set { _isDragging = value; }
    }

    private Vector3 offset;


    [SerializeField] private GameObject contextMenu; // 컨텍스트 메뉴 UI (Panel)
    private RectTransform contextMenuRect;

    private void Start()
    {
        contextMenuRect = contextMenu.GetComponent<RectTransform>();
    }


    void Update()
    {
        // 마우스 왼쪽 버튼을 눌렀을 때
        if (Input.GetMouseButtonDown(0))
        {
            // 2D Raycast를 위해 월드 좌표로 변환된 마우스 위치 사용
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // "Cursor" 레이어만 감지하도록 레이어 마스크 설정
            LayerMask cursorLayer = LayerMask.GetMask("Bar");
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, cursorLayer);

            if (hit.collider != null)
            {
                // 클릭한 오브젝트가 이 오브젝트인지 확인
                if (hit.transform == transform)
                {
                    Debug.Log("마우스가 이 오브젝트 위에 있음: " + gameObject.name);
                    _isDragging = true;
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
            _isDragging = false;
            Debug.Log("드래그 종료");
        }



        // 드래그 중일 때 오브젝트 위치 업데이트
        if (_isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            transform.position = mousePosition + offset;
            //Debug.Log("드래그 중 - 오브젝트 위치: " + transform.position);
        }
    }




    public void CancelDrag()
    {
        isDragging = false;
    }
}