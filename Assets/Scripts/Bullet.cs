using UnityEngine;
public class Bullet : MonoBehaviour
{
    public float damage = 15f;
    private Rigidbody2D rb;
    private Vector2 velocity;
    public ParticleSystem explosionParticles;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        velocity = rb.linearVelocity;
        var angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void Update()
    {
        if (MenuController.IsGamePaused())
        {
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            rb.linearVelocity = velocity;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }
        explosionParticles.Play();
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        velocity = Vector2.zero;
        Destroy(gameObject, 0.2f);
    }
}
