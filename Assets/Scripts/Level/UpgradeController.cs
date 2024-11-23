using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeController : MonoBehaviour
{
    public GameObject[] upgradeOptions; // Prefabs ou objetos de UI das opções de melhoria

    private void Start()
    {
        DisplayUpgradeOptions();
    }

    /// <summary>
    /// Exibe as opções de melhoria disponíveis para o jogador.
    /// </summary>
    private void DisplayUpgradeOptions()
    {
        foreach (var option in upgradeOptions)
        {
            option.SetActive(true); // Ativa cada opção de melhoria
        }
    }

    /// <summary>
    /// Aplica a melhoria escolhida e avança para o próximo subnível.
    /// </summary>
    /// <param name="upgradeID">ID da melhoria escolhida.</param>
    public void ApplyUpgrade(int upgradeID)
    {
        Debug.Log($"Upgrade {upgradeID} applied!");

        // Aqui você pode implementar a lógica específica para aplicar a melhoria
        // Por exemplo, aumentar dano, defesa, velocidade, etc.

        // Redireciona para a próxima subfase
        SceneManager.LoadScene("BattleScene");
    }

    public void GoToBattleScene()
    {
        SceneManager.LoadScene("BattleScene");
    }
}
