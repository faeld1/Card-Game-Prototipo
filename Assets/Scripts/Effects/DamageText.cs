using System.Collections;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public TextMeshProUGUI damageText => GetComponentInChildren<TextMeshProUGUI>();

    private Animator anim;

    private void OnEnable()
    {
        anim = GetComponentInChildren<Animator>();

        // Verifique se a animação está configurada corretamente
        if (anim != null)
        {
            float animationLength = anim.GetCurrentAnimatorStateInfo(0).length;
            StartCoroutine(ReturnTextToPool(animationLength));
        }
        else
        {
            Debug.LogWarning("Animator não encontrado.");
        }
    }

    private IEnumerator ReturnTextToPool(float _time)
    {
        yield return new WaitForSeconds(_time);
        damageText.color = Color.white;
        gameObject.SetActive(false);
        ObjectPooler.ReturnToPool(gameObject);
    }


}
