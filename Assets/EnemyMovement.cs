using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private GameObject player;

    private MovementState state = MovementState.IDLE;
    private Vector2 idleTargetPos;
    
    private float health = 0;
    
    public float maxHealth = 100;
    public float speed = 3;
    public float fieldOfView = 15;
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
        UpdateState();

        Move();
    }

    void Move() 
    {
        Vector2 playerPos = player.transform.position;
        
        switch (GetState())
        {
            case MovementState.IDLE:
                transform.position = Vector2.MoveTowards(transform.position, idleTargetPos, speed * Time.deltaTime);
                break;
            case MovementState.ANGRY:
                transform.position = Vector2.MoveTowards(transform.position, playerPos, speed * Time.deltaTime);
                break;
            case MovementState.SCARED:
                transform.position = Vector2.MoveTowards(transform.position, playerPos, -1 * speed * Time.deltaTime);
                break;
        }
        
        if (Vector2.Distance(transform.position, idleTargetPos) < 0.1f)
        {
            idleTargetPos = new Vector2(transform.position.x + Random.Range(-10, 10), transform.position.y + Random.Range(-10, 10));
        }
    }

    void UpdateState()
    {
        if (Vector2.Distance(transform.position, player.transform.position) < fieldOfView)
        {
            state = MovementState.IDLE;
        }
    }

    void TakeDamage(float amount)
    {
        health -= amount;
        SetState(MovementState.ANGRY);
        
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetState(MovementState newState)
    {
        state = newState;
    }

    public MovementState GetState()
    {
        return state;
    }

    public enum MovementState
    {
        IDLE, ANGRY, SCARED
    }
}
