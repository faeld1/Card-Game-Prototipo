using System.Collections;
using TMPro;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    public CharacterStats playerStats;
    public CharacterStats[] enemies;

    public TextMeshProUGUI howTurnText;

    private bool playerTurn = true;
    int gameOverOnce = 1;

    private int playerBaseEnergy;
    public int PlayerCurrentEnergy => playerStats.energy;
    private int startShield = 0;

    private bool isPlayerTakingHit = false;

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
           // UI_Manager.instance.HideBlockHandCards();
            playerStats.energy = playerBaseEnergy; //Reseta energia

            startShield++;

            if (startShield > 1)
                playerStats.ResetShield(); //Reseta escudo
        }
        else
        {
            howTurnText.text = "Turno do Enemy!";
            playerStats.energy = 0;
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
        if(playerTurn)
            UI_Manager.instance.ShowBlockHandCards();
            
        StartCoroutine(DelayEndTurn());     
    }

    private IEnumerator DelayEndTurn()
    {
        Debug.Log("IsAttackingAnimation(DelayEndTurn): " + playerStats.player.isAttackingAnimation);
        yield return new WaitUntil(() => !playerStats.player.isAttackingAnimation); // Aguarda ataque terminar

        yield return new WaitForSeconds(1f);

        if(!playerTurn)
            UI_Manager.instance.HideBlockHandCards();

        playerTurn = !playerTurn;

        StartTurn();
    }

    public void ForceEndTurn()
    {
        if (playerTurn)
            EndTurn();
    }

    private IEnumerator EnemyTurn()
    {
        UI_Manager.instance.ShowBlockHandCards();
        yield return new WaitForSeconds(0.9f);

        for (int i = 0; i < enemies.Length; i++)
        {
            CharacterStats enemy = enemies[i];
            
            yield return new WaitForSeconds(0.4f); // Delay entre os ataques dos inimigos
            if (enemy.currentHealth > 0 && enemy != null)
            {
                int enemyDamage = enemy.damage.GetValue(); // Dano do inimigo

                if (playerStats.TargetCanAvoidAttack(playerStats)) // Se o player esquivar
                {
                    enemyDamage = 0;
                }

                PlayetAnimationsOnEnemyTurn(enemyDamage);

                isPlayerTakingHit = true;

                Enemy enemyAnimator = enemy.GetComponent<Enemy>();
                enemyAnimator.enemyAnim.SetTrigger("Attack"); // Anima��o de ataque do inimigo

                yield return new WaitForSeconds(0.2f); // Delay para o hit DamageText aparecer
                playerStats.TakeDamage(enemyDamage); // Aplica o dano



                yield return new WaitUntil(() => !isPlayerTakingHit); // Espera a anima��o terminar

            }
            else
            {
                Debug.Log($"Enemy at index {i} is null or dead");
            }
        }

        //Debug.Log("HideBlockHandCards sendo chamado no BattleManager");
        //UI_Manager.instance.HideBlockHandCards();
        EndTurn(); // Termina o turno dos inimigos ap�s todos atacarem
    }

    private void PlayetAnimationsOnEnemyTurn(int enemyDamage)
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
                LevelManager.Instance.ReturnToMenu(); // Vai direto para o menu principal em caso de derrota
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

            /* if (playerStats.energy <= 0)
                 EndTurn();*/
            if (playerStats.energy <= 0)
                StartCoroutine(DelayCallEndTurn());
        }
    }

    private void UseCardAttack(Enemy_Stats enemyTarget, CardData cardData)
    {
        if (playerTurn && playerStats.energy >= cardData.energyCost)
        {
            playerStats.energy -= cardData.energyCost;
            UpdateEnergyText();

            /* if (playerStats.energy <= 0)
                 EndTurn();*/
            if (playerStats.energy <= 0)
                StartCoroutine(DelayCallEndTurn());
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
