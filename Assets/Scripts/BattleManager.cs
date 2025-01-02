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
    private int startShield = 0;

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
        if(playerTurn)
        {
            howTurnText.text = "Turno do Player!";
            DeckManager.instance.ShowHand();
            playerStats.energy = playerBaseEnergy; //Reseta energia

            startShield++;

            if(startShield> 1)
            playerStats.ResetShield(); //Reseta escudo
        }
        else
        {
            howTurnText.text = "Turno do Enemy!";
            playerStats.energy = 0;
            DeckManager.instance.HideHand();
            StartCoroutine(EnemyTurn());
        }

        UpdateEnergyText();
    }

    private void EndTurn()
    {
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
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < enemies.Length; i++)
        {
            CharacterStats enemy = enemies[i];
            if (enemy.currentHealth > 0 && enemy != null)
            {
                int enemyDamage = enemy.damage.GetValue();

                if (playerStats.TargetCanAvoidAttack(enemy))
                {
                    enemyDamage = 0;
                }
                playerStats.TakeDamage(enemyDamage); // Dano fixo de 10, altere conforme necessário
                yield return new WaitForSeconds(0.5f); // Pequeno delay para ver cada ataque
            }
            else
            {
                Debug.Log($"Enemy at index {i} is null or dead");
            }
        }

        

        EndTurn(); // Termina o turno dos inimigos após todos atacarem
    }

    public void VerifyPlayerIsAlive()
    {
        if (gameOverOnce == 1) // Garante que o método só será executado uma vez
        {
            if (playerStats.currentHealth <= 0) // Derrota
            {
                Debug.Log("Player foi derrotado.");
                gameOverOnce--;
                LevelManager.Instance.ReturnToMenu(); // Vai direto para o menu principal em caso de derrota
            }
            else if (playerStats.player.CurrentEnemyTarget == null) // Vitória no último subnível
            {
                Debug.Log("Player venceu o último subnível.");
                LevelManager.Instance.AdavanceSubLevel(); // Já lida com o retorno ao menu
                gameOverOnce--;
            }
        }
    }

    private void UpdateEnergyText()
    {
        energyText.text = playerStats.energy.ToString();
    }

    private void UseCard(CardData cardData)
    {
        if(playerTurn && playerStats.energy >= cardData.energyCost)
        {
            playerStats.energy -= cardData.energyCost;
            UpdateEnergyText();

            if (playerStats.energy <= 0)
                EndTurn();
        }
    }

    private void OnEnable()
    {
        DeckManager.OnCardUsed += UseCard;
    }

    private void OnDisable()
    {
        DeckManager.OnCardUsed -= UseCard;
    }
}
