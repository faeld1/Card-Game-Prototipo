using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeController : MonoBehaviour
{
    public GameObject[] upgradeOptions; // Prefabs ou objetos de UI das op��es de melhoria

    private void Start()
    {
        DisplayUpgradeOptions();
    }

    /// <summary>
    /// Exibe as op��es de melhoria dispon�veis para o jogador.
    /// </summary>
    private void DisplayUpgradeOptions()
    {
        foreach (var option in upgradeOptions)
        {
            option.SetActive(true); // Ativa cada op��o de melhoria
        }
    }

    /// <summary>
    /// Aplica a melhoria escolhida e avan�a para o pr�ximo subn�vel.
    /// </summary>
    /// <param name="upgradeID">ID da melhoria escolhida.</param>
    public void ApplyUpgrade(int upgradeID)
    {
        Debug.Log($"Upgrade {upgradeID} applied!");

        // Aqui voc� pode implementar a l�gica espec�fica para aplicar a melhoria
        // Por exemplo, aumentar dano, defesa, velocidade, etc.

        // Redireciona para a pr�xima subfase
        SceneManager.LoadScene("BattleScene");
    }

    public void GoToBattleScene()
    {
        SceneManager.LoadScene("BattleScene");
    }
}
