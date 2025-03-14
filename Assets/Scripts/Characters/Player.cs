using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public Animator animator;

    public CharacterStats Stats { get; set; }
    public PlayerStats PlayerStats_ { get; set; }
    public PlayerFX PlayerFX { get; set; }

    [SerializeField] private float attackRange = 3f;

    public Enemy_Stats CurrentEnemyTarget { get; set; }

    public float AttackRange => attackRange;
    private bool gameStarted;

    [Header("Player Attacking")]
    private bool isAttacking = false;
    public bool isAttackingAnimation = false;
    private Queue<Enemy_Stats> attackQueue = new Queue<Enemy_Stats>();
    private int remainingAttacks = 0;

    [SerializeField] private List<Enemy_Stats> _enemies;

    private void Start()
    {
        Stats = GetComponent<CharacterStats>();
        PlayerStats_ = GetComponent<PlayerStats>();
        PlayerFX = GetComponent<PlayerFX>();
        animator = GetComponentInChildren<Animator>();
        _enemies = new List<Enemy_Stats>();

        StartCoroutine(GameStartedDelay());

    }

    private void Update()
    {
        if (CurrentEnemyTarget == null || CurrentEnemyTarget.currentHealth <= 0)
        {
            GetCurrentEnemyTarget();
        }
    }

    public void SetTargetEnemy(Enemy_Stats targetEnemy)
    {
        if (targetEnemy != null && targetEnemy.currentHealth > 0)
        {
            CurrentEnemyTarget = targetEnemy;
        }
    }

    private void PlayerUseAttack(Enemy_Stats targetEnemy, CardData cardData)
    {
        if (isAttacking) return; // Bloqueia novas cartas enquanto estiver atacando

        if (CurrentEnemyTarget != null)
        {
            SetTargetEnemy(targetEnemy);

            attackQueue.Enqueue(targetEnemy);
            remainingAttacks += cardData.attackValue;

            isAttacking = true; // Bloqueia cartas até terminar os ataques
            isAttackingAnimation = true;
            UI_Manager.instance.ShowBlockHandCards();
            StartNextAttack();
        }
    }
    private void StartNextAttack()
    {
        Debug.Log("StartNextAttack chamado. RemainingAttacks: " + remainingAttacks + " | Queue Size: " + attackQueue.Count);
        if (remainingAttacks > 0 && attackQueue.Count > 0)
        {
            Enemy_Stats target = attackQueue.Peek();

            if (target == null || target.currentHealth <= 0)
            {

                attackQueue.Dequeue();
                GetCurrentEnemyTarget();

                if (CurrentEnemyTarget != null && CurrentEnemyTarget.currentHealth > 0)
                {
                    attackQueue.Enqueue(CurrentEnemyTarget);
                    target = CurrentEnemyTarget;
                    //Debug.Log("Terceiro if do StartnextAttack chamado");
                }
               // Debug.Log("Segundo if do StartnextAttack chamado");
            }

            if (target != null && target.currentHealth > 0)
            {
                animator.SetBool("IsAttacking", true);

                // Escolhe um número aleatório entre 1 e 3
                int randomAttack = UnityEngine.Random.Range(1, 4);

                // Ativa um dos três triggers
                animator.SetTrigger("Attack_" + randomAttack);
            }
            else
            {
                isAttacking = false;
                animator.SetBool("IsAttacking", false);
                //Debug.Log("HideBlockHandCards sendo chamado no StartNextAttack");
                UI_Manager.instance.HideBlockHandCards();
            }
        }
        else
        {
            isAttacking = false;
            animator.SetBool("IsAttacking", false);
           // Debug.Log("RemainingAttacks <= 0 no StartNextAttack");
        }

    }


    // Esta função será chamada no fim da animação via Animation Event
    public void OnAttackAnimationEnd()
    {

        isAttackingAnimation = true;
        if (remainingAttacks <= 0)
        {
            //Debug.Log("HideBlockHandCards sendo chamado no remaingAttack = 0 do OnAttackAnimationEnd");
            UI_Manager.instance.HideBlockHandCards();
            isAttacking = false;
            isAttackingAnimation = false;
            animator.SetBool("IsAttacking", false);
            return;
        }

        // Remove o inimigo morto da lista
        if (CurrentEnemyTarget != null && CurrentEnemyTarget.currentHealth <= 0)
        {
            _enemies.Remove(CurrentEnemyTarget);
        }

        GetCurrentEnemyTarget();

        // Se ainda temos um inimigo válido, atacar
        if (CurrentEnemyTarget != null && CurrentEnemyTarget.currentHealth > 0)
        {
            Stats.DoDamage(CurrentEnemyTarget);
            remainingAttacks--;

            // Se ainda há ataques restantes, continuar
            if (remainingAttacks > 0)
            {
                StartNextAttack();
            }
            else
            {
                //Debug.Log("PlayerCurrentEnergy: " + BattleManager.instance.PlayerCurrentEnergy);
                if (BattleManager.instance.PlayerCurrentEnergy > 0)
                {
                    //Debug.Log("HideBlockHandCards sendo chamado no primeiro else do OnAttackAnimationEnd");
                    UI_Manager.instance.HideBlockHandCards();
                }
                else
                    UI_Manager.instance.ShowBlockHandCards();

                isAttacking = false;
                animator.SetBool("IsAttacking", false);
            }
        }
        else
        {
            //Debug.Log("HideBlockHandCards sendo chamado no segundo else do OnAttackAnimationEnd");
            UI_Manager.instance.HideBlockHandCards();
            isAttacking = false;
            animator.SetBool("IsAttacking", false);
        }

        if (remainingAttacks <= 0)
        {
            isAttackingAnimation = false;
        }
    }
    public void OnHitAnimationEnd()
    {
        BattleManager.instance.OnPlayerHitAnimationEnd();
    }
    public void OnSupportAnimationEnd()
    {
        //Debug.Log("HideBlockHandCards sendo chamado no OnSupportAnimationEnd");
        UI_Manager.instance.HideBlockHandCards();
    }

    private void PlayerUseDefenseOrSupport(CardData cardData)
    {
        if (cardData.cardType == CardType.Defense)
        {
            Stats.IncreaseShield(cardData.defenseValue);
            UI_Manager.instance.ShowBlockHandCards();
            animator.SetTrigger("Shield");
        }
        else if (cardData.cardType == CardType.Support)
        {
            if (cardData.grantsEnergy)
            {
                Stats.IncreasyEnergy(cardData.energyGranted);
                //Debug.Log("HideBlockHandCards sendo chamado no PlayerUseDefenseOrSupport");
                UI_Manager.instance.HideBlockHandCards();
            }

            if (cardData.healsPlayer)
            {
                HealPlayer(cardData);
            }
        }
        else if (cardData.cardType == CardType.Attack)
        {
            GetCurrentEnemyTarget();
            PlayerUseAttack(CurrentEnemyTarget, cardData);
        }
    }

    private void HealPlayer(CardData cardData)
    {
        animator.SetTrigger("Heal");
        float healAmount = Stats.maxHealth.GetValue() * cardData.healPercentage / 100f;
        Stats.Heal(healAmount);
        PlayerFX.PlayHealEffect();
    }

    private IEnumerator GameStartedDelay()
    {
        yield return new WaitForSeconds(.2f);
        gameStarted = true;
    }

    private void GetCurrentEnemyTarget()
    {
        // Remove inimigos nulos ou mortos
        _enemies.RemoveAll(enemy => enemy == null || enemy.currentHealth <= 0);

        if (_enemies.Count <= 0 && gameStarted)
        {
            CurrentEnemyTarget = null;
            BattleManager.instance.VerifyPlayerIsAlive();
            return;
        }

        if (CurrentEnemyTarget != null && CurrentEnemyTarget.currentHealth > 0)
        {
            return;
        }

        Enemy_Stats closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (var enemy in _enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemy;
            }
        }

        CurrentEnemyTarget = closestEnemy;
    }

    private void OnTriggerEnter(Collider collision)
    {

        if (collision.CompareTag("Enemy"))
        {
            Enemy_Stats newEnemy = collision.GetComponent<Enemy_Stats>();
            if (newEnemy != null && !_enemies.Contains(newEnemy))
            {
                _enemies.Add(newEnemy);
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy_Stats newEnemy = collision.GetComponent<Enemy_Stats>();

            if (newEnemy != null && _enemies.Contains(newEnemy))
            {
                _enemies.Remove(newEnemy);
            }
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

        SphereCollider sphereCollider = GetComponent<SphereCollider>();

        sphereCollider.radius = attackRange;

        if (sphereCollider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position + sphereCollider.center, attackRange);
        }
    }

}
