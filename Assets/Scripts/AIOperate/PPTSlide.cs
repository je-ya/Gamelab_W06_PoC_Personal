using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PPTSlide : MonoBehaviour
{
    public List<GameObject> canvasList;

    GameObject pptCanvas;

    string objectName;

    void Start()
    {
        objectName = gameObject.name;
        //pptCanvas = FindAnyObjectByType<PPTCanvas>().gameObject;
        //if (pptCanvas != null)
        //{
        //    foreach (Transform child in pptCanvas.transform)
        //    {
        //        canvasList.Add(child.gameObject);
        //    }
        //}
    }

    void OnMouseDown()
    {
        // 클릭된 오브젝트가 InGame 레이어인지 확인
        if (gameObject.layer == LayerMask.NameToLayer("InGame"))
        {

            foreach (GameObject canvas in canvasList)
            {
                if (canvas != null)
                {
                    canvas.SetActive(canvas.name == objectName);
                }
            }
        }
    }
}
