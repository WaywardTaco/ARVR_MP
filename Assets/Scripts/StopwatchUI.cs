using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StopwatchUI : MonoBehaviour
{
    public TextMeshProUGUI stopwatchText; 
    public Button toggleButton;         

    private float elapsedTime = 0f;
    private bool isRunning = false;

    void Start()
    {

        stopwatchText.text = FormatTime(elapsedTime);
        toggleButton.onClick.AddListener(ToggleStopwatch);
        toggleButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
    }

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            stopwatchText.text = FormatTime(elapsedTime);
        }
    }

    void ToggleStopwatch()
    {
        isRunning = !isRunning;
        toggleButton.GetComponentInChildren<TextMeshProUGUI>().text = isRunning ? "Stop" : "Start";


        if (!isRunning)
        {
            elapsedTime = 0f;
            stopwatchText.text = FormatTime(elapsedTime);
        }
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time * 1000) % 1000);
        return $"{minutes:00}:{seconds:00}:{milliseconds:000}";
    }

    public bool IsRunning()
    {
        return isRunning;
    }

}
