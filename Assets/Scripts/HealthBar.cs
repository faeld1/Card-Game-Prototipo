using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private CharacterStats myStats;
    private Slider slider;
    private Enemy enemy;
    private Player player;

    private bool allowVerify = false;
    private bool isDead = false;

    private void Start()
    {
        slider = GetComponentInChildren<Slider>();
        myStats = GetComponentInParent<CharacterStats>();
        enemy = GetComponentInParent<Enemy>();
        player = GetComponentInParent<Player>();

        UpdateHealthUI();
    }


    public void UpdateHealthUI()
    {
        slider.maxValue = myStats.maxHealth.GetValue();
        slider.value = myStats.currentHealth;

        if (allowVerify)
            BattleManager.instance.VerifyPlayerIsAlive();

        if (myStats.currentHealth <= 0)
        {
            //Destroy(myStats.gameObject);
            if (this.enemy != null && !isDead)
            {
                this.enemy.enemyAnim.SetTrigger("Death");
                isDead = true;
                Debug.Log("Enemy Death Animation");
            }
            if(player != null)
            {
                player.animator.SetTrigger("Death");
            }

            this.slider.gameObject.SetActive(false);
            
            StartCoroutine(DeathWithDelay());
        }

        allowVerify = true;

    }
    private IEnumerator DeathWithDelay()
    {
        Destroy(myStats);
        yield return new WaitForSeconds(1.5f);
        if (enemy != null)
        {
            Destroy(enemy.gameObject);
        }
    }

    private void OnEnable()
    {
        CharacterStats.OnHealthChanged += UpdateHealthUI;
    }

    private void OnDisable()
    {
        CharacterStats.OnHealthChanged -= UpdateHealthUI;
    }
}
