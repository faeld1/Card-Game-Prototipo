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

    public TextMeshProUGUI howTurnText;

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
        playerBaseEnergy = playerStats.energy;

        StartTurn();
    }

    private void StartTurn()
    {
        if (playerTurn)
        {
            howTurnText.text = "Turno do Player!";
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
        }
        else
        {
            howTurnText.text = "Turno do Enemy!";
            playerStats.energy = 0;
            UI_Manager.instance.endTurnButton.interactable = false;
            UI_Manager.instance.DisableArrow();
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

            if (enemy != null)
            {
                EnemyFX fx = enemy.GetComponent<EnemyFX>();
                fx.ShowTurnSelectionEffect(true); // Ativa o efeito de seleção do inimigo
            }


            yield return new WaitForSeconds(0.6f); // Delay entre os ataques dos inimigos
            if (enemy.currentHealth > 0 && enemy != null)
            {
                int enemyDamage = enemy.damage.GetValue(); // Dano do inimigo

                if (playerStats.TargetCanAvoidAttack(playerStats)) // Se o player esquivar
                {
                    enemyDamage = 0;
                }

                PlayerAnimationsOnEnemyTurn(enemyDamage);

                isPlayerTakingHit = true;

                Enemy enemyAnimator = enemy.GetComponent<Enemy>();
                enemyAnimator.enemyAnim.SetTrigger("Attack"); // Animação de ataque do inimigo

                yield return new WaitForSeconds(0.2f); // Delay para o hit DamageText aparecer
                playerStats.TakeDamage(enemyDamage); // Aplica o dano
                VerifyPlayerIsAlive();



                yield return new WaitUntil(() => !isPlayerTakingHit); // Espera a animação terminar

            }
            else
            {
                if (enemy != null)
                {
                    EnemyFX fx = enemy.GetComponent<EnemyFX>();
                    fx.ShowTurnSelectionEffect(false); // Ativa o efeito de seleção do inimigo
                }
                Debug.Log($"Enemy at index {i} is null or dead");
            }

            if (enemy != null)
            {
                EnemyFX fx = enemy.GetComponent<EnemyFX>();
                fx.ShowTurnSelectionEffect(false); // Ativa o efeito de seleção do inimigo
            }
        }

        //Debug.Log("HideBlockHandCards sendo chamado no BattleManager");
        //UI_Manager.instance.HideBlockHandCards();
        EndTurn(); // Termina o turno dos inimigos após todos atacarem
    }

    private void PlayerAnimationsOnEnemyTurn(int enemyDamage)
    {
        if (enemyDamage == 0)
        {
            TriggerPlayerEvasionAnimation(); // Animação de esquiva
        }
        else if (enemyDamage <= playerStats.shield)
        {
            TriggerPlayerBlockAnimation(); // Animação de bloqueio
        }
        else
        {
            TriggerPlayerHitAnimation(); // Animação de hit normal
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
        if (gameOverOnce == 1) // Garante que o método só será executado uma vez
        {
            if (playerStats.currentHealth <= 0) // Derrota
            {
                Debug.Log("Player foi derrotado.");
                gameOverOnce--;
                //LevelManager.Instance.ReturnToMenu(); // Vai direto para o menu principal em caso de derrota
                UI_Manager.instance.ShowEndGame(false);
            }
            else if (playerStats.player.CurrentEnemyTarget == null) // Vitória no último subnível
            {
                Debug.Log("Player venceu o último subnível.");
                LevelManager.Instance.AdavanceSubLevel(); // Já lida com o retorno ao menu
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

        if (playerStats.energy <= 0 && playerTurn) // Verifica se ainda está sem energia e é o turno do player
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
