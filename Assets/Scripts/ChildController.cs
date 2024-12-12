using UnityEngine;

public class ChildController : NPCController
{
    public override void SpottedPlayer()
    {
        movement.SetState(NPCMovement.MovementState.SCARED);
    }
    public override void LostPlayer()
    {
        movement.SetState(NPCMovement.MovementState.IDLE);
        movement.RecalculateIdlePosition();
    }
}
