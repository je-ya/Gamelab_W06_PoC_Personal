using System.IO;
using UnityEngine;

public class CaptureDrawing : MonoBehaviour
{
    public void CaptureToImage()
    {
        // RenderTexture 생성
        RenderTexture renderTexture = new RenderTexture(512, 512, 24);
        Camera.main.targetTexture = renderTexture;
        Camera.main.Render();

        // Texture2D로 변환
        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(512, 512, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, 512, 512), 0, 0);
        texture.Apply();

        // PNG로 저장
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(Application.persistentDataPath + "/drawing.png", bytes);
        Debug.Log("Image saved to: " + Application.persistentDataPath + "/drawing.png");

        // 정리
        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);
        Destroy(texture);
    }
}