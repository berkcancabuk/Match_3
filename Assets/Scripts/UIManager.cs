using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [Header("Timer")]
    public float timeRemaining;
    public TextMeshProUGUI timeText;

    [Header("Score")] 
    public int score;
    public TextMeshProUGUI scoreText;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        EventManager.OnTimeSet?.Invoke();
    }

    public void AddScore()
    {
        
    }
}