using System;
using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

public class Player : Entity
{
    public static event Action OnPlayerDeath;

    private UI ui;
    public PlayerInputSet input { get; private set; }
    public PlayerLoadoutBridge loadoutBridge { get; private set; }

    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Player_JumpState jumpState { get; private set; }
    public Player_FallState fallState { get; private set; }
    public Player_WallSlideState wallSlideState { get; private set; }
    public Player_WallJumpState wallJumpState { get; private set; }
    public Player_DashState dashState { get; private set; }
    public Player_BasicAttackState basicAttackState { get; private set; }
    public Player_JumpAttackState jumpAttackState { get; private set; }
    public Player_DeadState deadState { get; private set; }
    public Player_CounterAttackState counterAttackState { get; private set; }

    [Header("Attack details")]
    public Vector2[] attackVelocity;
    public Vector2 jumpAttackVelocity;
    public float attackVelocityDuration = .1f;
    public float comboResetTime = 1f;
    private Coroutine queuedAttackCo;

    [Header("Movement details")]
    public float moveSpeed;
    public Vector2 wallJumpForce;
    [Range(0, 1)]
    public float inAirMoveMultiplier = .7f;
    [Range(0, 1)]
    public float wallSlideSlowMultiplier = .7f;
    [Space]
    public float dashDuration = .25f;
    public float dashSpeed = 20f;

    [Header("Jump details")]
    public float jumpHeight = 4.2f;
    public float jumpApexTime = .32f;
    public float coyoteTime = .12f;
    public float jumpBufferTime = .12f;
    [Range(.1f, 1f)]
    public float jumpCutMultiplier = .5f;
    public float fallSpeedLimit = -20f;

    [Header("Air tuning")]
    public float jumpHangTimeThreshold = 1.5f;
    public float apexMoveMultiplier = 1.15f;
    public float apexGravityMultiplier = .75f;
    public float riseGravityMultiplier = 1f;
    public float fallGravityMultiplier = 2f;
    public float airAcceleration = 65f;
    public float airDeceleration = 55f;

    [Header("Jump assist")]
    public float edgeNudgeForce = 1.5f;

    public Vector2 moveInput { get; private set; }

    public float jumpForce { get; private set; }
    public float defaultGravityScale { get; private set; }

    public float coyoteTimeCounter { get; private set; }
    public float jumpBufferCounter { get; private set; }

    public bool jumpInputReleased { get; private set; }
    public bool isJumping { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        ui = FindAnyObjectByType<UI>();

        input = new PlayerInputSet();

        loadoutBridge = GetComponent<PlayerLoadoutBridge>();

        RecalculateJumpValues();

        idleState = new Player_IdleState(this, stateMachine, "idle");
        moveState = new Player_MoveState(this, stateMachine, "move");
        jumpState = new Player_JumpState(this, stateMachine, "jumpFall");
        fallState = new Player_FallState(this, stateMachine, "jumpFall");
        wallSlideState = new Player_WallSlideState(this, stateMachine, "wallSlide");
        wallJumpState = new Player_WallJumpState(this, stateMachine, "jumpFall");
        dashState = new Player_DashState(this, stateMachine, "dash");
        basicAttackState = new Player_BasicAttackState(this, stateMachine, "basicAttack");
        jumpAttackState = new Player_JumpAttackState(this, stateMachine, "jumpAttack");
        deadState = new Player_DeadState(this, stateMachine, "dead");
        counterAttackState = new Player_CounterAttackState(this, stateMachine, "counterAttack");
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();
        UpdateJumpTimers();
    }

