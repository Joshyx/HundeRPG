using UnityEngine;

class PlayerMovement : MonoBehaviour
{
    float horizontal;
    float vertical;
    float moveLimiter = 0.7f;
    bool canMove = true;

    public float runSpeed = 5.0f;


    void Update()
    {
        // Gives a value between -1 and 1
        horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
        vertical = Input.GetAxisRaw("Vertical"); // -1 is down

        if (horizontal < 0)
        {
            if (transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }
        } 
        else if (horizontal > 0)
        {
            if (transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }
        }
    }

    void FixedUpdate()
    {
        if (!canMove)
        {
            return;
        }
        
        if (horizontal != 0 && vertical != 0) // Check for diagonal movement
        {
            // limit movement speed diagonally, so you move at 70% speed
            horizontal *= moveLimiter;
            vertical *= moveLimiter;
        }

        transform.position += new Vector3(horizontal, vertical, 0) * (Time.fixedDeltaTime * runSpeed);
    }

    public void DisableMovement()
    {
        canMove = false;
    }

    public void EnableMovement()
    {
        canMove = true;
    }
}