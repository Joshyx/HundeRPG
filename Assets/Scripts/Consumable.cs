using UnityEngine;

public class Consumable : MonoBehaviour
{
    public float speedMultiplier = 1f;
    public float damageMultiplier = 1f;
    public int coins = 0;
    public float effectDurationSeconds = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        GetComponentInChildren<ParticleSystem>().Play();
        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(gameObject, 0.2f);
    }
}
