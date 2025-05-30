using System.Collections;
using TMPro;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    public CharacterStats playerStats;
    public CharacterStats[] enemies;

    [Header("Special")]
    public int rageStacks = 0; //Contador de Raiva

    public TextMeshProUGUI whoTurnText;

    private bool playerTurn = true;
    int gameOverOnce = 1;

    private int playerBaseEnergy;
    public int PlayerCurrentEnergy => playerStats.energy;
    private int startShield = 0;

    private bool isPlayerTakingHit = false;
    [SerializeField] private bool autoEndTurn = false;

    [SerializeField] private TextMeshProUGUI energyText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Time.timeScale = 1f; // Garante que o jogo est� despausado ao carregar a cena
        playerBaseEnergy = playerStats.energy;

        StartCoroutine(StartBattleDelay()); // Inicia a batalha com um pequeno atraso
    }

    private IEnumerator StartBattleDelay()
    {
        yield return new WaitForSeconds(.1f);
        StartTurn();
    }

    private void StartTurn()
    {
        if (playerTurn)
        {
            whoTurnText.text = "Turno do Player!";
            UI_Manager.instance.MoveWhoTurn(true, 0);
            DeckManager.instance.ShowHand();
            UI_Manager.instance.endTurnButton.interactable = true;
            // UI_Manager.instance.HideBlockHandCards();
            playerStats.energy = playerBaseEnergy; //Reseta energia

            startShield++;

            if (startShield > 1)
                playerStats.ResetShield(); //Reseta escudo

            if (rageStacks >= 3)
            {
                rageStacks /= 2; // Reduz pela metade (arredondando para baixo)
                UI_Manager.instance.UpdateRageUI(rageStacks);
            }

            // Mostra as a��es futuras dos inimigos (cloud thinking)
            foreach (CharacterStats enemy in enemies)
            {
                if (enemy == null || enemy.currentHealth <= 0) continue;
                Enemy_Stats eStats = enemy as Enemy_Stats;
                EnemyFX fx = enemy.GetComponent<EnemyFX>();
                EnemyAction[] nextActions = eStats.GetCurrentTurnActions();
                fx.ShowNextAction(nextActions);
            }
        }
        else
        {
            DeckManager.instance.HideHand(); // Esconde a m�o do jogador
            // Reset a armadura do inimigo
            foreach (CharacterStats enemy in enemies)
            {
                if (enemy == null || enemy.currentHealth <= 0) continue;
                enemy.ResetShield();
            }
            whoTurnText.text = "Turno do Enemy!";
            playerStats.energy = 0;
            UI_Manager.instance.endTurnButton.interactable = false;
            UI_Manager.instance.MoveWhoTurn(true, 0);
            UI_Manager.instance.DisableArrow();
            UI_Manager.instance.ResetSpecialEffects();
            //DeckManager.instance.HideHand();
            UI_Manager.instance.ShowBlockHandCards();
            StartCoroutine(DiscardHandDelay());
            StartCoroutine(EnemyTurn());
        }

        UpdateEnergyText();
    }

    private IEnumerator DiscardHandDelay()
    {
        yield return new WaitForSeconds(1f);
        DeckManager.instance.DiscardHand();
    }

    private void EndTurn()
    {
        if (playerTurn)
            UI_Manager.instance.ShowBlockHandCards();

        StartCoroutine(DelayEndTurn());
    }

    private IEnumerator DelayEndTurn()
    {
        Debug.Log("IsAttackingAnimation(DelayEndTurn): " + playerStats.player.isAttackingAnimation);
        yield return new WaitUntil(() => !playerStats.player.isAttackingAnimation); // Aguarda ataque terminar

        yield return new WaitForSeconds(1f);

        if (!playerTurn)
            UI_Manager.instance.HideBlockHandCards();

        playerTurn = !playerTurn;

        StartTurn();
    }

    public void ForceEndTurn() //EndTurn no Botao
    {
        if (playerTurn)
        {
            EndTurn();
            UI_Manager.instance.DisableArrow();
        }
    }

    private IEnumerator EnemyTurn()
    {
        UI_Manager.instance.ShowBlockHandCards();
        yield return new WaitForSeconds(0.9f);

        for (int i = 0; i < enemies.Length; i++)
        {
            CharacterStats enemy = enemies[i];

            /* if (enemy != null)
             {
                 EnemyFX fx = enemy.GetComponent<EnemyFX>();
                 fx.ShowTurnSelectionEffect(true); // Ativa o efeito de sele��o do inimigo
             }*/

            if (enemy == null || enemy.currentHealth <= 0)
                continue;

            Enemy_Stats eStats = enemy as Enemy_Stats;
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            EnemyFX fx = enemy.GetComponent<EnemyFX>();

            fx.ShowTurnSelectionEffect(true);


            yield return new WaitForSeconds(0.6f); // Delay entre os ataques dos inimigos

            EnemyAction[] actions = eStats.GetCurrentTurnActions();

            /* if (enemy.currentHealth > 0 && enemy != null)
             {
                 int enemyDamage = enemy.damage.GetValue(); // Dano do inimigo

                 if (playerStats.TargetCanAvoidAttack(playerStats)) // Se o player esquivar
                 {
                     enemyDamage = 0;
                 }

                 PlayerAnimationsOnEnemyTurn(enemyDamage);

                 isPlayerTakingHit = true;

                 Enemy enemyAnimator = enemy.GetComponent<Enemy>();
                 enemyAnimator.enemyAnim.SetTrigger("Attack"); // Anima��o de ataque do inimigo

                 yield return new WaitForSeconds(0.2f); // Delay para o hit DamageText aparecer
                 playerStats.TakeDamage(enemyDamage); // Aplica o dano
                 VerifyPlayerIsAlive();



                 yield return new WaitUntil(() => !isPlayerTakingHit); // Espera a anima��o terminar

             }
             else
             {
                 if (enemy != null)
                 {
                     //EnemyFX fx2 = enemy.GetComponent<EnemyFX>();
                     //fx.ShowTurnSelectionEffect(false); // Ativa o efeito de sele��o do inimigo
                 }
                 Debug.Log($"Enemy at index {i} is null or dead");
             }*/
            foreach (var action in actions)
            {
                switch (action.actionType)
                {
                    case EnemyActionType.Attack:
                        int damage = eStats.damage.GetValue();

                        if (playerStats.TargetCanAvoidAttack(playerStats))
                            damage = 0;

                        PlayerAnimationsOnEnemyTurn(damage);
                        isPlayerTakingHit = true;

                        enemyComponent.enemyAnim.SetTrigger("Attack");

                        yield return new WaitForSeconds(0.2f);
                        playerStats.TakeDamage(damage);
                        VerifyPlayerIsAlive();
                        yield return new WaitUntil(() => !isPlayerTakingHit);
                        break;

                    case EnemyActionType.Shield:
                        eStats.IncreaseShield(1); // Multiplicador padr�o
                        yield return new WaitForSeconds(0.3f);
                        break;

                    case EnemyActionType.Heal:
                        float healAmount = eStats.maxHealth.GetValue() * 0.2f;
                        eStats.Heal(healAmount);
                        yield return new WaitForSeconds(0.3f);
                        break;
                }
            }

            if (enemy != null)
            {
                //EnemyFX fx2 = enemy.GetComponent<EnemyFX>();
                //fx.ShowTurnSelectionEffect(false); // Ativa o efeito de sele��o do inimigo
            }

            eStats.AdvanceTurnAction();
            fx.HideNextAction();
            fx.ShowTurnSelectionEffect(false);
        }

        //Debug.Log("HideBlockHandCards sendo chamado no BattleManager");
        //UI_Manager.instance.HideBlockHandCards();
        EndTurn(); // Termina o turno dos inimigos ap�s todos atacarem
    }

    private void PlayerAnimationsOnEnemyTurn(int enemyDamage)
    {
        if (enemyDamage == 0)
        {
            TriggerPlayerEvasionAnimation(); // Anima��o de esquiva
        }
        else if (enemyDamage <= playerStats.shield)
        {
            TriggerPlayerBlockAnimation(); // Anima��o de bloqueio
        }
        else
        {
            TriggerPlayerHitAnimation(); // Anima��o de hit normal
        }
    }

    private void TriggerPlayerHitAnimation()
    {
        Animator playerAnimator = playerStats.GetComponentInChildren<Animator>();
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Hit");
        }
    }
    private void TriggerPlayerBlockAnimation()
    {
        Animator playerAnimator = playerStats.GetComponentInChildren<Animator>();
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Block");
        }
    }
    private void TriggerPlayerEvasionAnimation()
    {
        Animator playerAnimator = playerStats.GetComponentInChildren<Animator>();
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Evade");
        }
    }

    public void OnPlayerHitAnimationEnd()
    {
        isPlayerTakingHit = false;
    }

    public void VerifyPlayerIsAlive()
    {
        if (gameOverOnce == 1) // Garante que o m�todo s� ser� executado uma vez
        {
            if (playerStats.currentHealth <= 0) // Derrota
            {
                Debug.Log("Player foi derrotado.");
                gameOverOnce--;
                //LevelManager.Instance.ReturnToMenu(); // Vai direto para o menu principal em caso de derrota
                UI_Manager.instance.ShowEndGame(false);
            }
            else if (playerStats.player.CurrentEnemyTarget == null) // Vit�ria no �ltimo subn�vel
            {
                Debug.Log("Player venceu o �ltimo subn�vel.");
                LevelManager.Instance.AdavanceSubLevel(); // J� lida com o retorno ao menu
                gameOverOnce--;
            }
        }
    }

    public void UpdateEnergyText()
    {
        energyText.text = playerStats.energy.ToString();
    }

    private void UseCard(CardData cardData)
    {
        if (playerTurn && playerStats.energy >= cardData.energyCost)
        {
            playerStats.energy -= cardData.energyCost;
            UpdateEnergyText();

            if (playerStats.energy <= 0)
            {
                DeckManager.instance.HideHand();

                if (autoEndTurn)
                    StartCoroutine(DelayCallEndTurn());
                else
                    StartCoroutine(CheckForNoEnergy());
            }
        }
    }

    private void UseCardAttack(Enemy_Stats enemyTarget, CardData cardData)
    {
        if (playerTurn && playerStats.energy >= cardData.energyCost)
        {
            playerStats.energy -= cardData.energyCost;
            UpdateEnergyText();

            if (playerStats.energy <= 0)
            {
                DeckManager.instance.HideHand();

                if (autoEndTurn)
                    StartCoroutine(DelayCallEndTurn());
                else
                    StartCoroutine(CheckForNoEnergy());
            }
        }
    }
    private IEnumerator CheckForNoEnergy()
    {
        yield return new WaitForSeconds(3f); // Espera 3 segundos

        if (playerStats.energy <= 0 && playerTurn) // Verifica se ainda est� sem energia e � o turno do player
        {
            StartCoroutine(UI_Manager.instance.ShowEndTurnArrow());
        }
    }

    private IEnumerator DelayCallEndTurn()
    {
        yield return new WaitForSeconds(1f);
        EndTurn();
    }


    private void OnEnable()
    {
        DeckManager.OnCardUsed += UseCard;
        DeckManager.OnCardAttackUsed += UseCardAttack;
    }

    private void OnDisable()
    {
        DeckManager.OnCardUsed -= UseCard;
        DeckManager.OnCardAttackUsed -= UseCardAttack;
    }
}
