using UnityEngine;

public abstract class NPCController : MonoBehaviour
{
    public float maxHealth = 100f;
    public float viewDistance = 10f;
    
    private float currentHealth;

    protected GameObject player;
    protected NPCMovement movement;

    void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player");
        movement = GetComponent<NPCMovement>();
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, player.transform.position) > viewDistance && movement.GetState() != NPCMovement.MovementState.IDLE)
        {
            LostPlayer();
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
        SpottedPlayer();
    }
    
    public abstract void SpottedPlayer();
    public abstract void LostPlayer();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }
}
