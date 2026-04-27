using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Entity_StatusHandler : MonoBehaviour
{
  private Entity entity;
  private Entity_VFX entityVfx;
  private Entity_Stats entityStats;
  private Entity_Health entityHealth;
  private ElementType currentEffect = ElementType.None;

  [Header("Electrify Effect Details")]
  [SerializeField] private GameObject lightningStrikeVfx;
  [SerializeField] private float currentCharge;
  [SerializeField] private float maximumCharge = 1;
  private Coroutine electrifyCo;

  private void Awake()
  
  {
    entityVfx = GetComponent<Entity_VFX>();
    entity = GetComponent<Entity>();
    entityStats = GetComponent<Entity_Stats>();
    entityHealth = GetComponent<Entity_Health>();
  }

  public void ApplyElectrifyEffect(float duration, float damage, float charge)
    {
      float lightningResistance = entityStats.GetElementalResistance(ElementType.Lightning);
      float finalCharge = charge * (1 - lightningResistance);

      currentCharge = currentCharge + finalCharge;

      if(currentCharge >= maximumCharge)
        {
            DoLightningStrike(damage);
            StopElectrifyEffect();
            return;
        }

        if(electrifyCo != null)
        {
            StopCoroutine(electrifyCo);
        }

        electrifyCo = StartCoroutine(ElectrifyEffectCo(duration));
    }

    private void StopElectrifyEffect()
  {
    currentEffect = ElementType.None;
    currentCharge = 0;
    entityVfx.StopAllVfx();
  }


    private void DoLightningStrike(float damage)
    {
        Instantiate(lightningStrikeVfx, transform.position, Quaternion.identity);
        entityHealth.ReduceHealth(damage);
    }

    private IEnumerator ElectrifyEffectCo(float duration)
    {
      currentEffect = ElementType.Lightning;
      entityVfx.PlayOnStatusVfx(duration, ElementType.Lightning);

      yield return new WaitForSeconds(duration);
      StopElectrifyEffect();
    }

    public void ApplyBurnEffectCo(float duration, float fireDamage)
  {
    float fireResistance = entityStats.GetElementalResistance(ElementType.Fire);

    float finalDamage = fireDamage * (1 - fireResistance);
    StartCoroutine(BurnEffectCo(duration, finalDamage));
  }

private IEnumerator BurnEffectCo(float duration, float totalDamage)
    {
        currentEffect = ElementType.Fire;
        entityVfx.PlayOnStatusVfx(duration, ElementType.Fire);
        int tickersPerSecond = 2;
        int tickcount = Mathf.RoundToInt(tickersPerSecond * duration);

        float damagePerTick = totalDamage / tickcount;
        float tickInterval = 1f / tickersPerSecond;

        for(int i = 0; i < tickcount; i++)
    {
       entityHealth.ReduceHealth(damagePerTick);
        yield return new WaitForSeconds(tickInterval);
    }
        currentEffect = ElementType.None;

    }

  
  public void ApplyChillEffect(float duration, float slowMultiplier)
    {

      float iceResistance = entityStats.GetElementalResistance(ElementType.Ice);
      float finalDuration = duration *(1-iceResistance);

        StartCoroutine(ChillEffectCo(finalDuration, slowMultiplier));
    }


    private IEnumerator ChillEffectCo(float duration, float slowMultiplier)
    {
        entity.SlowDownEntity(duration, slowMultiplier);
        currentEffect = ElementType.Ice;
        entityVfx.PlayOnStatusVfx(duration, ElementType.Ice);

        yield return new WaitForSeconds(duration);

        currentEffect = ElementType.None;
    }

  public bool CanBeApplied(ElementType element)
    {
      if (element == ElementType.Lightning && currentEffect == ElementType.Lightning)
      return true;

        return currentEffect == ElementType.None;
    }
}
