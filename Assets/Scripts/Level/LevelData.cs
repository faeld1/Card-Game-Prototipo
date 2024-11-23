using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Levels/Level Data")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public int levelNumber;
    public bool isUnlocked;

    // Listas separadas para os inimigos de cada subnível
    [Header("Sub Level Enemies")]
    public List<GameObject> subLevel1Enemies;
    public List<GameObject> subLevel2Enemies;
    public List<GameObject> subLevel3Enemies;
    public List<GameObject> subLevel4Enemies;
    public List<GameObject> subLevel5Enemies;
}
