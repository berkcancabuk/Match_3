using UnityEngine;

public class UIManager : MonoBehaviour
{
    private void Update()
    {
        EventManager.OnTimeSet?.Invoke();
    }
}