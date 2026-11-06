using UnityEngine;

// Health owns the hearts and the rule about how often they can be lost (i-frames).
public class Health : MonoBehaviour
{
    [SerializeField] int max = 3;
    [SerializeField] float iFrames = 1f;
    public bool shielded;
    int current;
    float invulnerableUntil;

    public int Current => current;
    public int Max => max;
    public bool Invulnerable => Time.time < invulnerableUntil;

    void Awake() => current = max;

    public bool TakeDamage(int amount)
    {
        if (current <= 0 || Time.time < invulnerableUntil) return false;
        invulnerableUntil = Time.time + iFrames;           // start the clock first, even if the shield absorbs it
        if (shielded) { shielded = false; return false; }
        current -= amount;
        if (current <= 0) GameManager.Instance.PlayerDied();   // <= 0, never == 0 (a hit of 2 skips 0)
        return true;
    }

    public void Restore(float protection)
    {
        current = max;
        invulnerableUntil = Time.time + protection;
    }
}
