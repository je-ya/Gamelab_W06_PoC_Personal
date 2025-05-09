using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;




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

    bool trshcanFlag = false;
    bool openFlag = false;



    [Header("활성화 될 창")]
    public List<GameObject> objectList;
    Dictionary<string, GameObject> objectDict;
    public List<GameObject> trayList;

    [Header("창 활성화 이후")]
    Stack<GameObject> activeObjects = new Stack<GameObject>();
    bool isRemovingWindow = false;
    GameObject removeTarget;
    Transform exitButton; // Exit 자식 오브젝트

    [Header("흔들 설정")]
    [SerializeField] float shakeDuration = 5f;    // 흔드는 총 시간
    [SerializeField] float shakeAmplitude = 0.1f;  // 흔드는 진폭
    [SerializeField] float shakeFrequency = 20f;   // 흔드는 속도
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
                Wait5sec();
                CheckIfOutsidePath();
                wasDragging = false;
            }


            //다른 창 열리면 종료하려고 해야하고, 창이 최소화되거나 종료되면 다시 켜야함
            if (isRemovingWindow && removeTarget != null && exitButton != null)
            {
                MoveAIToExitButton();
            }
            else
            {
                MoveTowardTarget();
            }
        }
        if (isDraggingSelf)
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
                Debug.Log("오브젝트가 다음 위치에 놓였습니다: " + transform.position);
                targetObject = null;
            }
        }


        HandleMouseInput2D();
    }

    void MoveTowardTarget()
    {
        if (!followTarget || CurrentTarget == null || isWaiting) return;

        transform.position = Vector3.MoveTowards(transform.position, CurrentTarget.position, moveSpeed * Time.deltaTime);

        if (CurrentTarget != null && !CurrentTarget.gameObject.activeInHierarchy)
        {
            currentTargetIndex--;
            if(currentTargetIndex ==2)
            openFlag = true;
        }

        // 항상 타겟에 도달하면 판정 시도
/*        if (!isWaiting && Vector3.Distance(transform.position, CurrentTarget.position) < 0.01f)
        {

            WaitAtTargetAndCheckDrag();
            Debug.Log("판정합니다!");
            if (isDragging)
            {
                Debug.Log("드래그 해야함");
                NpcClickDrag();
            }
            else
            {
                if (openFlag)
                {
                    Debug.Log("더블클릭 해야함");
                    StartCoroutine(WaitAtTargetAndCheckDouble());
                }
                else
                {

                    Debug.Log("클릭 해야함");
                    StartCoroutine(WaitAtTargetAndCheck());
                }
            }
        }*/
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
            else { reaction.ShowMessage("뒤로 간거 같은데..");
                StressManager.Instance.IncreaseStress(1);
            }
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
            else StressManager.Instance.IncreaseStress(1);
        }
    }



    //N초 대기 후 NPC가 화면 마우스 클릭함
    IEnumerator WaitAtTargetAndCheck()
    {
        isWaiting = true;
        Debug.Log("타겟 도착 1초 대기");

        yield return new WaitForSeconds(1f);

        NpcLeftClick();
        isWaiting = false; //대기 종료
    }


    IEnumerator WaitAtTargetAndCheckDouble()
    {
        isWaiting = true;
        Debug.Log("타겟 도착 1초 대기");

        yield return new WaitForSeconds(1f);

        NpcDoubleClick();
        isWaiting = false; //대기 종료
    }

    IEnumerator WaitAtTargetAndCheckDrag()
    {
        yield return new WaitForSeconds(.5f);
    }

    IEnumerator Wait5sec()
    {
        yield return new WaitForSeconds(5f);
    }


    public void NpcLeftClick()
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
            else if (hit.name == "ClearButton")
            {
                Debug.Log("비우기 버튼 클릭됨");
                objectList[2].SetActive(true);
                if (trshcanFlag)
                {
                    currentTargetIndex++;
                }
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
            else
            {
                Debug.Log("감지된 콜라이더 없음");
            }

        }
    }


    public void NpcDoubleClick()
    {
        Debug.Log("더블클릭해야해!");
        //클릭 소리 재생
        Vector2 center = transform.position;
        Vector2 boxSize = new Vector2(0.2f, 0.2f); // 감지 범위 조절 가능

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, boxSize, 0f);

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue; // 자기 자신은 무시
            else if (hit.name == "Trashcan")
            {
                Debug.Log("쓰래기통 폴더 클릭");
                objectList[0].SetActive(true);
                SetActiveObjectText(hit.name);
                if (trshcanFlag)
                {
                    currentTargetIndex++;
                }
                openFlag = false;
                startPosition = transform.position;

                return;
            }
            else if (hit.name == "Folder")
            {
                Debug.Log("내PC 폴더 클릭됨");
                //ActivateObject(3, hit.name);
                objectList[1].SetActive(true);
                SetActiveObjectText(hit.name);
                startPosition = transform.position;
                return;
            }
            else if (hit.name == "Messenger")
            {
                Debug.Log("메신저 클릭됨");
                //ActivateObject(6, hit.name);
                objectList[4].SetActive(true);
                SetActiveObjectText(hit.name);
                startPosition = transform.position;
                return;
            }
            else if (hit.name == "Game")
            {
                Debug.Log("게임 클릭됨");
                //ActivateObject(7, hit.name);
                objectList[5].SetActive(true);
                SetActiveObjectText(hit.name);
                startPosition = transform.position;
                return;
            }
            else if (hit.name == "Internet")
            {
                Debug.Log("인터넷 클릭됨");
                //ActivateObject(5, hit.name);
                objectList[3].SetActive(true);
                SetActiveObjectText(hit.name);
                InternetIcon internet;
                internet = FindAnyObjectByType<InternetIcon>();
                internet.spriteColorChange();
                GameObject ai;
                ai = FindAnyObjectByType<AI>().gameObject;
                Canvas canvas = ai.GetComponent<Canvas>();
                Image panelImage;
                panelImage = FindAnyObjectByType<AIPanel>().gameObject.GetComponent<Image>();
                if(canvas.enabled == true)
                {
                    if(panelImage.enabled == false)
                    {
                        panelImage.enabled = true;
                    }    
                }
                startPosition = transform.position;
                return;
            }
            else if (hit.name == "DeleteTargetFolder")
            {
                Debug.Log("타겟 폴더 클릭됨");
                //ActivateObject(4, hit.name);
                objectList[2].SetActive(true);
                SetActiveObjectText(hit.name);
                startPosition = transform.position;
                return;
            }
            else
            {
                Debug.Log("감지된 콜라이더 없음");
            }

        }
    }


    private void ActivateObject(int index, string objectName)
    {
        GameObject obj = objectList[index];
        if (!obj.activeSelf)
        {
            obj.SetActive(true);
            activeObjects.Push(obj); // 스택에 추가
            SetActiveObjectText(objectName); // 기존 텍스트 설정 함수
        }

        // 이미 켜져 있으면 치우기 시작
        StartCleaning(obj);
    }

    private void StartCleaning(GameObject target)
    {
        if (!isRemovingWindow)
        {
            isRemovingWindow = true;
            removeTarget = target;
            // Exit 자식 오브젝트 찾기
            exitButton = removeTarget.transform.Find("Exit");
            if (exitButton == null)
            {
                Debug.LogError("Exit 자식 오브젝트를 찾을 수 없습니다!");
                isRemovingWindow = false;
                removeTarget = null;
            }
        }
    }


    private void MoveAIToExitButton()
    {
        Vector3 targetPosition = exitButton.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        // AI가 Exit 버튼에 충분히 가까워지면 NpcLeftClick 호출
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            WaitAtTargetAndCheckDrag();
            NpcLeftClick(); // 기존 함수 호출
            activeObjects.Pop(); // 스택에서 제거
            isRemovingWindow = false;
            removeTarget = null;
            exitButton = null;

            // 스택에 남은 오브젝트가 있으면 다음 오브젝트 치우기
            if (activeObjects.Count > 0)
            {
                StartCleaning(activeObjects.Peek());
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
                    openFlag = true;
                    trshcanFlag = true;

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

            else
            {
                Debug.Log("감지된 콜라이더 없음");
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
            elapsed += Time.deltaTime;
            
            float xOffset = Mathf.Sin(elapsed * shakeFrequency) * shakeAmplitude;
            transform.position = basePos + Vector3.right * xOffset;
            reaction.ShowMessage("이거 왜 이러지?");
            // 1초마다 이벤트 트리거
            int currentSecond = Mathf.FloorToInt(elapsed);
            int previousSecond = Mathf.FloorToInt(elapsed - Time.deltaTime);
            if (currentSecond > previousSecond && currentSecond <= shakeDuration)
            {
                StressManager.Instance.IncreaseStress(5);
            }
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
        //DraggableObject 스크립트가 붙어 있으면, 강제 해제 함수 호출
        var draggable = GetComponent<DraggableObject>();
        if (draggable != null)
        {
            draggable.CancelDrag();
        }

    }


    void Proposalclick()
    {
        
    }

}

