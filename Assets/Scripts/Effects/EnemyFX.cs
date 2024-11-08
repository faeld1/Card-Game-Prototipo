using TMPro;
using UnityEngine;

public class EnemyFX : MonoBehaviour
{
    [SerializeField] private Transform textDamageSpawnPosition;
    [SerializeField] private GameObject textDamagePrefab;

    private Enemy_Stats _enemy;

    private void Start()
    {
        _enemy = GetComponent<Enemy_Stats>();
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
