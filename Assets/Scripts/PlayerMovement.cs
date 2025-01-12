using UnityEngine;

class PlayerMovement : MonoBehaviour
{
    float moveLimiter = 0.7f;
    bool canMove = true;

    private Rigidbody2D rb;
    private ParticleSystem walkParticles;
    public Animator textureAnimator;

    public float runSpeed = 5.0f;
    [HideInInspector]
    public float currentSpeed;
    
    public AudioClip[] walkSound;
    private AudioSource audioSource;

    private void Start()
    {
        walkParticles = GetComponentInChildren<ParticleSystem>();
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = runSpeed;
        audioSource = GetComponentInChildren<AudioSource>();
    }

    private void Update()
    {
        if (MenuController.IsGamePaused())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        
        // Gives a value between -1 and 1
        var horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
        var vertical = Input.GetAxisRaw("Vertical"); // -1 is down

        if (horizontal < 0) // Moves left
        {
            if (transform.localScale.x > 0) // Currently facing right
            {
                // Set look direction to left
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }
        } 
        else if (horizontal > 0) // Looks right
        {
            if (transform.localScale.x < 0) // Currently facing left
            {
                // Set look direction to right
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }
        }
        
        if (!canMove)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }
        
        if (horizontal != 0 && vertical != 0) // Check for diagonal movement
        {
            // limit movement speed diagonally, so you move at 70% speed
            horizontal *= moveLimiter;
            vertical *= moveLimiter;
        }

        if (horizontal != 0 || vertical != 0)
        {
            if(rb.linearVelocity == Vector2.zero) walkParticles.Play();

            textureAnimator.SetBool("isRunning", true);
            if (!audioSource.isPlaying)
            {
                audioSource.clip = walkSound[Random.Range(0, walkSound.Length)];
                audioSource.Play();
            }
        }
        else
        {
            walkParticles.Stop();
            audioSource.Stop();
            textureAnimator.SetBool("isRunning", false);
        }

        rb.linearVelocity = new Vector3(horizontal, vertical, 0) * currentSpeed;
    }

    public void DisableMovement()
    {
        canMove = false;
        rb.linearVelocity = Vector3.zero;
        audioSource.Stop();
        walkParticles.Stop();
    }

    public void EnableMovement()
    {
        canMove = true;
    }
}