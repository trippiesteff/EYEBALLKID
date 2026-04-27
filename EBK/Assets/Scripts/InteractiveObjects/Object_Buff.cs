using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;


[System.Serializable]
public class Buff
{
    public StatType type;
    public float value;
}

public class Object_Buff : MonoBehaviour
{
    private SpriteRenderer sr;
    private Entity_Stats statsToModify;

[Header("Buff Details")]
[SerializeField] private Buff[] buffs;
[SerializeField] private string buffName;

[SerializeField] private float buffDuration=4f;
[SerializeField] private bool canBeUsed = true;

[Header("Wiggle")]
[SerializeField] private float floatSpeed=1f;
[SerializeField] private float floatRange=.1f;
private Vector3 startposition;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        startposition = transform.position;
    }

    private void Update()
{
    float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatRange;
    transform.position = startposition + new Vector3(0, yOffset);
}

private void OnTriggerEnter2D(Collider2D collision)

{
    if(canBeUsed == false)
        return;
    statsToModify = collision.GetComponent<Entity_Stats>();
    StartCoroutine(BuffCo(buffDuration));
}

private IEnumerator BuffCo(float duration)
    {
        canBeUsed = false;
        sr.color = Color.clear;
        applybuff(true);

        yield return new WaitForSeconds(duration);

        applybuff(false);
        Destroy(gameObject);
    }

    private void applybuff(bool apply)
    {
        foreach(var buff in buffs)
        {
            if(apply)
            statsToModify.GetStatByType(buff.type).AddModifier(buff.value, buffName);
            else
            statsToModify.GetStatByType(buff.type).RemoveModifier(buffName);
        }
        
    }
}
