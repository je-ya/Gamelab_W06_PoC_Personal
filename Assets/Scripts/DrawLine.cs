using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class DrawLine : MonoBehaviour
{
    private LineRenderer currentLine;
    private List<LineRenderer> lines = new List<LineRenderer>();
    private Vector3 mousePos;
    private bool isDrawing = false;

    // RectTransform 경계 변수 (Vector2로 유지)
    private RectTransform drawArea;
    private Vector2 minBounds;
    private Vector2 maxBounds;
    private const float fixedZ = 10f; // Z축 고정값

    void Start()
    {
        // 이 스크립트가 붙은 오브젝트의 RectTransform 가져오기
        drawArea = GetComponent<RectTransform>();
        CalculateBounds(); // 경계 계산
        CreateNewLine();
    }

    // RectTransform의 경계 계산
    void CalculateBounds()
    {
        // RectTransform의 월드 좌표 기준 경계 계산
        Vector3[] corners = new Vector3[4];
        drawArea.GetWorldCorners(corners);

        // 월드 좌표 기준으로 최소/최대 경계 설정 (Vector2로 변환)
        minBounds = new Vector2(corners[0].x, corners[0].y); // 좌측 하단
        maxBounds = new Vector2(corners[2].x, corners[2].y); // 우측 상단
    }

    void CreateNewLine()
    {
        GameObject lineObj = new GameObject("Line");
        lineObj.transform.SetParent(transform);
        currentLine = lineObj.AddComponent<LineRenderer>();
        currentLine.startWidth = 0.1f;
        currentLine.endWidth = 0.1f;
        currentLine.material = new Material(Shader.Find("Sprites/Default"));
        currentLine.startColor = Color.black;
        currentLine.endColor = Color.black;
        currentLine.positionCount = 0;
        currentLine.sortingOrder = 12; // order in Layer를 12로 설정
        lines.Add(currentLine);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 원래 마우스 좌표로 경계 체크
            Vector3 originalMousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, fixedZ));
            if (IsWithinBounds(originalMousePos)) // 원래 좌표가 경계 내에 있을 때만 처리
            {
                mousePos = GetClampedMousePosition();
                isDrawing = true;
                CreateNewLine();
            }
            else
            {
                isDrawing = false; // 경계 밖에서는 그리기 시작하지 않음
            }
        }
        if (Input.GetMouseButton(0) && isDrawing)
        {
            mousePos = GetClampedMousePosition();
            if (IsWithinBounds(mousePos)) // 경계 내에서만 점 추가
            {
                if (currentLine.positionCount == 0 || Vector3.Distance(mousePos, currentLine.GetPosition(currentLine.positionCount - 1)) > 0.1f)
                {
                    currentLine.positionCount++;
                    currentLine.SetPosition(currentLine.positionCount - 1, mousePos);
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
        }

        // 키보드 입력으로 기능 테스트
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearAllLines();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            RemoveLastLine();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            CaptureToImage();
        }
    }

    // 마우스 좌표를 경계 내로 제한
    Vector3 GetClampedMousePosition()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, fixedZ));
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
        pos.z = fixedZ; // Z축 고정
        return pos;
    }

    // 좌표가 경계 내에 있는지 확인 (Z축 제외)
    bool IsWithinBounds(Vector3 pos)
    {
        return pos.x >= minBounds.x && pos.x <= maxBounds.x &&
               pos.y >= minBounds.y && pos.y <= maxBounds.y;
    }

    public void ClearAllLines()
    {
        foreach (var line in lines)
        {
            Destroy(line.gameObject);
        }
        lines.Clear();
        CreateNewLine();
    }

    public void RemoveLastLine()
    {
        if (lines.Count > 0)
        {
            LineRenderer lastLine = lines[lines.Count - 1];
            lines.RemoveAt(lines.Count - 1);
            Destroy(lastLine.gameObject);
        }
        if (lines.Count == 0)
        {
            CreateNewLine();
        }
        else
        {
            currentLine = lines[lines.Count - 1];
        }
    }

    public void CaptureToImage()
    {
        // 캡처 영역 계산 (월드 좌표 -> 화면 좌표)
        Vector3 minScreenPoint = Camera.main.WorldToScreenPoint(new Vector3(minBounds.x, minBounds.y, fixedZ));
        Vector3 maxScreenPoint = Camera.main.WorldToScreenPoint(new Vector3(maxBounds.x, maxBounds.y, fixedZ));

        // 화면 좌표 기준으로 Rect 생성 (y축은 화면 좌표계에서 아래로 증가하므로 조정)
        int width = (int)(maxScreenPoint.x - minScreenPoint.x);
        int height = (int)(maxScreenPoint.y - minScreenPoint.y);
        int x = (int)minScreenPoint.x;
        int y = (int)(Screen.height - maxScreenPoint.y); // 화면 좌표계 상단 기준으로 변환

        // RenderTexture 크기를 전체 화면 크기로 설정
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        Camera.main.targetTexture = renderTexture;
        Camera.main.Render();

        RenderTexture.active = renderTexture;
        // Texture2D는 캡처 영역 크기로 설정
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
        // 캡처 영역만 읽기 (화면 좌표 기준)
        texture.ReadPixels(new Rect(x, y-170, width, height), 0, 0);
        texture.Apply();

        // PNG로 저장
        byte[] bytes = texture.EncodeToPNG();
        string path = Application.persistentDataPath + "/drawing.png";
        File.WriteAllBytes(path, bytes);
        Debug.Log("Image saved to: " + path);

        // 리소스 정리
        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);
        Destroy(texture);
    }

    public void DrawEnd()
    {
        ClearAllLines();
        transform.parent.GetComponent<Canvas>().enabled = false;
        this.enabled = false;
    }
}