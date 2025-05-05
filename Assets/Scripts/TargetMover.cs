using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;


public class TargetMover : MonoBehaviour
{
    public Transform[] targets;  // 여러 타겟 등록용
    public Transform gameTray;
    [SerializeField]
    private int currentTargetIndex = 0;

    public Transform CurrentTarget =>
        (currentTargetIndex < targets.Length) ? targets[currentTargetIndex] : null;
    public float moveSpeed = 2f;
    public float allowedDistanceFromPath = 0.1f; // 경로에서 허용되는 거리
    public int counter = 0;

    private Camera mainCamera;
    private bool isDraggingTarget = false;
    private bool isDraggingSelf = false;
    private Vector3 dragOffset;

    private Vector3 startPosition; // 현재 경로의 시작점
    private bool wasDragging = false;

    private bool isWaiting = false;

    private float dragStartTime = 0f;

    public GameObject messagePanel;
    public TextMeshProUGUI messageText;
    private bool warningShown = false;   // 반복 방지용
    public GameObject[] Decks;

    public Image counterBarImage; // Inspector에서 연결

    bool cardSelected = false;

    public TextMeshProUGUI HandOpText;
    public TextMeshProUGUI DiscardOpText;
    int HandOp = 3;
    int DiscardOp = 2;
    public SplineContainer splineContainer;

    public float splineDuration = 2f;        // 이동 시간
    private float splineTimer = 0f;
    private bool followSpline = false;
    private bool followTarget = true;


    public GameObject cardGame;
    bool goTray = false;

    void Start()
    {
        mainCamera = Camera.main;
        startPosition = transform.position;


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
        if (isDraggingSelf)
        {
            float elapsed = Time.time - dragStartTime;
            if (elapsed >= 5f)
            {
                counter += 5;
                UpdateCounterBar();
                Debug.Log($"🕒 드래그 5초 경과! counter +5 → 현재: {counter}");
                dragStartTime = Time.time; // 5초 단위 반복을 위해 초기화
            }
        }

        HandleMouseInput2D();
        checkTarget();
        UpdateOp();
        if (!followTarget && goTray)
        {
            MoveTowardSystemTray();
        }

        if (followSpline)
        {
            splineTimer += Time.deltaTime;
            float t = Mathf.Clamp01(splineTimer / splineDuration);

            Vector3 splinePos = splineContainer.Spline.EvaluatePosition(t);
            transform.position = splinePos;

            if (t >= 1f)
            {
                followSpline = false;
                followTarget = true; // ✅ 타겟 추적 다시 켜기
                startPosition = transform.position;
                Debug.Log("🟢 스플라인 이동 완료 → 타겟 추적 다시 시작됨");
            }
        }


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

    void MoveTowardSystemTray()
    {
        transform.position = Vector3.MoveTowards(transform.position, gameTray.position, moveSpeed * Time.deltaTime);
        // 항상 타겟에 도달하면 판정 시도
        if (!isWaiting && Vector3.Distance(transform.position, gameTray.position) < 0.01f)
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
                dragOffset = transform.position - (Vector3)mousePos2D;
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (isDraggingTarget || isDraggingSelf)
            {
                Vector3 newPos = (Vector3)mousePos2D + dragOffset;

                if (isDraggingTarget)
                    CurrentTarget.position = new Vector3(newPos.x, newPos.y, CurrentTarget.position.z);
                else if (isDraggingSelf)
                    transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
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

    // 이동이 재개될 때 호출: 경로를 벗어났는지 체크
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

        // 선분 외부면 바로 경로 이탈
        if (proj < 0 || proj > lineLength)
        {
            counter += 5;
            UpdateCounterBar();
            Debug.Log($"[밖: 선분 바깥] counter = {counter}");
            startPosition = transform.position;
            return;
        }

        // 선분 내부면 수직 거리 계산
        Vector3 closestPoint = lineStart + lineDir * proj;
        float perpendicularDist = Vector3.Distance(closestPoint, current);

        if (perpendicularDist > allowedDistanceFromPath)
        {
            counter += 5;
            UpdateCounterBar();
            Debug.Log($"[밖: 거리 초과] counter = {counter}");
            startPosition = transform.position;
        }
    }


    // start → end 선분 상에서 point에 가장 가까운 점을 계산
    Vector3 ClosestPointOnLine(Vector3 start, Vector3 end, Vector3 point)
    {
        Vector3 lineDir = end - start;
        float lineLength = lineDir.magnitude;
        lineDir.Normalize();

        float projectedLength = Vector3.Dot(point - start, lineDir);
        projectedLength = Mathf.Clamp(projectedLength, 0f, lineLength);

        return start + lineDir * projectedLength;
    }

    void OnDrawGizmos()
    {
        if (CurrentTarget == null) return;

        Vector3 lineStart = startPosition;
        Vector3 lineEnd = CurrentTarget.position;

        Vector3 lineDir = (lineEnd - lineStart).normalized;
        Vector3 normal = new Vector3(-lineDir.y, lineDir.x, 0f); // 2D에서 선에 수직인 벡터

        float halfWidth = allowedDistanceFromPath;

        // 사각형 4점 계산
        Vector3 p1 = lineStart + normal * halfWidth;
        Vector3 p2 = lineStart - normal * halfWidth;
        Vector3 p3 = lineEnd - normal * halfWidth;
        Vector3 p4 = lineEnd + normal * halfWidth;

        // 경로 사각형 그리기
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(p1, p4); // 윗변
        Gizmos.DrawLine(p4, p3); // 오른쪽
        Gizmos.DrawLine(p3, p2); // 아랫변
        Gizmos.DrawLine(p2, p1); // 왼쪽

        // 중심선 표시
        Gizmos.color = Color.green;
        Gizmos.DrawLine(lineStart, lineEnd);

    }

    IEnumerator WaitAtTargetAndCheck()
    {
        isWaiting = true;
        Debug.Log("🟢 도착했습니다. 2초 대기 중...");

        yield return new WaitForSeconds(2f);

        CheckObjectBelow();

        // 다시 움직이지 않음, 대기만 종료
        isWaiting = false;
    }

    IEnumerator WaitSecond()
    {
        yield return new WaitForSeconds(1f);
        goTray = true;
    }

    IEnumerator WaitSecondforTarget()
    {
        yield return new WaitForSeconds(1f);
        followTarget = true;
    }


    void CheckObjectBelow()
    {
        Vector2 center = transform.position;
        Vector2 boxSize = new Vector2(0.2f, 0.2f); // 감지 범위 조절 가능

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, boxSize, 0f);



        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue; // 자기 자신은 무시
            if (hit.name == "cardSpades_8")
            {
                Debug.Log("카드 도착");
                hit.transform.position += new Vector3(0f, 0.3f, 0f);
                cardSelected = true;
                currentTargetIndex++;
                startPosition = transform.position;
                return;
            }
            else if (hit.name == "Play Hand_0" && cardSelected)
            {
                Debug.Log("헨드-1");
                HandOp--;
                targets[0].gameObject.SetActive(false);
                ShowMessage("잉?");
                currentTargetIndex++;
                StartSplineFollow();

            }
            else if (hit.name == "Discard_0" && cardSelected)
            {
                Debug.Log("버리기-1");


                currentTargetIndex++;
                DiscardOp--;
                targets[0].gameObject.SetActive(false);
            }
            else if (hit.name == "bar")
            {
                followTarget = false;
                cardGame.GetComponent<MinimizeToTrayEffect>().StartMinimizeEffect();
                StartCoroutine(WaitSecond());
                ShowMessage("어!!");
            }
            else if (hit.name == "System Tray icon")
            {
                cardGame.GetComponent<MinimizeToTrayEffect>().StartRestoreEffect();
                StartCoroutine(WaitSecondforTarget());
            }
            else Debug.Log("감지된 콜라이더 없음");

        }

    }

