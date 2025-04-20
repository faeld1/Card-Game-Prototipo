using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyFX : MonoBehaviour
{
    [Header("HUD FX")]
    [SerializeField] private Transform textDamageSpawnPosition;
    [SerializeField] private GameObject textDamagePrefab;
    [SerializeField] private GameObject selectedEffect;
    [SerializeField] private GameObject turnSelectedEffect;

    [Header("Cloud Thinking")]
    [SerializeField] private CloudThinkingDisplay thinkingDisplay;

    private Enemy_Stats _enemy;

    private void Start()
    {
        _enemy = GetComponent<Enemy_Stats>();
        ShowSelectedEffect(false);
        ShowTurnSelectionEffect(false);

        thinkingDisplay = GetComponentInChildren<CloudThinkingDisplay>();
        HideNextAction();

        // Faz o prefab do cloud conhecer o enemy
        if (thinkingDisplay != null)
            thinkingDisplay.Setup(_enemy);
    }

    public void ShowSelectedEffect(bool show)
    {
        if(selectedEffect != null)
            selectedEffect.SetActive(show);
    }

    public void ShowTurnSelectionEffect(bool show)
    {
        if(turnSelectedEffect != null)
            turnSelectedEffect.SetActive(show);
    }
    public void ShowNextAction(EnemyAction[] actions)
    {
        if (thinkingDisplay != null)
            thinkingDisplay.Show(actions);
    }

    public void HideNextAction()
    {
        if (thinkingDisplay != null)
            thinkingDisplay.Hide();
    }

    private void EnemyHit(Enemy_Stats enemy, int damage,bool crit)
    {
        if (_enemy == enemy && enemy != null)
        {
            //Text Damage
            float randomX = Random.Range(-0.5f, 0.5f);
            float randomY = Random.Range(0f, 1f);

            Vector3 randomPosition = new Vector3(randomX,randomY,0);

            // GameObject newInstance = Instantiate(textDamagePrefab, textDamageSpawnPosition.position+randomPosition,Quaternion.identity);
            GameObject newInstance = DamageTextManager.instance.Pooler.GetInstanceFromPool();

            TextMeshProUGUI damageText = newInstance.GetComponent<DamageText>().damageText;
            damageText.text = damage.ToString();           

            if (crit)
                damageText.color = Color.red;
            else
                damageText.color = Color.white;

            if (damage == 0)
            {
                damageText.text = "Miss";
                damageText.color = Color.white;
                enemy.enemy.enemyAnim.SetTrigger("Evasion"); // Animação de esquiva
            }
            else
            {
                enemy.enemy.enemyAnim.SetTrigger("Hit"); // Animação de dano recebido
            }

            newInstance.transform.position = textDamageSpawnPosition.position+randomPosition;
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

            //newInstance.transform.SetParent(textDamagePrefab.transform);
        }
    }

    private void OnEnable()
    {
        CharacterStats.OnEnemyHit += EnemyHit;
    }

    private void OnDisable()
    {
        CharacterStats.OnEnemyHit -= EnemyHit;
    }
}
