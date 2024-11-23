using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public Transform[] enemySpawnPositions;
    private List<GameObject> activeEnemies = new List<GameObject>();

    private void Start()
    {
        LoadCurrentSubLevel();
    }

    /// Carrega os inimigos do subnível atual com base no LevelManager.
    private void LoadCurrentSubLevel()
    {
        var currentSubLevelIndex = LevelManager.Instance.currentSubLevelIndex;
        var currentLevel = LevelManager.Instance.currentLevel;

        // Determina os inimigos do subnível atual
        List<GameObject> subLevelEnemies = null;

        switch (currentSubLevelIndex)
        {
            case 0: subLevelEnemies = currentLevel.subLevel1Enemies; break;
            case 1: subLevelEnemies = currentLevel.subLevel2Enemies; break;
            case 2: subLevelEnemies = currentLevel.subLevel3Enemies; break;
            case 3: subLevelEnemies = currentLevel.subLevel4Enemies; break;
            case 4: subLevelEnemies = currentLevel.subLevel5Enemies; break;
        }

        // Spawna os inimigos nas posições
        for (int i = 0; i < subLevelEnemies.Count; i++)
        {
            if (i < enemySpawnPositions.Length)
            {
                GameObject enemy = Instantiate(subLevelEnemies[i], enemySpawnPositions[i].position, Quaternion.identity);
                activeEnemies.Add(enemy);
            }
        }

        BattleManager.instance.enemies = GetCurrentEnemies();
    }

    /// Retorna os componentes CharacterStats dos inimigos instanciados
    public CharacterStats[] GetCurrentEnemies()
    {
        CharacterStats[] enemyStats = new CharacterStats[activeEnemies.Count];
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            if (activeEnemies[i] != null)
            {
                enemyStats[i] = activeEnemies[i].GetComponent<CharacterStats>();
            }
        }

        return enemyStats;
    }


    /// Chama este método quando o jogador vence o subnível atual.
    public void OnSubLevelWon()
    {
        LevelManager.Instance.AdavanceSubLevel();
    }


    /// Chama este método quando o jogador perde o subnível atual.
    public void OnSubLevelLost()
    {
        Debug.Log("SubLevel Lost! Restarting...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reinicia a cena atual
    }
}
