using UnityEngine;
using System.Collections;

public class LevelObjectPath : MonoBehaviour
{
    public enum MovementMode
    {
        Linear,
        EaseInOut,
        EaseIn,
        EaseOut,
        SmoothStep,
        SmootherStep,
        Instant
    }

    [Header("Movement")]
    [SerializeField] private Transform[] points;
    [SerializeField] private MovementMode movementMode = MovementMode.EaseInOut;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float delayBetweenPoints = 0.5f;
    [SerializeField] private bool loop;
    [SerializeField] private bool pingPong;
    private float ogDelay;

    private Coroutine movementCoroutine;

    private void Start()
    {
        StartMovement();
        ogDelay = delayBetweenPoints;
    }

    private void Update()
    {
        delayBetweenPoints = Random.Range(ogDelay, ogDelay + 1f);
    }

    public void StartMovement()
    {
        if (movementCoroutine != null)
            StopCoroutine(movementCoroutine);

        movementCoroutine = StartCoroutine(MoveThroughPoints());
    }

    public void StopMovement()
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
        }
    }

    private IEnumerator MoveThroughPoints()
    {
        if (points == null || points.Length < 2)
            yield break;

        int index = 0;
        int direction = 1;

        while (true)
        {
            int nextIndex = index + direction;

            if (nextIndex >= points.Length || nextIndex < 0)
            {
                if (pingPong)
                {
                    direction *= -1;
                    nextIndex = index + direction;
                }
                else if (loop)
                {
                    nextIndex = 0;
                    index = 0;
                    transform.position = points[0].position;
                }
                else
                {
                    break;
                }
            }

            yield return MoveToPoint(points[index].position, points[nextIndex].position);

            index = nextIndex;

            // Wait before moving to the next point
            if (delayBetweenPoints > 0f)
                yield return new WaitForSeconds(delayBetweenPoints);
        }

        movementCoroutine = null;
    }

    private IEnumerator MoveToPoint(Vector3 start, Vector3 end)
    {
        if (movementMode == MovementMode.Instant)
        {
            transform.position = end;
            yield break;
        }

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);
            float easedT = ApplyEasing(t);

            transform.position = Vector3.Lerp(start, end, easedT);

            yield return null;
        }

        transform.position = end;
    }

    private float ApplyEasing(float t)
    {
        switch (movementMode)
        {
            case MovementMode.Linear:
                return t;

            case MovementMode.EaseIn:
                return t * t;

            case MovementMode.EaseOut:
                return 1f - (1f - t) * (1f - t);

            case MovementMode.EaseInOut:
                return t < 0.5f
                    ? 2f * t * t
                    : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;

            case MovementMode.SmoothStep:
                return t * t * (3f - 2f * t);

            case MovementMode.SmootherStep:
                return t * t * t * (t * (6f * t - 15f) + 10f);

            default:
                return t;
        }
    }
}
