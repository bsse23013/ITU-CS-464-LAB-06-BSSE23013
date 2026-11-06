using System.Collections;
using UnityEngine;

// Lecture 06's controller, plus BoostSpeed (Lecture 07, Part 3).
// The boost coroutine lives HERE, on the player, so destroying the pickup can't stop it.
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveForce = 12f;

    Rigidbody rb;
    Renderer rend;
    Health health;
    Vector3 input;
    float speedMultiplier = 1f;
    float boostEnds;
    Coroutine boost;

    public bool Boosted => speedMultiplier > 1f;
    public float BoostLeft => Boosted ? Mathf.Max(0f, boostEnds - Time.time) : 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        health = GetComponent<Health>();
        rb.sleepThreshold = 0f;   // never sleep, or OnTriggerStay stops firing when the ball is still
    }

    void Update()
    {
        Vector2 m = GameInput.Move();
        input = Vector3.ClampMagnitude(new Vector3(m.x, 0f, m.y), 1f);
        UpdateLook();
    }

    void FixedUpdate() => rb.AddForce(input * moveForce * speedMultiplier);

    public void BoostSpeed(float multiplier, float seconds)
    {
        if (boost != null) StopCoroutine(boost);   // newest pickup wins, clock restarts (slide 24: REFRESH)
        boost = StartCoroutine(Boost(multiplier, seconds));
    }

    IEnumerator Boost(float multiplier, float seconds)
    {
        speedMultiplier = multiplier;
        boostEnds = Time.time + seconds;
        yield return new WaitForSeconds(seconds);
        speedMultiplier = 1f;
        boost = null;
    }

    // Visual feedback: blink during i-frames, tint for shield / boost.
    void UpdateLook()
    {
        if (rend == null) return;
        if (health != null)
            rend.enabled = !health.Invulnerable || ((int)(Time.time * 12f) % 2 == 0);
        Color c = new Color(0.95f, 0.95f, 0.95f);
        if (Boosted) c = new Color(1f, 0.85f, 0.2f);
        if (health != null && health.shielded) c = new Color(0.3f, 0.85f, 1f);
        rend.material.color = c;
    }
}
