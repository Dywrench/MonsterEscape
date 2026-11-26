using UnityEngine;
using System.Collections;

public class PlatformMovement : MonoBehaviour
{
    public float lowerPosition = -2.95f;
    public float upperPosition = 33.07f;
    public float speed = 2f;
    public float waitTime = 2f;

    private bool movingUp = true;
    private bool waiting = false;

    void Update()
    {
        if (waiting)
            return;

        if (movingUp)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                new Vector3(transform.position.x, upperPosition, transform.position.z),
                speed * Time.deltaTime
            );

            if (transform.position.y >= upperPosition)
            {
                StartCoroutine(WaitAndChangeDirection(false));
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                new Vector3(transform.position.x, lowerPosition, transform.position.z),
                speed * Time.deltaTime
            );

            if (transform.position.y <= lowerPosition)
            {
                StartCoroutine(WaitAndChangeDirection(true));
            }
        }
    }

    IEnumerator WaitAndChangeDirection(bool direction)
    {
        waiting = true;

        yield return new WaitForSeconds(waitTime);

        movingUp = direction;
        waiting = false;
    }
}