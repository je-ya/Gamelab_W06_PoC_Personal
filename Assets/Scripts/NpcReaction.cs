using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NpcReaction : MonoBehaviour
{
    [SerializeField]
    int counter = 0;

    public Image counterBarImage; // Inspector에서 연결
    bool warningShown = false;   // 반복 방지용

    public Canvas messageCanvas;
    public TextMeshProUGUI messageText;

    private Coroutine hideMessageCoroutine;



    void Start()
    {
        StressManager.Instance.OnStressChanged += UpdateStress;
    }

    void Update()
    {
    }


    void UpdateStress(int stress)
    {
        Debug.Log($"스트레스 수치 업데이트 : {stress}");
        counter = stress;


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
                ShowMessage("진짜 오늘 왜이러지??");
        }
    }




    public void ShowMessage(string message, float duration = 3f)
    {
        if (messageCanvas != null)
            messageCanvas.enabled = true;

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

        if (messageCanvas != null)
            messageCanvas.enabled = false;

        hideMessageCoroutine = null;
    }
}
