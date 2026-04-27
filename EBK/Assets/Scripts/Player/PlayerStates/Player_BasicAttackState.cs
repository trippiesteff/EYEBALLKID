using UnityEngine;

public class Player_BasicAttackState : PlayerState
{
    private float attackVelocityTimer;
    private float lastTimeAttacked;
    private bool comboAttackQueued;
    private int attackDir;
    private int comboIndex = 1;
    private const int FirstComboIndex = 1;

    public Player_BasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        comboAttackQueued = false;
        ResetComboIndexIfNeeded();
        SyncAttackSpeed();

        attackDir = player.moveInput.x != 0 ? (int)player.moveInput.x : player.facingDir;

        anim.SetInteger("basicAttackIndex", comboIndex);

        ApplyAttackVelocity();
    }

    public override void Update()
    {
        base.Update();
        HandleAttackVelocity();

        if (input.Player.Attack.WasPressedThisFrame())
            QueuedNextAttack();

        if (triggerCalled)
            HandleStateExit();
    }

    public override void Exit()
    {
        base.Exit();
        comboIndex++;
        lastTimeAttacked = Time.time;
    }

    private void HandleStateExit()
    {
        if (comboAttackQueued)
        {
            anim.SetBool(animBoolName, false);
            player.EnterAttackStateWithDelay();
        }
        else
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    private void QueuedNextAttack()
    {
        if (comboIndex < GetComboLimit())
            comboAttackQueued = true;
    }

   private void HandleAttackVelocity()
{
    attackVelocityTimer -= Time.deltaTime;

    if (attackVelocityTimer < 0)
    {
        if (ShouldStopMovementAfterAttackLunge())
        {
            player.SetVelocity(0, rb.linearVelocityY);
        }
        else
        {
            float xVelocity = player.moveInput.x * player.moveSpeed;
            player.SetVelocity(xVelocity, rb.linearVelocityY);
        }
    }
}


    private void ApplyAttackVelocity()
    {
        Vector2[] velocitySet = GetAttackVelocitySet();

        if (velocitySet == null || velocitySet.Length == 0)
            return;

        int velocityIndex = Mathf.Clamp(comboIndex - 1, 0, velocitySet.Length - 1);
        Vector2 attackVelocity = velocitySet[velocityIndex];

        attackVelocityTimer = GetAttackVelocityDuration();
        player.SetVelocity(attackVelocity.x * attackDir, attackVelocity.y);
    }

    private void ResetComboIndexIfNeeded()
    {
        if (Time.time > lastTimeAttacked + GetComboResetTime())
            comboIndex = FirstComboIndex;

        if (comboIndex > GetComboLimit())
            comboIndex = FirstComboIndex;
    }

    private WeaponData GetCurrentWeapon()
    {
        if (player.loadoutBridge == null)
            return null;

        return player.loadoutBridge.GetEquippedWeapon();
    }

    private Vector2[] GetAttackVelocitySet()
    {
        WeaponData weapon = GetCurrentWeapon();

        if (weapon != null && weapon.attackVelocity != null && weapon.attackVelocity.Length > 0)
            return weapon.attackVelocity;

        return player.attackVelocity;
    }

    private float GetAttackVelocityDuration()
    {
        WeaponData weapon = GetCurrentWeapon();

        if (weapon != null)
            return weapon.attackVelocityDuration;

        return player.attackVelocityDuration;
    }

    private float GetComboResetTime()
    {
        WeaponData weapon = GetCurrentWeapon();

        if (weapon != null)
            return weapon.comboResetTime;

        return player.comboResetTime;
    }

    private int GetComboLimit()
    {
        Vector2[] velocitySet = GetAttackVelocitySet();

        if (velocitySet == null || velocitySet.Length == 0)
            return 1;

        return velocitySet.Length;
    }

    private bool ShouldStopMovementAfterAttackLunge()
{
    WeaponData weapon = GetCurrentWeapon();

    if (weapon != null)
        return weapon.stopMovementAfterAttackLunge;

    return true;
}

}
