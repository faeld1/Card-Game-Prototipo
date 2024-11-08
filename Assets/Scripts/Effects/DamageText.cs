using System.Collections;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public TextMeshProUGUI damageText => GetComponentInChildren<TextMeshProUGUI>();

    private Animator anim;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();

        float animationLenght = anim.GetCurrentAnimatorStateInfo(0).length;

        StartCoroutine(ReturnTextToPool(animationLenght));
    }

    private IEnumerator ReturnTextToPool(float _time)
    {
        yield return new WaitForSeconds(_time);
        damageText.color = Color.white;
        ObjectPooler.ReturnToPool(gameObject);
    }

}
