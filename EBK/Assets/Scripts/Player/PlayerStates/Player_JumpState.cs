using UnityEngine;

public class Player_JumpState : Player_AiredState
{
    public Player_JumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.ConsumeJump();
        player.SetVelocity(rb.linearVelocityX, player.jumpForce);
        player.TryEdgeNudge();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayPlayerJump(player.transform.position);
    }

    public override void Update()
    {
        base.Update();

        if (rb.linearVelocity.y < 0 && stateMachine.currentState != player.jumpAttackState)
            stateMachine.ChangeState(player.fallState);
    }
}
