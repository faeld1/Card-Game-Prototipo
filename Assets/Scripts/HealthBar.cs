using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private CharacterStats myStats;
    private Slider slider;
    private Enemy enemy;
    private Player player;

    private TextMeshProUGUI healthText;

    private bool allowVerify = false;
    private bool isDead = false;

    private void Start()
    {
        slider = GetComponentInChildren<Slider>();
        myStats = GetComponentInParent<CharacterStats>();
        enemy = GetComponentInParent<Enemy>();
        player = GetComponentInParent<Player>();
        healthText = GetComponentInChildren<TextMeshProUGUI>();

        UpdateHealthUI();
    }


    public void UpdateHealthUI()
    {
        slider.maxValue = myStats.maxHealth.GetValue();
        slider.value = myStats.currentHealth;

        healthText.text = (myStats.currentHealth + "/" + myStats.maxHealth.GetValue());

        if (allowVerify)
            BattleManager.instance.VerifyPlayerIsAlive();

        if (myStats.currentHealth <= 0)
        {
            //Destroy(myStats.gameObject);
            if (this.enemy != null && !isDead)
            {
                this.enemy.enemyAnim.SetTrigger("Death");
                this.enemy.enemyAnim.SetBool("IsDead", true);
                isDead = true;
                Debug.Log("Enemy Death Animation");

                // Desativa o componente CloudThinkingDisplay, se existir
                CloudThinkingDisplay cloud = enemy.GetComponentInChildren<CloudThinkingDisplay>(true);
                if (cloud != null)
                    cloud.gameObject.SetActive(false);
            }
            if (player != null)
            {
                player.animator.SetTrigger("Death");
            }

            this.slider.gameObject.SetActive(false);

            // Desativa o componente ShieldBar, se existir
            if(myStats != null)
            {
                ShieldBar shieldBar = myStats.GetComponentInChildren<ShieldBar>(true);
                if (shieldBar != null)
                    shieldBar.gameObject.SetActive(false);
            }

            StartCoroutine(DeathWithDelay());
        }

            allowVerify = true;

    }
    private IEnumerator DeathWithDelay()
    {
        Destroy(myStats);
        yield return new WaitForSeconds(1.8f);
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
