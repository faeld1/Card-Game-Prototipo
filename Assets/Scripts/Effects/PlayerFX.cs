using UnityEngine;
using TMPro;

public class PlayerFX : MonoBehaviour
{
    [SerializeField] private Transform textDamageSpawnPosition;
    [SerializeField] private GameObject textDamagePrefab;

    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();
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

            newInstance.transform.position = textDamageSpawnPosition.position + randomPosition;
            newInstance.SetActive(true);
        }
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
