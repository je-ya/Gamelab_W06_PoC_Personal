using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;




public class NpcBehavior : MonoBehaviour
{

    [Header("NPC 행동 타겟")]
    public Transform[] targets;  // 여러 타겟 등록용
    public Transform CurrentTarget =>
        (currentTargetIndex < targets.Length) ? targets[currentTargetIndex] : null;

    [Header("지금 향하고 있는 타겟 번호")]
    [SerializeField]
    int currentTargetIndex = 0;
    
    float moveSpeed = 4f;
    float allowedDistanceFromPath = 0.1f; // 경로에서 허용되는 거리
    Vector3 startPosition; // 현재 경로의 시작점

    Camera mainCamera;


    bool wasDragging = false;
    bool isDraggingTarget = false;
    bool isDraggingSelf = false;
    Vector3 dragOffset;


    bool isWaiting = false;
    float dragStartTime = 0f;

    
    bool followTarget = true;

    NpcReaction reaction;

    bool trshcanflag =false;
    [Header("활성화 될 창")]
    public List<GameObject> objectList;
    Dictionary<string, GameObject> objectDict;
    public List<GameObject> trayList;
    


    [Header("흔들 설정")]
    [SerializeField] float shakeDuration = 5f;    // 흔드는 총 시간
    [SerializeField] float shakeAmplitude = 0.1f;  // 흔드는 진폭
    [SerializeField] float shakeFrequency = 20f;   // 흔드는 속도
    [SerializeField] float throwForce = 5f;    // 발사 힘
    float shakeStartTime;

    Rigidbody2D rb;


    [Header("드래그 앤 드롭 설정")]
    bool isDragging = false;
    bool endDrag;
    private LayerMask raycastLayerMask; // 인스펙터에서 설정할 레이어 마스크
    private int originalTargetLayer;
    private GameObject targetObject = null;





    void Start()
    {
        mainCamera = Camera.main;
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        reaction = FindAnyObjectByType<NpcReaction>();
    }

    void Update()
    {
        if (!isDraggingSelf)
        {
            // 드래그가 끝나고 처음으로 이동이 시작되는 타이밍
            if (wasDragging)
            {
                CheckIfOutsidePath();
                wasDragging = false;
            }

            MoveTowardTarget();
        }
        if(isDraggingSelf)
        {

            
            float elapsed = Time.time - dragStartTime;
            if (elapsed >= 3f)
            {
                HandleDragWithShake();

            }
        }


        if (isDragging && targetObject != null)
        {
            targetObject.transform.position = transform.position;

            if (endDrag == true)
            {
                targetObject.layer = originalTargetLayer;
                targetObject.transform.position = transform.position;
                isDragging = false;
                targetObject = null;
                Debug.Log("오브젝트가 다음 위치에 놓였습니다: " + transform.position);
            }
        }


        HandleMouseInput2D();
    }

    void MoveTowardTarget()
    {
        if (!followTarget || CurrentTarget == null || isWaiting) return;

        transform.position = Vector3.MoveTowards(transform.position, CurrentTarget.position, moveSpeed * Time.deltaTime);

        // 항상 타겟에 도달하면 판정 시도
        if (!isWaiting && Vector3.Distance(transform.position, CurrentTarget.position) < 0.01f)
        {
            if (isDragging) {
                NpcClickDrag();
            }
            else
            {
                StartCoroutine(WaitAtTargetAndCheck());
            }
        }
    }


