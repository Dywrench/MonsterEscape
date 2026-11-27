using UnityEngine;

public class PlatformMovementX : MonoBehaviour
{
    public float leftPosition = -5f;
    public float rightPosition = 5f;
    public float speed = 2f;
    public float waitTime = 2f;

    private bool movingRight = true;
    private bool waiting = false;

    void Update()
    {
        if (waiting)
            return;

        if (movingRight)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                new Vector3(rightPosition, transform.position.y, transform.position.z),
                speed * Time.deltaTime
            );

            if (transform.position.x >= rightPosition)
            {
                StartCoroutine(WaitAndChangeDirection(false));
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                new Vector3(leftPosition, transform.position.y, transform.position.z),
                speed * Time.deltaTime
            );

            if (transform.position.x <= leftPosition)
            {
                StartCoroutine(WaitAndChangeDirection(true));
            }
        }
    }

    System.Collections.IEnumerator WaitAndChangeDirection(bool direction)
    {
        waiting = true;

        yield return new WaitForSeconds(waitTime);

        movingRight = direction;
        waiting = false;
    }
}