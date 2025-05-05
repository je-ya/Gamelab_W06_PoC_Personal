using System;
using UnityEngine;

public class StressManager : MonoBehaviour
{
    public static StressManager Instance; // 싱글톤 패턴
    private int stressLevel = 0;

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
}