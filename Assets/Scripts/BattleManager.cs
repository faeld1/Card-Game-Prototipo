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
                playerStats.TakeDamage(10); // Dano fixo de 10, altere conforme necess�rio
                yield return new WaitForSeconds(0.5f); // Pequeno delay para ver cada ataque
            }
            else
            {
                Debug.Log($"Enemy at index {i} is null or dead");
            }
        }

        

        EndTurn(); // Termina o turno dos inimigos ap�s todos atacarem
    }

    public void VerifyPlayerIsAlive()
    {
        if(playerStats.player.CurrentEnemyTarget == null || playerStats.currentHealth <= 0)
        {
            if(gameOverOnce == 1)
            {
            UI_Manager.instance.ShowEndGame();
            Debug.Log("Verify do BattleManager");
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
