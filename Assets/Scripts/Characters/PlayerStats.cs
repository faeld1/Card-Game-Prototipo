using System;
using System.Buffers.Text;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    public static Action<PlayerStats, int> OnPlayerHit;
    public static Action OnPlayerLevelUp;

    public static PlayerStats instance;

    [SerializeField] private int currentXP;
    [SerializeField] private int xpToNextLevel;
    private float xpGrowthFactor = 1.25f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    protected override void Start()
    {
        LoadFragmentData();
        base.Start();
        xpToNextLevel = Mathf.RoundToInt(10 * Mathf.Pow(xpGrowthFactor, level - 1));
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void TakeDamage(int _damage)
    {
        OnPlayerHit?.Invoke(this, _damage);
        base.TakeDamage(_damage);
    }

    public void AddXP(int amout)
    {
        currentXP += amout * 2; //2 = Double XP
        CheckLevelUp();
        SaveFragmentData();
    }

    // Método para verificar se é hora de subir de nível
    private void CheckLevelUp()
    {
        while (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    // Método para subir de nível
    private void LevelUp()
    {
        level++;
        currentXP -= xpToNextLevel;  // Reduz o XP necessário para o próximo nível
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * xpGrowthFactor);  // Aumenta o XP necessário com base no fator de crescimento
        player.PlayerFX.PlayLevelUpEffect();
        OnPlayerLevelUp?.Invoke();
        Debug.Log("Level Up! Novo nível: " + level);
    }


    private void OnEnable()
    {
        Enemy_Stats.OnEnemyDie += AddXP;
    }

    private void OnDisable()
    {
        Enemy_Stats.OnEnemyDie -= AddXP;
    }

    private void SaveFragmentData()
    {
        ES3.Save("PlayerLevel", level);
        ES3.Save("PlayerCurrentXP", currentXP);
    }

    private void LoadFragmentData()
    {
        level = ES3.Load("PlayerLevel", defaultValue: 1);
        currentXP = ES3.Load("PlayerCurrentXP", defaultValue: 0);
    }
}
