using UnityEngine;

public class AdultController : NPCController
{
    private SpriteRenderer spriteRenderer;
    public Sprite[] sprites;

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        var sprite = sprites[Random.Range(0, sprites.Length)];
        spriteRenderer.sprite = sprite;
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
