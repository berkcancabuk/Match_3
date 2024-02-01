using TMPro;
using UnityEngine;
public class Timer : MonoBehaviour
{
    private float _timeRemaining = 0;
    [SerializeField]private TextMeshProUGUI timeText;
    
    private void CheckTimer()
    {
        if (_timeRemaining >= 0)
        {
            _timeRemaining += Time.deltaTime;
            DisplayTime(_timeRemaining);
        }
    }
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = $"{minutes:00}:{seconds:00}";
    }

    private void OnEnable()
    {
        EventManager.OnTimeSet += CheckTimer;
    }

    private void OnDestroy()
    {
        EventManager.OnTimeSet -= CheckTimer;
    }
}
