using UnityEngine;

// Pickups spin and bob so the player can read them as "take me".
public class Spinner : MonoBehaviour
{
    public float spin = 100f, bobHeight = 0.12f, bobSpeed = 2.5f;
    float baseY;

    void Start() { baseY = transform.localPosition.y; }

    void Update()
    {
        transform.Rotate(0f, spin * Time.deltaTime, 0f, Space.World);
        Vector3 p = transform.localPosition;
        p.y = baseY + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.localPosition = p;
    }
}