    void HandleMouseInput2D()
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null && hit.transform == transform)
            {
                isDraggingSelf = true;
                dragStartTime = Time.time; // 드래그 시작 시간 기록
                shakeStartTime = Time.time;
                dragOffset = transform.position - (Vector3)mousePos2D;
            }
        }



        if (Input.GetMouseButtonUp(0))
        {
            if (isDraggingSelf || isDraggingTarget)
            {
                wasDragging = true; // 드래그가 끝났음을 표시
            }

            isDraggingTarget = false;
            isDraggingSelf = false;
        }
    }
    

    // 드래그가 멈추고 타겟을 향한 이동이 재개될 때 호출: 사전에 정한 경로를 벗어났는지 확인
    void CheckIfOutsidePath()
    {
        if (CurrentTarget == null) return;

        Vector3 lineStart = startPosition;
        Vector3 lineEnd = CurrentTarget.position;
        Vector3 current = transform.position;

        Vector3 lineDir = (lineEnd - lineStart).normalized;
        Vector3 toCurrent = current - lineStart;

        // 선분의 길이
        float lineLength = Vector3.Distance(lineStart, lineEnd);

        // 선을 따라 projection (0 ~ lineLength)
        float proj = Vector3.Dot(toCurrent, lineDir);

        if (proj < 0 || proj > lineLength)
        {
            startPosition = transform.position;
            if (Random.value < 0.1f)
            {
                reaction.ShowMessage("이거 왜 이래?");
                StressManager.Instance.IncreaseStress(10);
            }
            else reaction.ShowMessage("뒤로 간거 같은데..");
            //선에서 수직 거리가 계산 가능한 위치에 있음
            return;
        }

        Vector3 closestPoint = lineStart + lineDir * proj;
        float perpendicularDist = Vector3.Distance(closestPoint, current);

        if (perpendicularDist > allowedDistanceFromPath)
        {
            startPosition = transform.position;
            //선에서 수직 거리가 계산 불가능 한 위치에 있음
            
            if (Random.value < 0.3f)
            {

                reaction.ShowMessage("마우스가 흔들린 것 같은데?");
                StressManager.Instance.IncreaseStress(10);

            }
        }
    }



    //N초 대기 후 NPC가 화면 마우스 클릭함
    IEnumerator WaitAtTargetAndCheck()
    {
        isWaiting = true;
        Debug.Log("타겟 도착 2초 대기");

        yield return new WaitForSeconds(1f);

        NpcLeftClick();
        isWaiting = false; //대기 종료
    }



    void NpcLeftClick()
    {
        //클릭 소리 재생
        Vector2 center = transform.position;
        Vector2 boxSize = new Vector2(0.2f, 0.2f); // 감지 범위 조절 가능

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, boxSize, 0f);

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue; // 자기 자신은 무시
            if (hit.name == "DeleteTargetFolder")
            {
                Debug.Log("삭제할 오브젝트 클릭 성공");
                DragObject(hit.gameObject);
                currentTargetIndex++;
                startPosition = transform.position;
                return;
            }
            else if (hit.name == "Trashcan")
            {
                Debug.Log("쓰래기통 폴더 클릭");
                objectList[0].SetActive(true);
                SetActiveObjectText(hit.name);
                if (trshcanflag)
                {
                    currentTargetIndex++;
                }
                startPosition = transform.position;

                return;
            }
            else if(hit.name == "ClearButton")
            {
                Debug.Log("비우기 버튼 클릭됨");
                objectList[2].SetActive(true);
                if (trshcanflag)
                {
                    currentTargetIndex++;
                }
                startPosition = transform.position;
                return;
            }
            else if (hit.name == "Folder")
            {
                Debug.Log("내PC 폴더 클릭됨");
                objectList[3].SetActive(true);
                SetActiveObjectText(hit.name);
                startPosition = transform.position;
                return;
            }
            else if (hit.name == "Messenger")
            {
                Debug.Log("메신저 클릭됨");
                objectList[6].SetActive(true);
                SetActiveObjectText(hit.name);
                startPosition = transform.position;
                return;
            }
            else if (hit.name == "Game")
            {
                Debug.Log("게임 클릭됨");
                objectList[7].SetActive(true);
                SetActiveObjectText(hit.name);
                startPosition = transform.position;
                return;
            }
            else if (hit.name == "Internet")
            {
                Debug.Log("인터넷 클릭됨");
                objectList[5].SetActive(true);
                SetActiveObjectText(hit.name);
                startPosition = transform.position;
                return;
            }
            else if (hit.name == "DeleteTargetFolder")
            {
                Debug.Log("타겟 폴더 클릭됨");
                objectList[4].SetActive(true);
                SetActiveObjectText(hit.name);
                startPosition = transform.position;
                return;
            }
            if (hit.name == "Minimize")
            {
                GameObject obj = hit.gameObject;
                Debug.Log("창 최소화");
                //부모 오브젝트만 비활성화
                if (obj.transform.parent != null)
                {
                    obj.transform.parent.gameObject.SetActive(false);
                }
                startPosition = transform.position;
                return;
            }
            if (hit.name == "SizeAdj")
            {
                Debug.Log("전체화면, 창모드 전환");
                //이거 어케만들지
                startPosition = transform.position;
                return;
            }
            if (hit.name == "Exit")
            {
                GameObject obj = hit.gameObject;
                Debug.Log("창 종료");
                //부모 오브젝트 비활성화
                if (obj.transform.parent != null)
                {
                    obj.transform.parent.gameObject.SetActive(false);
                }
                //트레이도 비활성화
                if (obj.transform.parent.name == "TrashCanWindow")
                {
                    GameObject trayObj;
                    trayObj = FindAnyObjectByType<SystemTrayIcon>().gameObject;
                    trayObj.transform.GetChild(0).gameObject.SetActive(false);
                }
                startPosition = transform.position;
                return;
            }
            if (hit.name == "Yes")
            {
                GameObject obj = hit.gameObject;
                Debug.Log("휴지통 비운다!");
                //부모 오브젝트 비활성화
                if (obj.transform.parent != null)
                {
                    obj.transform.parent.gameObject.SetActive(false);
                }
                //DeleteTargetFolder 비활성화
                GameObject windowObj;
                windowObj = FindAnyObjectByType<TrashCanWindow>().gameObject;
                windowObj.transform.GetChild(0).gameObject.SetActive(false);
                startPosition = transform.position;
                return;
            }
            if (hit.name == "No")
            {
                GameObject obj = hit.gameObject;
                Debug.Log("휴지통 비우기 싫어!!");
                //부모 오브젝트 비활성화
                if (obj.transform.parent != null)
                {
                    obj.transform.parent.gameObject.SetActive(false);
                }
                startPosition = transform.position;
                return;
            }
            else { 
                Debug.Log("감지된 콜라이더 없음");
            }

        }
    }


    void NpcClickDrag()
    {
        Vector2 center = transform.position;
        Vector2 boxSize = new Vector2(0.2f, 0.2f); // 감지 범위 조절 가능

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, boxSize, 0f);

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue; // 자기 자신은 무시
            if (hit.name == "Trashcan")
            {
                Debug.Log("쓰래기통으로 드래그");
                    if (targetObject.name == "DeleteTargetFolder")
                    {
                        Debug.Log("삭제할 타겟이다!");
                        targetObject.SetActive(false);
                        endDrag = true;
                        trshcanflag =true;

                        currentTargetIndex++;
                    }
                    else
                    {
                        Debug.Log("다른 타겟이지만..");
                        targetObject.SetActive(false);

                    }
                startPosition = transform.position;
                return;
            }
            if (hit.name == "DeleteTargetFolder")
            {
                Debug.Log("직박구리 폴더로 드래그");

                targetObject.SetActive(false);

                startPosition = transform.position;
                return;
            }
            else
            {
                Debug.Log("감지된 콜라이더 없음");
                currentTargetIndex++;
            }

        }
    }


    void DragObject(GameObject obj)
    {
        if (obj == null) return;

        originalTargetLayer = obj.layer;
        obj.layer = LayerMask.NameToLayer("Ignore Raycast");

        targetObject = obj;
        isDragging = true;
        endDrag = false; // 드래그가 계속되도록 초기 설정

    }


    void SetActiveObjectText(string name)
    {
        // trayList가 비어 있거나 유효하지 않은 경우
        if (trayList == null || trayList.Count == 0)
        {
            Debug.LogWarning("trayList가 비어 있거나 유효하지 않습니다.");
            return;
        }

        // 비활성화된 첫 번째 오브젝트 찾기
        foreach (GameObject obj in trayList)
        {
            if (obj != null && !obj.activeSelf)
            {
                // 오브젝트 활성화
                obj.SetActive(true);

                // TMP_Text 컴포넌트 찾기
                TMP_Text tmpText = obj.GetComponentInChildren<TMP_Text>();
                if (tmpText != null)
                {
                    tmpText.text = name;
                }
                else
                {
                    Debug.LogWarning("TMP_Text 컴포넌트를 찾을 수 없습니다.");
                }
                return; // 처리 후 종료
            }
        }

        // 모든 오브젝트가 활성화된 경우
        Debug.LogWarning("비활성화된 오브젝트가 없습니다. 모든 오브젝트가 이미 활성화되어 있습니다.");
    }




    void HandleDragWithShake()
    {
        // 1) 베이스 위치: 마우스 + offset
        var mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 basePos = new Vector3(mouseWorld.x, mouseWorld.y, transform.position.z) + dragOffset;

        // 2) 경과 시간에 따른 흔들 오프셋
        float elapsed = Time.time - shakeStartTime;
        if (elapsed < shakeDuration)
        {
            float xOffset = Mathf.Sin(elapsed * shakeFrequency) * shakeAmplitude;
            transform.position = basePos + Vector3.right * xOffset;
        }
        else
        {
            wasDragging = true;
            isDraggingTarget = false;
            isDraggingSelf = false;
            ThrowAway();
        }
    }

    void ThrowAway()
    {
        // 1) DraggableObject 스크립트가 붙어 있으면, 강제 해제 및 비활성화
        var draggable = GetComponent<DraggableObject>();
        if (draggable != null)
        {
            draggable.CancelDrag();
        }


    }


}

