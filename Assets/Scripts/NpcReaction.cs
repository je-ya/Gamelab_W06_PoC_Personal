using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NpcReaction : MonoBehaviour
{

    public int counter = 0;
    public Image counterBarImage; // Inspector에서 연결
    private bool warningShown = false;   // 반복 방지용

    public GameObject messagePanel;
    public TextMeshProUGUI messageText;
    private Coroutine hideMessageCoroutine;




    void UpdateCounterBar()
    {
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
                ShowMessage("아!!!!!!!!!");
        }
    }


    public void ShowMessage(string message, float duration = 3f)
    {
        if (messagePanel != null)
            messagePanel.SetActive(true);

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

        if (messagePanel != null)
            messagePanel.SetActive(false);

        hideMessageCoroutine = null;
    }
}
