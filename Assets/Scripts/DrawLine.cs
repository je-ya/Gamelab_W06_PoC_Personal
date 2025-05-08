using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class DrawLine : MonoBehaviour
{
    private LineRenderer currentLine;
    private List<LineRenderer> lines = new List<LineRenderer>();
    private Vector3 mousePos;
    private bool isDrawing = false;

    void Start()
    {
        CreateNewLine();
    }

    // 새로운 LineRenderer 생성
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
        lines.Add(currentLine);
    }

    void Update()
    {
        // 마우스 입력 처리
        if (Input.GetMouseButtonDown(0))
        {
            isDrawing = true;
            CreateNewLine(); // 새 선 시작
        }
        if (Input.GetMouseButton(0) && isDrawing)
        {
            mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
            // 점 간 간격 제한으로 성능 최적화
            if (currentLine.positionCount == 0 || Vector3.Distance(mousePos, currentLine.GetPosition(currentLine.positionCount - 1)) > 0.1f)
            {
                currentLine.positionCount++;
                currentLine.SetPosition(currentLine.positionCount - 1, mousePos);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
        }

        // 키보드 입력으로 기능 테스트 (UI 버튼으로 대체 가능)
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearAllLines(); // 모든 선 지우기
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            RemoveLastLine(); // 마지막 선 지우기
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            CaptureToImage(); // 이미지 저장
        }
    }

    // 모든 선 지우기
    public void ClearAllLines()
    {
        foreach (var line in lines)
        {
            Destroy(line.gameObject); // 각 LineRenderer 오브젝트 파괴
        }
        lines.Clear(); // 리스트 비우기
        CreateNewLine(); // 새 선 시작 준비
    }

    // 마지막 선 지우기
    public void RemoveLastLine()
    {
        if (lines.Count > 0)
        {
            LineRenderer lastLine = lines[lines.Count - 1];
            lines.RemoveAt(lines.Count - 1); // 리스트에서 제거
            Destroy(lastLine.gameObject); // 오브젝트 파괴
        }
        if (lines.Count == 0)
        {
            CreateNewLine(); // 리스트가 비면 새 선 준비
        }
        else
        {
            currentLine = lines[lines.Count - 1]; // 마지막 선을 현재 선으로 설정
        }
    }

    // 그린 그림을 이미지로 저장
    public void CaptureToImage()
    {
        RenderTexture renderTexture = new RenderTexture(512, 512, 24);
        Camera.main.targetTexture = renderTexture;
        Camera.main.Render();

        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(512, 512, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, 512, 512), 0, 0);
        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();
        string path = Application.persistentDataPath + "/drawing.png";
        File.WriteAllBytes(path, bytes);
        Debug.Log("Image saved to: " + path);

        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);
        Destroy(texture);
    }
}