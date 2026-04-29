using UnityEngine;

public class Player_IdleState : Player_GroundedState
{
    public Player_IdleState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.SetVelocity(0, rb.linearVelocityY);
    }
    public override void Update()
    {
        base.Update();

        if (Mathf.Sign(player.moveInput.x) == Mathf.Sign(player.facingDir) && player.wallDetected)
            return;

        if (player.moveInput.x != 0)
            stateMachine.ChangeState(player.moveState);
        
        player.HandleGroundMovement(player.moveInput.x);
    }
}