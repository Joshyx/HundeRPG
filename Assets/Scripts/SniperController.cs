using System;
using UnityEngine;

public class SniperController : NPCController
{
    public float snipeDistance;
    public float fleeDistance;

    public float fireRate = 2f;
    public float bulletSpeed = 5f;
    
    public GameObject bulletPrefab;
    
    public AudioClip fireSound;
    
    private void FixedUpdate()
    {
        if (MenuController.IsGamePaused()) return;
        
        var distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance > snipeDistance && distance < viewDistance)
        {
            if (!movement.CanMove()) movement.EnableMovement();
            movement.SetState(NPCMovement.MovementState.ANGRY);
        } else if (distance < snipeDistance && distance > fleeDistance)
        {
            var hit = Physics2D.Raycast(transform.position, player.transform.position - transform.position, distance, LayerMask.GetMask("Environment"));
            if (hit)
            {
                if (!movement.CanMove()) movement.EnableMovement();
                return;
            }
            if(movement.CanMove()) movement.DisableMovement();

            TryShoot();
        } else if (distance < fleeDistance)
        {
            if (!movement.CanMove()) movement.EnableMovement();
            movement.SetState(NPCMovement.MovementState.SCARED);
        }
    }

    private DateTime? lastShot = null;
    private void TryShoot()
    {
        var secondsSinceLastShot = DateTime.Now.Subtract(lastShot ?? DateTime.UnixEpoch).TotalSeconds;
        if (secondsSinceLastShot < fireRate)
        {
            return;
        }
        lastShot = DateTime.Now;
        anim.SetTrigger("Shoot");
        AudioSource.PlayClipAtPoint(fireSound, transform.position);
        var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        var playerWalkDir = (Vector3) player.GetComponent<Rigidbody2D>().linearVelocity.normalized;
        bullet.GetComponent<Rigidbody2D>().linearVelocity = ((player.transform.position + playerWalkDir * 3) - transform.position).normalized * bulletSpeed;
    }

    public override void SpottedPlayer()
    {
        movement.SetState(NPCMovement.MovementState.ANGRY);
    }

    protected override void LostPlayer()
    {
        movement.SetState(NPCMovement.MovementState.IDLE);
    }

    private new void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, snipeDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, fleeDistance);
    }
}
