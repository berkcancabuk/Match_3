using UnityEngine;

public class AddScore : MonoBehaviour
{
    private UIManager _uiManager;
        
    void DisplayScore(int index)
    {
        _uiManager.score+=index;
        _uiManager.scoreText.text = (_uiManager.score).ToString();
    }

    private void OnEnable()
    {
        _uiManager = UIManager.Instance;
        EventManager.OnAddScore += DisplayScore;
    }

    private void OnDestroy()
    {
        EventManager.OnAddScore -= DisplayScore;
    }
}