    public override void EntityDeath()
    {
        base.EntityDeath();

        stateMachine.ChangeState(deadState);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RespawnAtLastCheckpoint();
        }
    }

    public void RecalculateJumpValues()
    {
        float gravity = -(2 * jumpHeight) / Mathf.Pow(jumpApexTime, 2);
        jumpForce = Mathf.Abs(gravity) * jumpApexTime;
        defaultGravityScale = gravity / Physics2D.gravity.y;
    }

    private void UpdateJumpTimers()
    {
        if (groundDetected)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        jumpBufferCounter -= Time.deltaTime;

        if (input != null)
            jumpInputReleased = input.Player.Jump.WasReleasedThisFrame();

        if (groundDetected && rb.linearVelocityY <= 0)
            isJumping = false;
    }

    public void BufferJumpInput()
    {
        if (input.Player.Jump.WasPressedThisFrame())
            jumpBufferCounter = jumpBufferTime;
    }

    public bool HasBufferedJump()
    {
        return jumpBufferCounter > 0;
    }

    public bool CanUseCoyoteJump()
    {
        return coyoteTimeCounter > 0;
    }

    public void ConsumeJump()
    {
        jumpBufferCounter = 0;
        coyoteTimeCounter = 0;
        jumpInputReleased = false;
        isJumping = true;
    }

    public void ResetJumpState()
    {
        isJumping = false;
        jumpInputReleased = false;
        rb.gravityScale = defaultGravityScale;
    }

    public void HandleBetterJump()
    {
        float yVelocity = rb.linearVelocityY;
        float targetGravityScale = defaultGravityScale;

        if (yVelocity > 0)
        {
            if (jumpInputReleased && isJumping)
                targetGravityScale *= 1 / jumpCutMultiplier;
            else if (yVelocity < jumpHangTimeThreshold)
                targetGravityScale *= apexGravityMultiplier;
            else
                targetGravityScale *= riseGravityMultiplier;
        }
        else if (yVelocity < 0)
        {
            targetGravityScale *= fallGravityMultiplier;
        }

        rb.gravityScale = targetGravityScale;

        if (rb.linearVelocityY < fallSpeedLimit)
            rb.linearVelocity = new Vector2(rb.linearVelocityX, fallSpeedLimit);
    }

    public void HandleAirMovement()
    {
        float targetSpeed = moveInput.x * moveSpeed * inAirMoveMultiplier;

        if (Mathf.Abs(rb.linearVelocityY) < jumpHangTimeThreshold)
            targetSpeed *= apexMoveMultiplier;

        float speedDif = targetSpeed - rb.linearVelocityX;
        float accelRate = Mathf.Abs(targetSpeed) > .01f ? airAcceleration : airDeceleration;
        float movement = speedDif * accelRate * Time.deltaTime;

        rb.linearVelocity = new Vector2(rb.linearVelocityX + movement, rb.linearVelocityY);
        HandleFlip(rb.linearVelocityX);
    }

    public void TryEdgeNudge()
    {
        if (Mathf.Abs(moveInput.x) <= .01f)
            return;

        rb.linearVelocity = new Vector2(rb.linearVelocityX + (moveInput.x * edgeNudgeForce), rb.linearVelocityY);
    }

    public void EnterAttackStateWithDelay()
    {
        if (queuedAttackCo != null)
            StopCoroutine(queuedAttackCo);

        queuedAttackCo = StartCoroutine(EnterAttackStateWithDelayCo());
    }

    private IEnumerator EnterAttackStateWithDelayCo()
    {
        yield return new WaitForEndOfFrame();
        stateMachine.ChangeState(basicAttackState);
    }

    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        float originalMoveSpeed = moveSpeed;
        Vector2 originalWallJump = wallJumpForce;
        float originalAnimSpeed = anim.speed;
        Vector2 originalJumpAttack = jumpAttackVelocity;
        Vector2[] originalAttackVelocity = new Vector2[attackVelocity.Length];
        Array.Copy(attackVelocity, originalAttackVelocity, originalAttackVelocity.Length);

        float originalJumpHeight = jumpHeight;
        float originalJumpApexTime = jumpApexTime;
        float originalFallSpeedLimit = fallSpeedLimit;
        float originalAirAcceleration = airAcceleration;
        float originalAirDeceleration = airDeceleration;
        float originalEdgeNudgeForce = edgeNudgeForce;

        float speedMultiplier = 1 - slowMultiplier;

        moveSpeed *= speedMultiplier;
        wallJumpForce *= speedMultiplier;
        anim.speed *= speedMultiplier;
        jumpAttackVelocity *= speedMultiplier;
        jumpHeight *= speedMultiplier;
        jumpApexTime = Mathf.Max(.05f, jumpApexTime * speedMultiplier);
        fallSpeedLimit *= speedMultiplier;
        airAcceleration *= speedMultiplier;
        airDeceleration *= speedMultiplier;
        edgeNudgeForce *= speedMultiplier;

        for (int i = 0; i < attackVelocity.Length; i++)
        {
            attackVelocity[i] *= speedMultiplier;
        }

        RecalculateJumpValues();

        yield return new WaitForSeconds(duration);

        moveSpeed = originalMoveSpeed;
        wallJumpForce = originalWallJump;
        anim.speed = originalAnimSpeed;
        jumpAttackVelocity = originalJumpAttack;
        jumpHeight = originalJumpHeight;
        jumpApexTime = originalJumpApexTime;
        fallSpeedLimit = originalFallSpeedLimit;
        airAcceleration = originalAirAcceleration;
        airDeceleration = originalAirDeceleration;
        edgeNudgeForce = originalEdgeNudgeForce;

        for (int i = 0; i < attackVelocity.Length; i++)
        {
            attackVelocity[i] = originalAttackVelocity[i];
        }

        RecalculateJumpValues();
    }

    private void OnEnable()
    {
        input.Enable();

        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;

        input.Player.ToggleSkillTreeUI.performed += ctx => ui.ToggleSkillTreeUI();
    }

    void OnDisable()
    {
        input.Disable();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (jumpApexTime < .05f)
            jumpApexTime = .05f;

        if (jumpHeight < .1f)
            jumpHeight = .1f;

        if (airAcceleration < 0)
            airAcceleration = 0;

        if (airDeceleration < 0)
            airDeceleration = 0;

        if (Application.isPlaying == false)
            return;

        if (rb != null)
            RecalculateJumpValues();
    }
#endif
}
