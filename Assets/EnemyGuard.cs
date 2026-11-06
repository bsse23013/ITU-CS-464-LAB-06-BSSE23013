using UnityEngine;

// From patrol to chase: Patrol -> Chase -> Return -> Patrol (Lecture 07, slide 20).
// While in Patrol, the EnemyPatrol component does the walking.
[RequireComponent(typeof(EnemyPatrol))]
public class EnemyGuard : MonoBehaviour
{
    enum State { Patrol, Chase, Return }

    public float chaseSpeed = 4f, sightRange = 7f, fieldOfView = 240f, loseSightDelay = 1.2f, turnSpeed = 540f;
    public Color calmColor = new Color(1f, 0.55f, 0.15f);
    public Color alertColor = new Color(1f, 0.1f, 0.1f);

    EnemyPatrol patrol;
    Renderer rend;
    State state = State.Patrol;
    float lastSeen;
    int returnIndex;

    void Awake()
    {
        patrol = GetComponent<EnemyPatrol>();
        rend = GetComponent<Renderer>();
    }

    void Start()
    {
        if (rend != null) rend.material.color = calmColor;
    }

    void Update()
    {
        GameManager gm = GameManager.Instance;
        Transform player = gm != null ? gm.PlayerTransform : null;
        bool sees = player != null && !gm.Respawning && CanSee(player, state != State.Chase);
        if (sees) lastSeen = Time.time;

        switch (state)
        {
            case State.Patrol:
                if (sees) Enter(State.Chase);
                break;

            case State.Chase:
                if (player != null)
                    MoveTo(new Vector3(player.position.x, transform.position.y, player.position.z), chaseSpeed);
                if (!sees && Time.time - lastSeen > loseSightDelay) Enter(State.Return);
                break;

            case State.Return:
            {
                if (sees) { Enter(State.Chase); break; }
                Vector3 target = patrol.points[returnIndex].position;
                MoveTo(target, patrol.speed);
                if (transform.position == target) Enter(State.Patrol);
                break;
            }
        }
    }

    void Enter(State s)
    {
        state = s;
        patrol.enabled = (s == State.Patrol);
        if (s == State.Return) returnIndex = patrol.NearestIndex();
        if (s == State.Patrol) patrol.SetIndex(returnIndex);
        if (rend != null) rend.material.color = (s == State.Chase) ? alertColor : calmColor;
    }

    void MoveTo(Vector3 target, float speed)
    {
        Vector3 to = target - transform.position;
        to.y = 0f;
        if (to != Vector3.zero)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(to), turnSpeed * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    // Range + field of view + a raycast, so walls hide the player.
    bool CanSee(Transform player, bool useFieldOfView)
    {
        Vector3 full = player.position - transform.position;
        if (full.magnitude > sightRange) return false;
        Vector3 flat = full; flat.y = 0f;
        if (useFieldOfView && Vector3.Angle(transform.forward, flat) > fieldOfView * 0.5f) return false;

        Vector3 eye = transform.position + Vector3.up * 0.2f;
        Vector3 dir = player.position - eye;
        if (Physics.Raycast(eye, dir.normalized, out RaycastHit hit, sightRange, ~0, QueryTriggerInteraction.Ignore))
            return hit.collider.CompareTag("Player");
        return false;
    }
}
