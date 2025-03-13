using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerFX : MonoBehaviour
{
    [SerializeField] private Transform textDamageSpawnPosition;
    [SerializeField] private GameObject textDamagePrefab;
    [SerializeField] private GameObject selectedEffect; // Efeito de seleção

    [SerializeField] private GameObject healEffect;
    [SerializeField] private GameObject levelUpEffect;

    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        ShowSelectedEffect(false); // Garantir que o efeito esteja desativado no início
    }
    public void ShowSelectedEffect(bool show)
    {
        if (selectedEffect != null)
            selectedEffect.SetActive(show);
    }


    private void PlayerHit(PlayerStats player, int damage)
    {
        if(playerStats == player && player != null)
        {
            float randomX = Random.Range(-0.5f, 0.5f);
            float randomY = Random.Range(0f, 1f);

            Vector3 randomPosition = new Vector3(randomX, randomY, 0);

            // GameObject newInstance = Instantiate(textDamagePrefab, textDamageSpawnPosition.position+randomPosition,Quaternion.identity);
            GameObject newInstance = DamageTextManager.instance.Pooler.GetInstanceFromPool();

            TextMeshProUGUI damageText = newInstance.GetComponent<DamageText>().damageText;
            damageText.text = damage.ToString();
            damageText.color = Color.white;

            if (damage <= player.shield) //Deixa a cor do texto azul se o dano for absorvido pelo escudo
            {
                damageText.color = Color.blue;
            }

            if (damage == 0) //Se o dano for 0, exibe "Miss" no texto
            {
                damageText.text = "Miss";
                damageText.color = Color.white;
            }

            newInstance.transform.position = textDamageSpawnPosition.position + randomPosition;
            newInstance.SetActive(true);
        }
    }

    public void PlayHealEffect()
    {
        if (healEffect != null)
        {
            healEffect.SetActive(true);
            StartCoroutine(DisableEffectAfterTime(healEffect, 2f)); // Desativa após 2s
        }
    }

    public void PlayLevelUpEffect()
    {
        if (levelUpEffect != null)
        {
            levelUpEffect.SetActive(true);
            StartCoroutine(DisableEffectAfterTime(levelUpEffect, 5f)); // Desativa após 5s
        }
    }

    // Método auxiliar para desativar efeitos após um tempo
    private IEnumerator DisableEffectAfterTime(GameObject effect, float time)
    {
        yield return new WaitForSeconds(time);
        effect.SetActive(false);
    }

    private void OnEnable()
    {
        PlayerStats.OnPlayerHit += PlayerHit;
    }

    private void OnDisable()
    {
        PlayerStats.OnPlayerHit -= PlayerHit;
    }
}
