using UnityEngine;
public enum EnemyActionType
{
    Attack,
    Shield,
    Heal
}
public class Enemy : MonoBehaviour
{
    public Enemy_Stats Stats { get; set; }
    public EnemyFX FX { get; set; }
    public Animator enemyAnim;

    private void Awake()
    {
        enemyAnim = GetComponentInChildren<Animator>();
        FX = GetComponent<EnemyFX>();
        Stats = GetComponent<Enemy_Stats>();
    }

}
