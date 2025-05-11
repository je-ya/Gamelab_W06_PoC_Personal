using UnityEngine;

public class DangerButton : MonoBehaviour
{

    Canvas aiCanvas;
    public GameObject prefab; // 생성할 오브젝트 프리팹
    private GameObject spawnedObject; // 생성된 오브젝트
    private bool isFollowingMouse = false; // 마우스 따라다니는지 여부
    private Camera mainCamera;

    void Start()
    {
        aiCanvas = FindAnyObjectByType<AI>().gameObject.GetComponent<Canvas>();
        mainCamera = Camera.main;
    }


    public void OnButtonClick()
    {
        aiCanvas.enabled = false;
        if (spawnedObject == null) // 오브젝트가 아직 생성되지 않았다면
        {
            // 마우스 위치에서 오브젝트 생성
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0; // 2D라면 Z축을 0으로 설정
            spawnedObject = Instantiate(prefab, mousePos, Quaternion.identity);
            isFollowingMouse = true; // 마우스 따라다니기 시작
        }
    }


    void Update()
    {
        if (isFollowingMouse && spawnedObject != null)
        {
            // 마우스 위치로 오브젝트 이동
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0; // 2D 기준
            spawnedObject.transform.position = mousePos;

            // 마우스 클릭 시 오브젝트 고정
            if (Input.GetMouseButtonDown(0)) // 좌클릭
            {
                isFollowingMouse = false; // 마우스 따라다니기 중지
            }
        }
    }


}
