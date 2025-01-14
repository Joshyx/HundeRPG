using UnityEngine;

public class AnimalCatcherController : NPCController
{
    private void FixedUpdate()
    {
        if(MenuController.IsGamePaused()) return;
        
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

    protected override void LostPlayer()
    {
        movement.SetState(NPCMovement.MovementState.IDLE);
        movement.idleTargetPos = player.transform.position;
    }
}
