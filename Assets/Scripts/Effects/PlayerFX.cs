using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerFX : MonoBehaviour
{
    [SerializeField] private Transform textDamageSpawnPosition;
    [SerializeField] private GameObject textDamagePrefab;
    [SerializeField] private GameObject selectedEffect; // Efeito de seleção

    [SerializeField] private GameObject healEffect;
    [SerializeField] private GameObject levelUpEffect;

    [Header("OutLine FX")]
    private Renderer[] renderers;
    private Material[] outlineMaterials;

    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        ShowSelectedEffect(false); // Garantir que o efeito esteja desativado no início

        SelectedMaterialSetup();
    }




    private void SelectedMaterialSetup()
    {
        renderers = GetComponentsInChildren<Renderer>();

        // Cria instâncias únicas dos materiais para não afetar globalmente
        List<Material> mats = new List<Material>();
        foreach (var rend in renderers)
        {
            Material instanced = rend.material;
            mats.Add(instanced);
            rend.material = instanced;
        }

        outlineMaterials = mats.ToArray();
    }

    public void ShowSelectedEffect(bool show)
    {
        if (selectedEffect != null)
            selectedEffect.SetActive(false);

        // selectedEffect.SetActive(show); //Se quiser o selectEffect funcionando use esse aqui acima

        if (outlineMaterials == null) return;

        foreach (Material mat in outlineMaterials)
        {
            if (show)
            {
                mat.SetColor("_OutlineColor", new Color(.9f, .9f, .9f, 0.5f)); // laranja
                mat.SetFloat("_OutlineWidth", 3.5f);


            }
            else
            {
                mat.SetFloat("_OutlineWidth", 1f);
                mat.SetColor("_OutlineColor", new Color(0f, 0f, 0f)); // preto
            }
        }
    }


    private void PlayerHit(PlayerStats player, int damage)
    {
        if (playerStats == player && player != null)
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

            //Hit Effect
            float randomEffectX = Random.Range(-0.5f, 0.5f);
            float randomEffectY = Random.Range(0f, 1f);

            Vector3 randomEffectPosition = new Vector3(randomEffectX, randomEffectY, -0.5f);

            GameObject hitEffectInstance = HitFxManager.instance.Pooler.GetInstanceFromPool();
            if (hitEffectInstance == null)
            {
                Debug.LogError("Hit Effect retornou NULL! Verifique se o Pooler está configurado corretamente.");
                return;
            }

            hitEffectInstance.transform.localScale = Vector3.one * 0.7f; // Reduz tamanho original
            hitEffectInstance.transform.position = selectedEffect.transform.position + randomEffectPosition;
            hitEffectInstance.SetActive(true);
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
