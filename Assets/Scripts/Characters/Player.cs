using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private Animator animator;

    public CharacterStats Stats { get; set; }
    public PlayerStats PlayerStats_ { get; set; }

    [SerializeField] private float attackRange = 3f;

    public Enemy_Stats CurrentEnemyTarget { get; set; }

    public float AttackRange => attackRange;
    private bool gameStarted;

   [SerializeField] private List<Enemy_Stats> _enemies;

    private void Start()
    {
        Stats = GetComponent<CharacterStats>();
        PlayerStats_ = GetComponent<PlayerStats>();
        animator = GetComponent<Animator>();
        _enemies = new List<Enemy_Stats>();

        StartCoroutine(GameStartedDelay());

    }

    private void Update()
    {

        GetCurrentEnemyTarget();
   
    }

    private void PlayerUseAttack(Enemy_Stats targetEnemy, CardData cardData)
    {
        if (CurrentEnemyTarget != null)
        {
            if (cardData.cardType == CardType.Attack)
            {
                //Stats.DoDamage(CurrentEnemyTarget, cardData.attackValue);
                StartCoroutine(ExecuteMultipleAttacks(targetEnemy,cardData.attackValue));
            }
        }  
    }
    private void PlayerUseDefenseOrSupport(CardData cardData)
    {
        if (cardData.cardType == CardType.Defense)
        {
            Stats.IncreaseShield(cardData.defenseValue);
        }
        else if (cardData.cardType == CardType.Support)
        {
            if (cardData.grantsEnergy)
            {
                Stats.IncreasyEnergy(cardData.energyGranted);
            }

            if (cardData.healsPlayer)
            {
                float healAmount = Stats.maxHealth.GetValue() * cardData.healPercentage / 100f;
                Stats.Heal(healAmount);
            }
        }
        else if(cardData.cardType == CardType.Attack)
        {
            GetCurrentEnemyTarget();
            PlayerUseAttack(CurrentEnemyTarget, cardData);
        }
    }

    private IEnumerator ExecuteMultipleAttacks(Enemy_Stats initialTarget, int attackCount)
    {
        Enemy_Stats currentTarget = initialTarget;
        for (int i = 0; i < attackCount; i++)
        {
            if (currentTarget != null && currentTarget.currentHealth > 0)
            {
                Stats.DoDamage(currentTarget);
                animator.SetTrigger("Attack");
            }
            else
            {
                GetCurrentEnemyTarget(); // Atualiza para o inimigo mais pr�ximo
                currentTarget = CurrentEnemyTarget;

                if (currentTarget == null) // Sem inimigos restantes
                {
                    Debug.Log("Todos os inimigos foram derrotados!");
                    break;
                }
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator GameStartedDelay()
    {
        yield return new WaitForSeconds(.2f);
        gameStarted = true;
    }


    private void GetCurrentEnemyTarget()
    {
        if(_enemies.Count <= 0 && gameStarted)
        {
            CurrentEnemyTarget = null;
            BattleManager.instance.VerifyPlayerIsAlive();
            return;
        }

        Enemy_Stats closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (var enemy in _enemies)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemy;
            }
        }

        CurrentEnemyTarget = closestEnemy;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy_Stats newEnemy = collision.GetComponent<Enemy_Stats>();
            _enemies.Add(newEnemy);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
       if( collision.CompareTag("Enemy"))
        {
            Enemy_Stats newEnemy = collision.GetComponent<Enemy_Stats>();

            if(_enemies.Contains(newEnemy))
            _enemies.Remove(newEnemy);
        }
    }

    private void OnEnable()
    {
        DeckManager.OnCardUsed += PlayerUseDefenseOrSupport;
        DeckManager.OnCardAttackUsed += PlayerUseAttack;
    }

    private void OnDisable()
    {
        DeckManager.OnCardUsed -= PlayerUseDefenseOrSupport;
        DeckManager.OnCardAttackUsed -= PlayerUseAttack;
    }

    private void OnDrawGizmos()
    {
        GetComponent<CircleCollider2D>().radius = attackRange;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }

}
