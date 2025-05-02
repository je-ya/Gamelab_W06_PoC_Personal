using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class BehaviorPatternManager : MonoBehaviour
{
/*    [SerializeField] private StageData currentStage;
    private int currentTargetIndex = 0;
    private List<IState> states;
    private int currentStateIndex;
    private IState currentState;
    private InterruptionHandler interruptionHandler;

    public Vector2 MousePosition { get; set; }
    public GameObject Folder { get; private set; } // 직박구리 폴더
    public GameObject TargetObject { get; private set; } // 현재 목표 오브젝트
    public Vector2 InitialObjectPosition { get; private set; } // 오브젝트 초기 위치

    private void Awake()
    {
        interruptionHandler = gameObject.AddComponent<InterruptionHandler>();
        InitializeStates();
    }

    private void InitializeStates()
    {
        states = new List<IState>();
        foreach (var stateConfig in currentStage.states)
        {
            switch (stateConfig.stateType)
            {
                case StateType.MoveToTarget:
                    states.Add(new MoveToTargetState(this));
                    break;
                case StateType.Click:
                    states.Add(new ClickState(this));
                    break;
                case StateType.DoubleClick:
                    states.Add(new DoubleClickState(this));
                    break;
                case StateType.DragToTarget:
                    states.Add(new DragToTargetState(this));
                    break;
                case StateType.Custom:
                    states.Add(new CustomState(this));
                    break;
            }
            // 초기 오브젝트 설정
            if (stateConfig.stateType == StateType.MoveToTarget || stateConfig.stateType == StateType.Click)
            {
                Folder = stateConfig.targetObject;
                InitialObjectPosition = stateConfig.targetPosition;
            }
            else if (stateConfig.stateType == StateType.DragToTarget)
            {
                TargetObject = stateConfig.targetObject; // 예: 휴지통
            }
        }
        currentStateIndex = 0;
        currentState = states[currentStateIndex];
        currentState.Enter(currentStage.states[currentStateIndex]);
    }

    private void Update()
    {
        if (interruptionHandler.IsInterrupting)
        {
            return; // 방해 중에는 상태 실행 중지
        }

        currentState.Execute();
        if (currentState.CanTransition())
        {
            TransitionToNextState();
        }
    }

    private void TransitionToNextState()
    {
        currentState.Exit();
        currentStateIndex++;
        if (currentStateIndex >= states.Count)
        {
            currentTargetIndex++;
            Debug.Log($"Stage {currentStage.stageName} completed! CurrentTargetIndex: {currentTargetIndex}");
            return;
        }
        currentState = states[currentStateIndex];
        currentState.Enter(currentStage.states[currentStateIndex]);
    }

    public void HandleInterruption(InterruptionHandler.InterruptionType interruption)
    {
        // 방해 유형에 따라 반응
        switch (interruption)
        {
            case InterruptionHandler.InterruptionType.HoldStill:
                Debug.Log("Player is holding still. Waiting...");
                break;
            case InterruptionHandler.InterruptionType.MoveAndReleaseShort:
                Debug.Log("Player moved slightly and released. Resuming...");
                interruptionHandler.IsInterrupting = false;
                break;
            case InterruptionHandler.InterruptionType.MoveAndReleaseFar:
                Debug.Log("Player moved far. Adding recovery action...");
                // 예: 폴더로 다시 이동
                currentStateIndex = 0; // 첫 상태로 복귀
                currentState = states[currentStateIndex];
                currentState.Enter(currentStage.states[currentStateIndex]);
                interruptionHandler.IsInterrupting = false;
                break;
            case InterruptionHandler.InterruptionType.MoveAndHoldFolder:
                Debug.Log("Player holding at folder. Continuing...");
                interruptionHandler.IsInterrupting = false;
                break;
            case InterruptionHandler.InterruptionType.MoveAndHoldCenter:
                Debug.Log("Player holding at center. Prompting return...");
                interruptionHandler.IsInterrupting = false;
                break;
            case InterruptionHandler.InterruptionType.MoveAndHoldCorner:
                Debug.Log("Player holding at corner. Triggering recovery...");
                currentStateIndex = 0; // 첫 상태로 복귀
                currentState = states[currentStateIndex];
                currentState.Enter(currentStage.states[currentStateIndex]);
                interruptionHandler.IsInterrupting = false;
                break;
            case InterruptionHandler.InterruptionType.MoveContinuously:
                Debug.Log("Player moving continuously. Monitoring...");
                break;
        }

        // 인덱스 증가 조건
        if (CanIncrementIndex(interruption))
        {
            IncrementIndex();
        }
    }

    public bool CanIncrementIndex(InterruptionHandler.InterruptionType interruption)
    {
        // 예: 스테이지 완료 또는 특정 방해(예: 폴더에 정확히 놓기) 시 증가
        return currentStateIndex >= states.Count ||
               interruption == InterruptionHandler.InterruptionType.MoveAndHoldFolder;
    }

    public void IncrementIndex()
    {
        currentTargetIndex++;
        Debug.Log($"CurrentTargetIndex increased to: {currentTargetIndex}");
    }

    public int GetCurrentTargetIndex()
    {
        return currentTargetIndex;
    }*/
}