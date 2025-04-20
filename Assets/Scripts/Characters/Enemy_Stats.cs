using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyTurn
{
    public EnemyAction[] actions;
}
public class Enemy_Stats : CharacterStats
{
    public static Action<int> OnEnemyDie;
    private DropItem myDropSystem;
    public Enemy enemy;

    [Header("Actions")]
    public List<EnemyTurn> actionSequence = new List<EnemyTurn>();
    private int currentTurnIndex = 0;

    [SerializeField] private int xpReward = 5;

    protected override void Start()
    {
        base.Start();
        enemy = GetComponent<Enemy>();
        myDropSystem = GetComponent<DropItem>();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);

        if (currentHealth <= 0)
        {
            myDropSystem.GenerateDrop();
            OnEnemyDie?.Invoke(xpReward);
        }
    }

    public EnemyAction[] GetCurrentTurnActions()
    {
        if (actionSequence.Count == 0)
            return Array.Empty<EnemyAction>();

        if (currentTurnIndex >= actionSequence.Count)
            currentTurnIndex = 0;

        return actionSequence[currentTurnIndex].actions;
    }

    public void AdvanceTurnAction()
    {
        currentTurnIndex++;
        if (currentTurnIndex >= actionSequence.Count)
            currentTurnIndex = 0;
    }
}
