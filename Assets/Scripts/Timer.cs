using TMPro;
using UnityEngine;
public class Timer : MonoBehaviour
{

    private UIManager _uiManager;
    
    private void CheckTimer()
    {
        if (_uiManager.timeRemaining >= 0)
        {
            _uiManager.timeRemaining += Time.deltaTime;
            DisplayTime(_uiManager.timeRemaining);
        }
    }
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        _uiManager.timeText.text = $"{minutes:00}:{seconds:00}";
    }

    private void OnEnable()
    {
        _uiManager = UIManager.Instance;
        EventManager.OnTimeSet += CheckTimer;
    }

    private void OnDestroy()
    {
        EventManager.OnTimeSet -= CheckTimer;
    }
}
