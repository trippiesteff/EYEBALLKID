using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Entity_VFX : MonoBehaviour
{
    private SpriteRenderer sr;
    private Entity entity;
    
    [Header("On Taking Damage VFX")]
    [SerializeField] private Material onDamageMaterial;
    [SerializeField] private float onDamageVfxDuration = .2f;
    private Material originalMaterial;
    private Coroutine onDamageVfxCoroutine;

    [Header("On Doing Damage VFX")]
    [SerializeField] private Color hitVfxColor = Color.white;
    [SerializeField] private GameObject hitVfx;
    [SerializeField] private GameObject critHitVfx;

    [Header("Element Colors")]
    [SerializeField] private Color chillVfx = Color.cyan;
    [SerializeField] private Color burnVfx = Color.red;
    [SerializeField] private Color electrifyVfx = Color.yellow;
    private Color originalHitVfxColor;

    [Header("Afterimage Settings")]
    [SerializeField] private float afterimageInterval = 0.05f;
    [SerializeField] private float afterimageDuration = 0.3f;
    [SerializeField] private Color afterimageColor = new Color(0.5f, 0.8f, 1f, 0.6f);
    private bool isSpawningAfterimages;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = sr.material;
        originalHitVfxColor = hitVfxColor;
    }

    public void StartAfterimages()
    {
        if (!isSpawningAfterimages)
            StartCoroutine(SpawnAfterimagesCo());
    }

    public void StopAfterimages()
    {
        isSpawningAfterimages = false;
    }

    private IEnumerator SpawnAfterimagesCo()
    {
        isSpawningAfterimages = true;

        while (isSpawningAfterimages)
        {
            SpawnAfterimage();
            yield return new WaitForSeconds(afterimageInterval);
        }
    }

    private void SpawnAfterimage()
    {
        if (sr == null)
            return;

        GameObject ghost = new GameObject("Afterimage");
        ghost.transform.position = transform.position;
        ghost.transform.rotation = transform.rotation;
        ghost.transform.localScale = transform.localScale;

        SpriteRenderer ghostSprite = ghost.AddComponent<SpriteRenderer>();
        ghostSprite.sprite = sr.sprite;
        ghostSprite.color = afterimageColor;
        ghostSprite.sortingLayerName = sr.sortingLayerName;
        ghostSprite.sortingOrder = sr.sortingOrder - 1;

        StartCoroutine(FadeAfterimage(ghostSprite));
    }

    private IEnumerator FadeAfterimage(SpriteRenderer ghostSprite)
    {
        float elapsed = 0f;
        Color startColor = ghostSprite.color;

        while (elapsed < afterimageDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsed / afterimageDuration);
            ghostSprite.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        Destroy(ghostSprite.gameObject);
    }

    public void PlayOnStatusVfx(float duration, ElementType element)
    {
        if (element == ElementType.Ice)
            StartCoroutine(PlayStatusVfxCo(duration, chillVfx));

        if (element == ElementType.Fire)
            StartCoroutine(PlayStatusVfxCo(duration, burnVfx));

        if (element == ElementType.Lightning)
            StartCoroutine(PlayStatusVfxCo(duration, electrifyVfx));
    }

    public void StopAllVfx()
    {
        StopAllCoroutines();
        isSpawningAfterimages = false;
        sr.color = Color.white;
        sr.material = originalMaterial;
    }

    private IEnumerator PlayStatusVfxCo(float duration, Color effectColor)
    {
        float tickInterval = .25f;
        float timeHasPassed = 0;

        Color lightColor = effectColor * 1.2f;
        Color darkColor = effectColor * .8f;

        bool toggle = false;

        while (timeHasPassed < duration)
        {
            sr.color = toggle ? lightColor : darkColor;
            toggle = !toggle;

            yield return new WaitForSeconds(tickInterval);

            timeHasPassed = timeHasPassed + tickInterval;
        }
        sr.color = Color.white;
    }

    public void UpdateOnHitColor(ElementType element)
    {
        if (element == ElementType.Ice)
            hitVfxColor = chillVfx;

        if (element == ElementType.None)
            hitVfxColor = originalHitVfxColor;
    }

    public void CreateOnHitVFX(Transform target, bool isCrit)
    {
        GameObject hitPrefab = isCrit ? critHitVfx : hitVfx;
        GameObject vfx = Instantiate(hitPrefab, target.position, Quaternion.identity);
        vfx.GetComponentInChildren<SpriteRenderer>().color = hitVfxColor;

        if (entity.facingDir == -1 && isCrit)
            vfx.transform.Rotate(0, 180, 0);
    }

    public void PlayOnDamageVfx()
    {
        if (onDamageVfxCoroutine != null)
        {
            StopCoroutine(onDamageVfxCoroutine);
        }

        onDamageVfxCoroutine = StartCoroutine(OnDamageVfxCo());
    }

    private IEnumerator OnDamageVfxCo()
    {
        sr.material = onDamageMaterial;
        yield return new WaitForSeconds(onDamageVfxDuration);
        sr.material = originalMaterial;
    }
}