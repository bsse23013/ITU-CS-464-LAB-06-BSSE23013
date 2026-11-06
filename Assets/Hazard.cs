using System.Collections;
using UnityEngine;

// One script is every hazard: spikes, a guard's body, a sweeper, a crusher.
// With restTime > 0 it becomes a TIMED hazard: rest -> warn -> strike.
public class Hazard : MonoBehaviour
{
    public int damage = 1;
    public float knockback = 6f;
    public float restTime = 0f, warnTime = 0.7f, strikeTime = 0.4f, startDelay = 0f;
    public GameObject warning, blade;

    IEnumerator Start()
    {
        if (restTime <= 0f) yield break;                  // static / moving hazards: always dangerous
        Collider zone = GetComponent<Collider>();
        if (warning != null) warning.SetActive(false);
        if (blade != null) blade.SetActive(false);
        if (startDelay > 0f)
        {
            zone.enabled = false;
            yield return new WaitForSeconds(startDelay);
        }
        while (true)
        {
            zone.enabled = false;
            if (blade != null) blade.SetActive(false);
            yield return new WaitForSeconds(restTime);
            if (warning != null) warning.SetActive(true);      // telegraph first
            yield return new WaitForSeconds(warnTime);
            if (warning != null) warning.SetActive(false);
            zone.enabled = true;                               // then strike
            if (blade != null) blade.SetActive(true);
            yield return new WaitForSeconds(strikeTime);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponent(out Health health) || !health.TakeDamage(damage)) return;
        Rigidbody body = other.attachedRigidbody;
        if (body == null) return;
        Vector3 away = body.position - transform.position;
        away.y = 0f;
        body.AddForce(away.normalized * knockback, ForceMode.Impulse);
    }
}
