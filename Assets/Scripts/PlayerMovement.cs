using UnityEngine;

class PlayerMovement : MonoBehaviour
{
    float moveLimiter = 0.7f;
    bool canMove = true;

    private Rigidbody2D rb;

    public float runSpeed = 5.0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
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

        rb.linearVelocity = new Vector3(horizontal, vertical, 0) * runSpeed;
    }

    public void DisableMovement()
    {
        canMove = false;
        rb.linearVelocity = Vector3.zero;
    }

    public void EnableMovement()
    {
        canMove = true;
    }
}