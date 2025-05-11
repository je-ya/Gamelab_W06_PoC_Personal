using System;
using UnityEngine;

public class StressManager : MonoBehaviour
{
    public static StressManager Instance; // 싱글톤 패턴
    private int stressLevel = 0;


    private float lastStressTime; // 마지막 스트레스 증가 시간
    private float stressDecreaseDelay = 5f; // 스트레스 감소 대기 시간 (초)
    private float stressDecreaseInterval = 1f; // 스트레스 감소 주기 (초)
    private int stressDecreaseAmount = 1; // 한 번에 감소하는 스트레스 양

    // 스트레스 변경 시 호출되는 Action
    public Action<int> OnStressChanged;

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // 마지막 스트레스 증가 후 충분한 시간이 지났는지 확인
        if (Time.time - lastStressTime >= stressDecreaseDelay)
        {
            // 주기적으로 스트레스 감소
            if (Time.time - lastStressTime >= stressDecreaseDelay + stressDecreaseInterval)
            {
                DecreaseStress();
                lastStressTime = Time.time - stressDecreaseDelay; // 다음 감소를 위해 타이머 조정
            }
        }
    }


    // 스트레스 값을 변경하는 메서드
    public void IncreaseStress(int amount)
    {
        stressLevel += amount;
        OnStressChanged?.Invoke(stressLevel); // 이벤트 호출
    }

    // 현재 스트레스 값을 가져오는 메서드
    public float GetStressLevel()
    {
        return stressLevel;
    }

    void DecreaseStress()
    {
        if (stressLevel > 0)
        {
            stressLevel = Mathf.Max(0, stressLevel - stressDecreaseAmount); // 0 이하로 떨어지지 않도록
            OnStressChanged?.Invoke(stressLevel); // 이벤트 호출
            Debug.Log($"스트레스가 {stressDecreaseAmount} 감소했습니다. 현재 스트레스: {stressLevel}");
        }
    }

}