using UnityEngine;
using System.Collections;

public class NpcBehavior : MonoBehaviour
{
    public Transform[] targets;  // 여러 타겟 등록용
    public Transform CurrentTarget =>
        (currentTargetIndex < targets.Length) ? targets[currentTargetIndex] : null;

    [SerializeField]
    int currentTargetIndex = 0;
    
    float moveSpeed = 3f;
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


    [Header("흔들 설정")]
    [SerializeField] float shakeDuration = 5f;    // 흔드는 총 시간
    [SerializeField] float shakeAmplitude = 0.1f;  // 흔드는 진폭
    [SerializeField] float shakeFrequency = 20f;   // 흔드는 속도
    [SerializeField] float throwForce = 5f;    // 발사 힘

    Rigidbody2D rb;

    bool isDragging = false;
    float shakeStartTime;


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
                //Debug.Log($"드래그 5초 경과!");

                //if (Random.value < 0.3f)
                //{
                //    reaction.ShowMessage("뭐지??");
                //    StressManager.Instance.IncreaseStress(10);
                //}
                //else reaction.ShowMessage("마우스 고장났나?");


                //dragStartTime = Time.time; // 5초 단위 반복을 위해 초기화
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
            StartCoroutine(WaitAtTargetAndCheck());
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

        //if (Input.GetMouseButton(0))
        //{
        //    if (isDraggingTarget || isDraggingSelf)
        //    {
        //        Vector3 newPos = (Vector3)mousePos2D + dragOffset;

        //        if (isDraggingTarget)
        //            CurrentTarget.position = new Vector3(newPos.x, newPos.y, CurrentTarget.position.z);
        //        else if (isDraggingSelf)
        //            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
        //    }
        //}

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

        yield return new WaitForSeconds(2f);

        NPCClickRightMouse();
        isWaiting = false; //대기 종료
    }



    void NPCClickRightMouse()
    {
        //클릭 소리 재생
        Vector2 center = transform.position;
        Vector2 boxSize = new Vector2(0.2f, 0.2f); // 감지 범위 조절 가능

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, boxSize, 0f);

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue; // 자기 자신은 무시

            // IClickable 인터페이스를 가진 컴포넌트 찾기
            IClickableObject clickable = hit.gameObject.GetComponent<IClickableObject>();

            if (clickable != null)
            {
                clickable.OnNPCRightClick(); // 클릭 효과 실행
                //currentTargetIndex++;
            }
            else
            {
                Debug.Log("IClickable 컴포넌트가 없음: " + hit.gameObject.name);
            }
        }
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

        // 2) 물리 모드 전환 및 발사
        //rb.bodyType = RigidbodyType2D.Dynamic;
        Vector2 dir = (Vector2.up + Vector2.right * (Random.value < 0.5f ? -1 : 1)).normalized;
        rb.AddForce(dir * throwForce, ForceMode2D.Impulse);
    }


}

