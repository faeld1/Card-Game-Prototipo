using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Enemy_Stats Stats { get; set; }

    private void Awake()
    {
        Stats = GetComponent<Enemy_Stats>();
    }

}
