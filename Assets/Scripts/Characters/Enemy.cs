using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Enemy_Stats Stats { get; set; }
    public Animator enemyAnim;

    private void Awake()
    {
        enemyAnim = GetComponentInChildren<Animator>();
        Stats = GetComponent<Enemy_Stats>();
    }

}
