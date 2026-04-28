using UnityEngine;
using UnityEngine.UI;

public class Entity_Health : MonoBehaviour, IDamageable
{
    private Slider healthBar;
    private Entity entity;
    private Entity_VFX entityVfx;
    private Entity_Stats entityStats;

    [SerializeField] protected float currentHealth;
    [SerializeField] protected bool isDead;

    [Header("Health regeneration")]
    [SerializeField] private float regenInterval = 1f;
    [SerializeField] private bool canRegenerateHealth = true;

    [Header("On Damage Knockback")]
    [SerializeField] private Vector2 knockbackPower = new Vector2(1.5f, 2.5f);
    [SerializeField] private Vector2 heavyKnockbackPower = new Vector2(7, 7);
    [SerializeField] private float knockbackDuration = .2f;
    [SerializeField] private float heavyKnockbackDuration = .5f;

    [Header("On Heavy Damage")]
    [SerializeField] private float heavyDamageTreshold = .3f;

    protected virtual void Awake()
    {
        entity = GetComponent<Entity>();
        entityVfx = GetComponent<Entity_VFX>();
        entityStats = GetComponent<Entity_Stats>();
        healthBar = GetComponentInChildren<Slider>();

        currentHealth = entityStats.GetMaxHealth();
        UpdateHealthBar();

        InvokeRepeating(nameof(RegenerateHealth), 0, regenInterval);
    }

    public virtual bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        if (isDead)
            return false;

        if (AttackEvaded())
            return false;

        Entity_Stats attackerStats = damageDealer.GetComponent<Entity_Stats>();
        float armorReduction = attackerStats != null ? attackerStats.GetArmorReduction() : 0;

        float mitigation = entityStats.GetArmorMitigation(armorReduction);
        float physicalDamageTaken = damage * (1 - mitigation);

        float resistance = entityStats.GetElementalResistance(element);
        float elementalDamageTaken = elementalDamage * (1 - resistance);

        TakeKnockback(damageDealer, physicalDamageTaken);
        ReduceHealth(physicalDamageTaken + elementalDamageTaken);

        return true;
    }

    private bool AttackEvaded()
    {
        return Random.Range(0, 100) < entityStats.GetEvasion();
    }

    public void RegenerateHealth()
    {
        if (!canRegenerateHealth || isDead)
            return;

        float regenAmount = entityStats.resources.healthRegen.GetValue();
        IncreaseHealth(regenAmount);
    }

    public void IncreaseHealth(float healAmount)
    {
        if (isDead)
            return;

        float maxHealth = entityStats.GetMaxHealth();
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        UpdateHealthBar();
    }

    public void ReduceHealth(float damage)
    {
        if (isDead)
            return;

        entityVfx?.PlayOnDamageVfx();
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);
        UpdateHealthBar();

        if (currentHealth <= 0)
            Die();
    }

    public void SetCurrentHealth(float value)
    {
        float maxHealth = entityStats.GetMaxHealth();
        currentHealth = Mathf.Clamp(value, 0f, maxHealth);
        isDead = currentHealth <= 0f;
        UpdateHealthBar();
    }

    public void SetMaxHealth(float value, bool refillToFull = true)
    {
        entityStats.SetMaxHealth(value);

        if (refillToFull)
            currentHealth = entityStats.GetMaxHealth();
        else
            currentHealth = Mathf.Clamp(currentHealth, 0f, entityStats.GetMaxHealth());

        isDead = currentHealth <= 0f;
        UpdateHealthBar();
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;
        entity.EntityDeath();
    }

    private void UpdateHealthBar()
    {
        if (healthBar == null)
            return;

        float maxHealth = entityStats.GetMaxHealth();

        healthBar.minValue = 0f;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }

    private void TakeKnockback(Transform damageDealer, float finalDamage)
    {
        Vector2 knockback = CalculateKnockback(finalDamage, damageDealer);
        float duration = CalculateDuration(finalDamage);

        entity?.ReceiveKnockback(knockback, duration);
    }

    private Vector2 CalculateKnockback(float damage, Transform damageDealer)
    {
        int direction = transform.position.x > damageDealer.position.x ? 1 : -1;

        Vector2 knockback = IsHeavyDamage(damage) ? heavyKnockbackPower : knockbackPower;
        knockback.x *= direction;

        return knockback;
    }

    private float CalculateDuration(float damage)
    {
        return IsHeavyDamage(damage) ? heavyKnockbackDuration : knockbackDuration;
    }

    private bool IsHeavyDamage(float damage)
    {
        float maxHealth = entityStats.GetMaxHealth();
        if (maxHealth <= 0f)
            return false;

        return damage / maxHealth > heavyDamageTreshold;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return entityStats.GetMaxHealth();
    }
}
