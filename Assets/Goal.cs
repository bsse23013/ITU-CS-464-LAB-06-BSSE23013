using UnityEngine;

// The exit pad. Grey while locked, green once the level's condition is met.
public class Goal : MonoBehaviour
{
    Renderer[] rends;
    bool? lastUnlocked;

    void Start() { rends = GetComponentsInChildren<Renderer>(); }

    void Update()
    {
        bool unlocked = GameManager.Instance.GoalUnlocked;
        if (lastUnlocked.HasValue && lastUnlocked.Value == unlocked) return;
        lastUnlocked = unlocked;
        Color c = unlocked ? new Color(0.2f, 0.9f, 0.4f) : new Color(0.45f, 0.45f, 0.5f);
        foreach (Renderer r in rends) r.material.color = c;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.Instance.GoalUnlocked)
            GameManager.Instance.ReachGoal();
    }
}
