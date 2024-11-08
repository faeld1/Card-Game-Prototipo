using System;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public static Action<Enemy_Stats, int, bool> OnEnemyHit;

    public static Action OnHealthChanged;

    public Stats damage;
    public Stats critChance;
    public Stats critPower;

    public Stats maxHealth;

    public int level;
    public int energy = 1;

    public int currentHealth;
    public int shield;

    public Player player;


   protected virtual void Start()
    {
        player = GetComponent<Player>();

        currentHealth = maxHealth.GetValue();
        shield = 0;
    }

    protected virtual void Update()
    {

    }

    public virtual void DoDamage(CharacterStats targetStats)
    {
        //multiplicador do level
        float damageMultiplier = 1 + (0.1f * (level - 1));

        int totalDamage =Mathf.RoundToInt(damage.GetValue() * damageMultiplier);
        bool crited = false;

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
            crited = true;
        }

        OnEnemyHit?.Invoke(player.CurrentEnemyTarget, totalDamage,crited);

        if(player.CurrentEnemyTarget != null)
        {
        targetStats.TakeDamage(totalDamage);
        }
    }

    public virtual void IncreaseShield(int _shield)
    {
        shield += _shield;
        OnHealthChanged?.Invoke();
    }

    public virtual void TakeDamage(int _damage)
    {

        int remainingDamage = _damage;

        if (shield > 0)
        {
            if (shield >= remainingDamage)
            {
                shield -= remainingDamage;
                remainingDamage = 0;
            }
            else
            {
                remainingDamage -= shield;
                shield = 0;
            }
        }

        if (remainingDamage > 0)
        {
            currentHealth -= remainingDamage;
        }


        OnHealthChanged?.Invoke();
    }

    protected bool CanCrit()
    {
        if (UnityEngine.Random.Range(0, 100) <= critChance.GetValue())
        {
            return true;
        }
        return false;
    }

    protected int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue()  * .01f);

        float critDamage = _damage * totalCritPower;

        return Mathf.RoundToInt(critDamage);
    }
}
