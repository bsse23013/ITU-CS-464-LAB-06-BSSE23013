using UnityEngine;

// A coin / gem / key. In timed levels, "cyan" coins also add seconds to the clock.
public class Collectible : MonoBehaviour
{
    public int value = 1;
    public float timeBonus = 0f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        GameManager.Instance.AddScore(value, timeBonus);
        Destroy(gameObject);
    }
}