    void UpdateCounterBar()
    {
        float percent = Mathf.Clamp01(counter / 100f); // 0~1 사이로 변환
        if (counterBarImage != null)
            counterBarImage.fillAmount = percent;


        if (!warningShown && counter >= 20)
        {
            if (counter == 20)
                ShowMessage("뭐야??");

            else if (counter == 30)
                ShowMessage("아 진짜...");

            else if (counter == 80)
                ShowMessage("아!!!!!!!!!");
        }
    }

    void UpdateOp()
    {
        HandOpText.text = $"{HandOp}";
        DiscardOpText.text = $"{DiscardOp}";
    }

    private Coroutine hideMessageCoroutine;
    public void ShowMessage(string message, float duration = 3f)
    {
        if (messagePanel != null)
            messagePanel.SetActive(true);

        if (messageText != null)
            messageText.text = message;

        // 이전 코루틴이 있다면 중복 방지
        if (hideMessageCoroutine != null)
            StopCoroutine(hideMessageCoroutine);

        hideMessageCoroutine = StartCoroutine(HideMessageAfterDelay(duration));
    }

    IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (messagePanel != null)
            messagePanel.SetActive(false);

        hideMessageCoroutine = null;
    }



    float moveDuration = 0.5f;
    bool specialTriggered = false;

    void checkTarget()
    {
        if (!specialTriggered && currentTargetIndex == 2)
        {
            Vector3 targetPositionForSpecial = new Vector3(4.6f, 1.95f, 0);
            specialTriggered = true;
            Decks[0].SetActive(true);
            StartCoroutine(MoveObjectSmoothly(Decks[0].transform, targetPositionForSpecial, moveDuration));
        }
    }

    IEnumerator MoveObjectSmoothly(Transform obj, Vector3 target, float duration)
    {
        Vector3 start = obj.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            obj.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        obj.position = target; // 정확하게 고정
    }


    void StartSplineFollow()
    {
        if (splineContainer != null)
        {
            followSpline = true;
            splineTimer = 0f;

            followTarget = false; // ✅ 타겟 추적 일시 정지
        }
    }

    public void EnableFollowTarget(bool enabled)
    {
        followTarget = enabled;
    }

}
