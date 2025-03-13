using System;
using System.Collections;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public static Action<Enemy_Stats, int, bool> OnEnemyHit;

    public static Action OnHealthChanged;

    public Stats damage;
    public Stats critChance;
    public Stats critPower;

    public Stats maxHealth;
    public Stats evasion;
    public Stats armor;

    public int level;
    public int energy = 1;

    public int currentHealth;
    public int shield;

    public Player player;


   protected virtual void Start()
    {
        player = GetComponent<Player>();

        AdjustStatsBasedOnLevel();

        currentHealth = maxHealth.GetValue();

        shield = 0;
    }

    protected virtual void Update()
    {

    }
    public void AdjustStatsBasedOnLevel()
    {
        if (level == 1)
        {
            return; // Nenhuma modificação é aplicada no nível 1
        }

        // Aplica os modificadores de acordo com o nível
        Modify(damage, 10, 10000); // De +10 no level 2 a +10000 no level 100
        Modify(maxHealth, 10, 10000); // De +10 no level 2 a +10000 no level 100
        Modify(armor, 5, 50000); // De +5 no level 2 a +50000 no level 100

       // Debug.Log($"Stats ajustados para o nível {level}. Damage: {damage.GetValue()}, MaxHealth: {maxHealth.GetValue()}, Armor: {armor.GetValue()}");
    }

    private void Modify(Stats _stat, int minGain, int maxGain)
    {

        // Calcula o valor de crescimento acumulativo baseado no level
        float progress = Mathf.Pow((level - 1) / 99f, 1.6f); // Suaviza o progresso inicial com um expoente de 2
        int accumulatedGain = Mathf.RoundToInt(Mathf.Lerp(minGain, maxGain, progress)); // Interpola o valor entre os limites

        // Adiciona o modificador ao stat
        _stat.AddModifiers(accumulatedGain);

    }

    public virtual void DoDamage(CharacterStats targetStats)
    {
        //multiplicador do level
        float damageMultiplier = 1 + (0.1f * (level - 1));

        //int totalDamage =Mathf.RoundToInt(damage.GetValue() * damageMultiplier);

        int totalDamage = damage.GetValue();
        bool crited = false;

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
            crited = true;
        }

        if(TargetCanAvoidAttack(targetStats) && targetStats != null)
        {
            totalDamage = 0;
        }

        OnEnemyHit?.Invoke(player.CurrentEnemyTarget, totalDamage,crited);

        if(player.CurrentEnemyTarget != null)
        {
        targetStats.TakeDamage(totalDamage);
        }
    }

    public virtual void IncreaseShield(int _shield)
    {
        shield += armor.GetValue() * _shield;
        OnHealthChanged?.Invoke();
    }

    public virtual void ResetShield()
    {
        shield = 0;
        OnHealthChanged?.Invoke();
    }

    public virtual void Heal(float _healAmount)
    {
        currentHealth += Mathf.RoundToInt(_healAmount);
        if(currentHealth > maxHealth.GetValue())
        {
            currentHealth = maxHealth.GetValue();
        }

        OnHealthChanged?.Invoke();
    }

    public virtual void IncreasyEnergy(int _energyAmount)
    {
        energy += _energyAmount;
        BattleManager.instance.UpdateEnergyText();
    }

    public virtual void UpdateHealth()
    {
        StartCoroutine(UpdateHealthWithDelay());
    }

    private IEnumerator UpdateHealthWithDelay()
    {
        yield return new WaitForSeconds(0.001f);
        currentHealth = maxHealth.GetValue();
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

    public bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {

        int totalEvasion = _targetStats.evasion.GetValue();

        if (UnityEngine.Random.Range(0, 100) < totalEvasion)
        {
            //Debug.Log("Attack avoided");
            return true;
        }
        return false;
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
