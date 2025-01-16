using UnityEngine;
using UnityEngine.UI;

public abstract class NPCController : MonoBehaviour
{
    public float maxHealth = 100f;
    public float viewDistance = 10f;
    public float attackDistance = 2f;
    
    public float damage = 10f;
    public float damageCooldownSeconds = 0.5f;
    private float? startOfAttack = null;
    
    private float currentHealth;

    public float xpOnDamage = 15f;
    
    public Slider healthSlider;
    public Consumable coinObject;
    public GameObject healthObject;

    protected PlayerController player;
    protected NPCMovement movement;
    protected Animator anim;
    
    public AudioClip deathSound;
    public AudioClip hurtSound;

    private void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        movement = GetComponent<NPCMovement>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if(MenuController.IsGamePaused()) return;
        
        if (Vector2.Distance(transform.position, player.transform.position) > viewDistance && movement.GetState() != NPCMovement.MovementState.IDLE)
        {
            LostPlayer();
        }

        TryAttack();
    }

    public void TakeDamage(float amount, bool isPlayerDamage = true)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
            movement.DisableMovement();
            healthSlider.gameObject.SetActive(false);
            anim.SetTrigger("Death");
            var obj = Instantiate(coinObject, transform.position, Quaternion.identity);
            obj.coins = (int) maxHealth / 10;
            if(Random.value < 0.2f) Instantiate(healthObject, transform.position + (Vector3)Random.insideUnitCircle, Quaternion.identity);
            
            GameProgressController.AddXP(xpOnDamage);
            Destroy(gameObject, 0.5f);
            return;
        }
        anim.SetTrigger("Hurt");
        healthSlider.gameObject.SetActive(true);
        healthSlider.value = currentHealth / maxHealth;
        AudioSource.PlayClipAtPoint(hurtSound, transform.position);
        
        if(isPlayerDamage) SpottedPlayer();
    }

    private void TryAttack()
    {
        if (damage <= 0 ) return;
        if (movement.GetState() != NPCMovement.MovementState.ANGRY) return;

        if (Vector2.Distance(transform.position, player.transform.position) > attackDistance)
        {
            if (startOfAttack is not null)
            {
                startOfAttack = null;
                movement.EnableMovement();
            }
            return;
        }

        if (startOfAttack is null)
        {
            startOfAttack = Time.time;
            movement.DisableMovement();
            return;
        }
        
        var timeSinceStart = Time.time - startOfAttack.Value;
        if (timeSinceStart < damageCooldownSeconds)
        {
            return;
        }
        
        anim.SetTrigger("Attack");
        player.TakeDamage(damage);
        movement.EnableMovement();
        startOfAttack = null;
    }

    public abstract void SpottedPlayer();
    protected abstract void LostPlayer();

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}
