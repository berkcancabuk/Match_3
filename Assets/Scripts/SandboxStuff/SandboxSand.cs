using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using Cysharp.Threading.Tasks;
using Random = UnityEngine.Random;
using Abstracts;
using Unity.VisualScripting;

public class SandboxSand : MonoBehaviour
{

    public Vector2 arrPos;

    private bool isFreefalling = true;
    private void Start()
    {
        StartCoroutine(nameof(SwapChecker));
    }

    // Test with coroutine and UniTask
    // normal async 
    IEnumerator SwapChecker()
    {
        bool random = false;
        bool end = false;
        WaitForSeconds time = new WaitForSeconds(0.2f);
        while (true)
        {
            random = Random.value > 0.5f ? true : false;

            if (SandboxBoard.Instance.CheckBelow(arrPos))
            {
                Debug.Log("Sand Falling Below");
                SandboxBoard.Instance.SwapBelow(arrPos);
                arrPos = new Vector2(arrPos.x, arrPos.y - 1);
                transform.position = new Vector3(transform.position.x, transform.position.y - 1);
                yield return time;
            }
            else
            {
                if (random)
                {
                    if (SandboxBoard.Instance.CheckLeft(arrPos))
                    {
                        arrPos = new Vector2(arrPos.x-1, arrPos.y - 1);
                        transform.position = new Vector2(transform.position.x-1, transform.position.y - 1);
                    }
                }
                else {
                    if (SandboxBoard.Instance.CheckRight(arrPos))
                    {
                        arrPos = new Vector2(arrPos.x+1, arrPos.y - 1);
                        transform.position = new Vector2(transform.position.x+1, transform.position.y - 1);
                    }
                }

                Debug.Log("Sand Not Falling Below");
            }

            yield return null;
        }

    }

}
