using UnityEngine;

public class ChildController : NPCController
{
    public override void SpottedPlayer()
    {
        movement.SetState(NPCMovement.MovementState.SCARED);
        var parents = Physics2D.OverlapCircleAll(transform.position, 8f, LayerMask.GetMask("NPC"));
        foreach (var parent in parents)
        {
            var component = parent.GetComponent<AdultController>();
            if (component)
            {
                component.SpottedPlayer();
            }
        }
    }
    public override void LostPlayer()
    {
        movement.SetState(NPCMovement.MovementState.IDLE);
        movement.RecalculateIdlePosition();
    }
}
