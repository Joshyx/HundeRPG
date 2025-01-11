public class AdultController : NPCController
{
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
