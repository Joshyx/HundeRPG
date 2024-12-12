using UnityEngine;

public class AnimalCatcherController : NPCController
{
    void FixedUpdate()
    {
        if (Vector2.Distance(transform.position, player.transform.position) < viewDistance 
            && movement.GetState() == NPCMovement.MovementState.IDLE)
        {
            SpottedPlayer();
        }
    }
    
    public override void SpottedPlayer()
    {
        movement.SetState(NPCMovement.MovementState.ANGRY);
    }

    public override void LostPlayer()
    {
        movement.SetState(NPCMovement.MovementState.IDLE);
        movement.idleTargetPos = player.transform.position;
    }
}
