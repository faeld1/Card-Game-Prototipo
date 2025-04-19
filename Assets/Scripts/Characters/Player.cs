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

    [Header("Special Attacks")]
    private Queue<Enemy_Stats> specialAttackQueue = new Queue<Enemy_Stats>();
    private int remainingSpecialAttacks = 0;
    private bool isSpecialAttacking = false;

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

    #region Attack from Cards
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
        //Debug.Log("StartNextAttack chamado. RemainingAttacks: " + remainingAttacks + " | Queue Size: " + attackQueue.Count);
        if (remainingAttacks > 0 && attackQueue.Count > 0)
        {
            UI_Manager.instance.endTurnButton.interactable = false;
            UI_Manager.instance.specialAttackButton.interactable = false;
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
        if (!isAttacking)
            return;

        isAttackingAnimation = true;
        if (remainingAttacks <= 0)
        {
            //Debug.Log("HideBlockHandCards sendo chamado no remaingAttack = 0 do OnAttackAnimationEnd");
            if (Stats.energy > 0)
                UI_Manager.instance.HideBlockHandCards();

            isAttacking = false;
            isAttackingAnimation = false;
            animator.SetBool("IsAttacking", false);
            UI_Manager.instance.endTurnButton.interactable = true;
            if (BattleManager.instance.rageStacks >= 3)
                UI_Manager.instance.specialAttackButton.interactable = true;
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
            GainRage();
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
                    Debug.Log("HideBlockHandCards sendo chamado no primeiro else do OnAttackAnimationEnd");
                    UI_Manager.instance.HideBlockHandCards();
                }
                else
                {
                    UI_Manager.instance.ShowBlockHandCards();
                }

                //UI_Manager.instance.endTurnButton.interactable = true;
                isAttacking = false;
                animator.SetBool("IsAttacking", false);
            }
        }
        else
        {
            Debug.Log("HideBlockHandCards sendo chamado no segundo else do OnAttackAnimationEnd");
            UI_Manager.instance.HideBlockHandCards();
            isAttacking = false;
            animator.SetBool("IsAttacking", false);
        }

        if (remainingAttacks <= 0)
        {
            isAttackingAnimation = false;
            UI_Manager.instance.endTurnButton.interactable = true;
            if(BattleManager.instance.rageStacks >= 3)
                UI_Manager.instance.specialAttackButton.interactable = true;
        }
    }
    #endregion 
    public void OnHitAnimationEnd()
    {
        BattleManager.instance.OnPlayerHitAnimationEnd();
    }
    public void OnSupportAnimationEnd()
    {
        if (Stats.energy > 0)
            UI_Manager.instance.HideBlockHandCards();
    }

    private void PlayerUseDefenseOrSupport(CardData cardData)
    {
        if (cardData.cardType == CardType.Defense)
        {
            Debug.Log("Carta de defesa sendo usada");
            Stats.IncreaseShield(cardData.defenseValue);
            UI_Manager.instance.ShowBlockHandCards();
            animator.SetTrigger("Shield");
        }
        else if (cardData.cardType == CardType.Support)
        {
            if (cardData.grantsEnergy)
            {
                Stats.IncreasyEnergy(cardData.energyGranted);
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

    #region Special Attack

    // Chamado sempre que o player ataca
    public void GainRage()
    {
        BattleManager.instance.rageStacks++;
        UI_Manager.instance.UpdateRageUI(BattleManager.instance.rageStacks); // Atualiza UI
    }

    // Chamado quando o botão de ataque especial é pressionado
    public void UseSpecialAttack()
    {
        int rageStacks = BattleManager.instance.rageStacks;

        if (rageStacks >= 3) // Se a raiva for maior ou igual a 3
        {
            remainingSpecialAttacks = rageStacks; // Define a quantidade de ataques
            //rageStacks = 0; // Reseta a raiva
            BattleManager.instance.rageStacks = 0;
            UI_Manager.instance.UpdateRageUI(BattleManager.instance.rageStacks);

            isSpecialAttacking = true;
            specialAttackQueue.Clear();

            // Adiciona o inimigo atual na fila
            if (CurrentEnemyTarget != null && CurrentEnemyTarget.currentHealth > 0)
            {
                specialAttackQueue.Enqueue(CurrentEnemyTarget);
            }

            StartNextSpecialAttack();
        }
        else
        {
            Debug.Log("Raiva insuficiente para ataque especial!");
        }
    }
    private void StartNextSpecialAttack()
    {
        UI_Manager.instance.endTurnButton.interactable = false;
        if (remainingSpecialAttacks > 0 && specialAttackQueue.Count > 0)
        {
            Enemy_Stats target = specialAttackQueue.Peek();

            if (target == null || target.currentHealth <= 0)
            {
                specialAttackQueue.Dequeue();
                GetCurrentEnemyTarget();
                if (CurrentEnemyTarget != null && CurrentEnemyTarget.currentHealth > 0)
                {
                    specialAttackQueue.Enqueue(CurrentEnemyTarget);
                    target = CurrentEnemyTarget;
                }
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
                isSpecialAttacking = false;
                animator.SetBool("IsAttacking", false);
            }
        }
        else
        {
            isSpecialAttacking = false;
            animator.SetBool("IsAttacking", false);
            UI_Manager.instance.endTurnButton.interactable = true;
        }
    }

    public void OnSpecialAttackAnimationEnd()
    {
        if (!isSpecialAttacking)
            return;

        if (remainingSpecialAttacks <= 0)
        {
            isSpecialAttacking = false;
            animator.SetBool("IsAttacking", false);
            UI_Manager.instance.endTurnButton.interactable = true;

            if (Stats.energy > 0)
            {
                Debug.Log("Player energy: " + Stats.energy);
                UI_Manager.instance.HideBlockHandCards();

            }
            else
                UI_Manager.instance.ShowBlockHandCards();

            return;
        }

        // Aplica dano ao inimigo atual
        if (CurrentEnemyTarget != null && CurrentEnemyTarget.currentHealth > 0)
        {
            Stats.DoDamage(CurrentEnemyTarget);
        }

        remainingSpecialAttacks--;
        StartNextSpecialAttack();
    }


    #endregion

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

    #region GET ENEMY TARGET
    public void SetTargetEnemy(Enemy_Stats targetEnemy)
    {
        if (targetEnemy != null && targetEnemy.currentHealth > 0)
        {
            CurrentEnemyTarget = targetEnemy;
        }
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

    #endregion

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
