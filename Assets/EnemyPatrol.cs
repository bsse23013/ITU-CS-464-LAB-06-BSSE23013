using UnityEngine;

public enum PatrolMode { Loop, PingPong, Random }

// A patrol is a list, a counter and MoveTowards.
public class EnemyPatrol : MonoBehaviour
{
    public Transform[] points;
    public PatrolMode mode = PatrolMode.Loop;
    public float speed = 2.6f, waitTime = 0f, turnSpeed = 360f;

    int index, step = 1;
    float waitUntil;

    void Update()
    {
        if (points == null || points.Length == 0) return;
        if (Time.time < waitUntil) return;

        Vector3 target = points[index].position;
        Vector3 toTarget = target - transform.position;
        toTarget.y = 0f;
        if (toTarget != Vector3.zero)                       // face where you walk
        {
            Quaternion look = Quaternion.LookRotation(toTarget);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, look, turnSpeed * Time.deltaTime);
        }

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if (transform.position == target)                  // safe: MoveTowards lands exactly
        {
            index = NextIndex();
            waitUntil = Time.time + waitTime;
        }
    }

    int NextIndex()
    {
        if (points.Length < 2) return 0;
        if (mode == PatrolMode.Loop)
            return (index + 1) % points.Length;
        if (mode == PatrolMode.PingPong)
        {
            int ahead = index + step;
            if (ahead < 0 || ahead >= points.Length) step = -step;
            return index + step;
        }
        int next = Random.Range(0, points.Length);
        return next == index ? (next + 1) % points.Length : next;
    }

    // Used by EnemyGuard when it returns to its route.
    public int NearestIndex()
    {
        int best = 0;
        float bestDist = float.MaxValue;
        for (int i = 0; i < points.Length; i++)
        {
            float d = (points[i].position - transform.position).sqrMagnitude;
            if (d < bestDist) { bestDist = d; best = i; }
        }
        return best;
    }

    public void SetIndex(int i) { index = Mathf.Clamp(i, 0, points.Length - 1); }
}
