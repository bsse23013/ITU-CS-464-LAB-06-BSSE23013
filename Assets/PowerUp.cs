using UnityEngine;

public enum PowerUpType { Speed, Shield }

// A pickup is a trigger that reports and disappears. The timer belongs to the player.
public class PowerUp : MonoBehaviour
{
    public PowerUpType type = PowerUpType.Speed;
    public float multiplier = 1.6f;
    public float duration = 5f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (type == PowerUpType.Speed)
        {
            other.GetComponent<PlayerController>().BoostSpeed(multiplier, duration);
            GameManager.Instance.ShowToast("Speed boost!");
        }
        else
        {
            other.GetComponent<Health>().shielded = true;
            GameManager.Instance.ShowToast("Shield ready: blocks the next hit");
        }
        Destroy(gameObject);
    }
}
