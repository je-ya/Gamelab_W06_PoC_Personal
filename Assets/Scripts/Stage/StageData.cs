using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StageData", menuName = "Stage/StageData")]
public class StageData : ScriptableObject
{
    public string stageName;
    public List<StateConfig> states; // 스테이지의 상태 리스트
}

[System.Serializable]
public class StateConfig
{
    public string stateName; // 행동명
    public GameObject targetObject; // 목표 오브젝트 (타겟)
    public ReachBehaviorType stateType; // 타겟 도달 시 동작 유형
}

public enum ReachBehaviorType
{
    RightClick,        // 우클릭
    LeftClick,      // 좌클릭
    DoubleClick,  // 더블 클릭
    DragToTarget, // 드래그(클릭 유지)
    Custom        // 커스텀 상태(이후에 추가할 키보드 입력 등)
}