using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private GameObject player;

    private MovementState state = MovementState.IDLE;
    private Vector2 idleTargetPos;
    
    private float health = 0;
    
    public float maxHealth = 100;
    public float speed = 3;
    public float damage = 10;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        idleTargetPos = transform.position;
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            // Spieler aus dem Sichtfeld verschwunden
            
            // letzte bekannte Position abchecken
            idleTargetPos = other.transform.position;
            
            // Weiter rumlaufen
            state = MovementState.IDLE;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Attack();
        }
    }

    void Move() 
    {
        Vector2 playerPos = player.transform.position;
        
        switch (state)
        {
            case MovementState.IDLE:
                // Zur nächsten Zielposition laufen
                transform.position = Vector2.MoveTowards(transform.position, idleTargetPos, speed * Time.deltaTime);
                break;
            case MovementState.ANGRY:
                // Zum Spieler laufen
                transform.position = Vector2.MoveTowards(transform.position, playerPos, speed * Time.deltaTime);
                break;
            case MovementState.SCARED:
                // Vom Spieler weg laufen
                transform.position = Vector2.MoveTowards(transform.position, playerPos, -1 * speed * Time.deltaTime);
                break;
        }
        
        if (Vector2.Distance(transform.position, idleTargetPos) < 0.1f)
        {
            // Wenn die Zielposition erreicht wurde, eine zufällige neue aussuchen
            idleTargetPos = new Vector2(transform.position.x + Random.Range(-10, 10), transform.position.y + Random.Range(-10, 10));
        }
    }

    void TakeDamage(float amount)
    {
        health -= amount;
        state = MovementState.ANGRY;
        
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    void Attack()
    {
        player.GetComponent<PlayerController>().TakeDamage(damage);
    }

    public enum MovementState
    {
        IDLE, ANGRY, SCARED
    }
}
