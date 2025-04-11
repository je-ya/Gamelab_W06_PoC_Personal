using UnityEngine;

public class MinimizeToTrayEffect : MonoBehaviour
{
    public float duration = 1.0f;

    private float timer = 0f;
    private bool isMinimizing = false;
    private bool isRestoring = false;

    private Vector3 startPos;
    private Vector3 trayPos;
    private Vector3 originalPos;

    private Vector3 startScale;
    private Vector3 trayScale = Vector3.zero;
    private Vector3 originalScale;



    void Start()
    {

        originalPos = transform.position;
        originalScale = transform.localScale;
    }

    public void StartMinimizeEffect()
    {
        trayPos = Camera.main.ScreenToWorldPoint(new Vector3(-6.8f, -4.7f, 0));

        startPos = transform.position;
        startScale = transform.localScale;
        timer = 0f;

        isMinimizing = true;
        gameObject.SetActive(true);
    }

    public void StartRestoreEffect()
    {
        transform.position = trayPos;
        transform.localScale = trayScale;


        timer = 0f;
        isRestoring = true;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (isMinimizing)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);

            transform.position = Vector3.Lerp(startPos, trayPos, t);
            transform.localScale = Vector3.Lerp(startScale, trayScale, t);


            if (t >= 1f)
            {
                isMinimizing = false;
                gameObject.SetActive(false);
            }
        }

        if (isRestoring)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);

            transform.position = Vector3.Lerp(trayPos, originalPos, t);
            transform.localScale = Vector3.Lerp(trayScale, originalScale, t);


            if (t >= 1f)
            {
                isRestoring = false;
            }
        }
    }
}
