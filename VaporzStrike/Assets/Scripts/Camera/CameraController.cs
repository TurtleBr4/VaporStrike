using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform[] players;

    [Header("Movement")]
    public float followSpeed = 4f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("FOV")]
    public float minFOV = 50f;
    public float maxFOV = 65f;
    public float maxPlayerSpread = 12f;
    public float fovSmoothSpeed = 3f;

    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        players = new Transform[GameManager.playerCount];
    }

    private void Start()
    {
 

        /*
        int count = 0;
        foreach(GameObject p in GameObject.FindGameObjectsWithTag("Player"))
        {
            players[count] = p.transform;
            count++;
        }
        */
    }

    public void init()
    {
        int count = 0;
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
        {
            print(p.name + count);
            players[count] = p.transform;
            count++;
        }
    }

    private void Update()
    {
        if (players == null || players.Length == 0)
        {
            init();
        }
    }
    void LateUpdate()
    {
        if (players == null || players.Length == 0)
        {
            return;
        }

        Vector3 center = getCenterPoint();
        Vector3 targetPos = center + offset;

        //2d lock
        targetPos.z = offset.z;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            followSpeed * Time.deltaTime
        );

        float spread = getPlayerDistance();
        float t = Mathf.Clamp01(spread / maxPlayerSpread);
        float targetFOV = Mathf.Lerp(minFOV, maxFOV, t);

        cam.fieldOfView = Mathf.Lerp(
            cam.fieldOfView,
            targetFOV,
            fovSmoothSpeed * Time.deltaTime
        );
    }

    Vector3 getCenterPoint()
    {
        if (players.Length == 1)
            return players[0].position;

        Bounds bounds = new Bounds(players[0].position, Vector3.zero);

        foreach (Transform p in players)
            bounds.Encapsulate(p.position);

        return bounds.center;
    }

    float getPlayerDistance()
    {
        float maxDist = 0f;

        for (int i = 0; i < players.Length; i++)
        {
            for (int j = i + 1; j < players.Length; j++)
            {
                float dist = Vector2.Distance(
                    new Vector2(players[i].position.x, players[i].position.y),
                    new Vector2(players[j].position.x, players[j].position.y)
                );

                if (dist > maxDist)
                    maxDist = dist;
            }
        }

        return maxDist;
    }
}
