using UnityEngine;

public class Player_DashState : PlayerState
{
    private float originalGravityScale;
    private Vector2 dashDirection;
    private float activeDashSpeed;
    private Entity_VFX vfx;

    public Player_DashState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        vfx = player.GetComponent<Entity_VFX>();
    }

    public override void Enter()
    {
        base.Enter();

        player.ConsumeDashCharge();

        Vector2 rawInput = input.Player.Movement.ReadValue<Vector2>();
        dashDirection = CalculateDashDirection(rawInput);

        stateTimer = player.dashDuration;
        originalGravityScale = rb.gravityScale;
        rb.gravityScale = 0;

        activeDashSpeed = player.dashSpeed;

        if (Mathf.Abs(dashDirection.x) > 0.1f && Mathf.Abs(dashDirection.y) > 0.1f)
            activeDashSpeed *= player.dashDiagonalSpeedMultiplier;

        if (Mathf.Abs(dashDirection.x) > 0.1f)
        {
            if (dashDirection.x > 0 && player.facingDir < 0)
                player.Flip();
            else if (dashDirection.x < 0 && player.facingDir > 0)
                player.Flip();
        }

        rb.linearVelocity = dashDirection * activeDashSpeed;

        // Juice - Freeze + Shake (Inspector-tunable)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.FreezeFrame(player.dashFreezeFrameDuration);
            GameManager.Instance.ScreenShake(player.dashShakeDuration, player.dashShakeIntensity);
        }

        // Juice - Afterimages
        if (vfx != null)
            vfx.StartAfterimages();

        // Juice - Dash Burst VFX
        if (player.dashVfxPrefab != null)
        {
            float angle = Mathf.Atan2(dashDirection.y, dashDirection.x) * Mathf.Rad2Deg;
            GameObject burst = Object.Instantiate(player.dashVfxPrefab, player.transform.position, Quaternion.Euler(0, 0, angle));
            Object.Destroy(burst, 0.5f);
        }

        // Juice - Sound
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayPlayerDash(player.transform.position);
    }

    public override void Update()
    {
        base.Update();
        CancelDashIfNeeded();

        if (stateTimer > player.dashDecelerationThreshold)
        {
            rb.linearVelocity = dashDirection * activeDashSpeed;
        }
        else
        {
            float t = stateTimer / player.dashDecelerationThreshold;
            float currentSpeed = Mathf.Lerp(player.dashEndSpeed, activeDashSpeed, t);
            rb.linearVelocity = dashDirection * currentSpeed;
        }

        if (stateTimer < 0)
            ExitDash();
    }

    public override void Exit()
    {
        base.Exit();
        rb.gravityScale = originalGravityScale;

        if (vfx != null)
            vfx.StopAfterimages();
    }

    private void ExitDash()
    {
        float momentumKeep = 0.4f;

        float exitX = dashDirection.x * activeDashSpeed * momentumKeep;
        float exitY = dashDirection.y > 0 ? dashDirection.y * activeDashSpeed * 0.3f : 0;

        rb.linearVelocity = new Vector2(exitX, exitY);

        if (player.groundDetected)
            stateMachine.ChangeState(player.idleState);
        else
            stateMachine.ChangeState(player.fallState);
    }

    private Vector2 CalculateDashDirection(Vector2 rawInput)
    {
        if (rawInput.magnitude < 0.1f)
            return new Vector2(player.facingDir, 0);

        float x = Mathf.Abs(rawInput.x) > 0.1f ? Mathf.Sign(rawInput.x) : 0;
        float y = Mathf.Abs(rawInput.y) > 0.1f ? Mathf.Sign(rawInput.y) : 0;

        return new Vector2(x, y).normalized;
    }

    private void CancelDashIfNeeded()
    {
        if (player.wallDetected)
        {
            if (player.groundDetected)
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.wallSlideState);
        }
    }
}