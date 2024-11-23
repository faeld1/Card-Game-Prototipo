using System;
using UnityEngine;

public class Enemy_Stats : CharacterStats
{
    public static Action<int> OnEnemyDie;
    private DropItem myDropSystem;

    [SerializeField] private int xpReward = 5;

    protected override void Start()
    {
        base.Start();
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
}
