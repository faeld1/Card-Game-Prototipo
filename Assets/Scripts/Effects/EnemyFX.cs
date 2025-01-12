using TMPro;
using UnityEngine;

public class EnemyFX : MonoBehaviour
{
    [SerializeField] private Transform textDamageSpawnPosition;
    [SerializeField] private GameObject textDamagePrefab;
    [SerializeField] private GameObject selectedEffect;

    private Enemy_Stats _enemy;

    private void Start()
    {
        _enemy = GetComponent<Enemy_Stats>();
        ShowSelectedEffect(false);
    }

    public void ShowSelectedEffect(bool show)
    {
        if(selectedEffect != null)
            selectedEffect.SetActive(show);
    }

    private void EnemyHit(Enemy_Stats enemy, int damage,bool crit)
    {
        if (_enemy == enemy && enemy != null)
        {
            float randomX = Random.Range(-0.5f, 0.5f);
            float randomY = Random.Range(0f, 1f);

            Vector3 randomPosition = new Vector3(randomX,randomY,0);

            // GameObject newInstance = Instantiate(textDamagePrefab, textDamageSpawnPosition.position+randomPosition,Quaternion.identity);
            GameObject newInstance = DamageTextManager.instance.Pooler.GetInstanceFromPool();

            TextMeshProUGUI damageText = newInstance.GetComponent<DamageText>().damageText;
            damageText.text = damage.ToString();

            if(crit)
                damageText.color = Color.red;

            if(damage == 0)
            {
                damageText.text = "Miss";
                damageText.color = Color.white;
            }


            newInstance.transform.position = textDamageSpawnPosition.position+randomPosition;
            newInstance.SetActive(true);

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
