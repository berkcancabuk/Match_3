

using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;

public class Candy : Tile
{


    Vector2 endPos;
    Vector2 startPos;
    bool draggingStarted = false;

    float swipeThreshold = 0.2f;

    Direction direction = Direction.None;

    Vector3 mousePosition;


    public CandyType GetCandyType()
    {
        return candyType;
    }

    private void OnMouseDown()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log("On begin Drag");
        draggingStarted = true;
        startPos = mousePosition;
    }
    private void OnMouseDrag()
    {
        if (draggingStarted)
        {
            endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 difference = endPos - startPos; // difference vector between start and end positions.

            if (difference.magnitude > swipeThreshold)
            {
                if (Mathf.Abs(difference.x) > Mathf.Abs(difference.y)) // Do horizontal swipe
                {
                    direction = difference.x > 0 ? Direction.Right : Direction.Left; // If greater than zero, then swipe to right.
                }
                else //Do vertical swipe
                {
                    direction = difference.y > 0 ? Direction.Up : Direction.Down; // If greater than zero, then swipe to up.
                }
            }
            else
            {
                direction = Direction.None;
            }

        }
    }

    private void OnMouseUp()
    {
        if (draggingStarted && direction != Direction.None)
        {
            //Do something with this direction data.
            Debug.Log("Swipe direction: " + direction);
        }
        //reset the variables
        switch (direction)
        {
            case Direction.Left:
                GetComponent<BoxCollider2D>().offset = new Vector2(-0.2f, 0);
                break;
            case Direction.Up:
                GetComponent<BoxCollider2D>().offset = new Vector2(0, 0.2f);
                break;
            case Direction.Right:
                GetComponent<BoxCollider2D>().offset = new Vector2(0.2f, 0);
                break;
            case Direction.Down:
                GetComponent<BoxCollider2D>().offset = new Vector2(0, -0.2f);

                break;
            case Direction.None:
                GetComponent<BoxCollider2D>().offset = new Vector2(0, 0);
                break;
        }

        startPos = Vector2.zero;
        endPos = Vector2.zero;
        draggingStarted = false;
    }
    private const float TWEEN_DURATION = 0.2f;
    public async Task Swap(Transform obj1, Transform obj2)
    {
       
        var sequence = DOTween.Sequence();
        sequence.Join(obj1.DOMove(obj2.position, TWEEN_DURATION))
            .Join(obj2.DOMove(obj1.position, TWEEN_DURATION));

        await sequence.Play().AsyncWaitForCompletion();
        GetComponent<BoxCollider2D>().offset = new Vector2(0, 0);
    }

    private async void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.TryGetComponent(out Candy comp))
        {
            
            await Swap(transform, collision.transform);

        }
    }

}