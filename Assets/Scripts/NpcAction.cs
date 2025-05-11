using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class NpcAction : MonoBehaviour
{

    BehaviorState currentState;
    NpcBehavior behavior;

    void Start()
    {
        behavior = FindAnyObjectByType<NpcBehavior>();
    }

    /*
     제목 선정
    1. 제목 텍스트 박스 클릭
    2. 제목 텍스트 박스에 들어갈 내용 작성
	    2-1. 과제 자료 폴더 열기
	    2-2. 자료.txt 열기
    3. 마우스를 좌우로 왔다갔다 유지 (반복)
     */
    [Header("제목 선정")]
    public List<GameObject> setTitleTarget;
    


    public void HandleStateSetTitle()
    {
        ClickTitle();
    }

    void ClickTitle()
    {
        behavior.MoveTowardTarget(setTitleTarget[0]);
    }






    void NextState()
    {
        currentState = (BehaviorState)(((int)currentState + 1) % 3);
    }



    /*
    PPT 배경
    1. 2번 슬라이드 클릭
    2 삽입 클릭
    3. 사각형 도형 생성
    4. 도형 드래그로 이동
    5. AI에게 이미지 생성 요청 -> 커서 정지
    6. 생성된 이미지 전체 슬라이드에 적용 
     */
    [Header("PPT 배경")]
    public List<GameObject> setBGTarget;

    /*
    내용 작성
    1. 텍스트 박스 클릭
    2. 자료 요약 요청 -> 커서 정지
    3. 요약된 자료 복사 및 불여넣기
    4. 3번 슬라이드 선택
    5. 자료 복사 및 불여넣기
    끝!
    */
    [Header("내용 작성")]
    public List<GameObject> writeContentTarget;

}
