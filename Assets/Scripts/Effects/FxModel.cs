using System.Collections;
using UnityEngine;

public class FxModel : MonoBehaviour
{
    private ParticleSystem fxParticleSystem;

    private void OnEnable()
    {
        fxParticleSystem = GetComponent<ParticleSystem>();

        // Verifique se a animação está configurada corretamente
        if (fxParticleSystem != null)
        {
            var mainModule = fxParticleSystem.main; // Captura o MainModule corretamente
            float animationLength = mainModule.duration;

            //StartCoroutine(ReturnTextToPool(animationLength));
        }
        else
        {
            Debug.LogWarning("ParticleSystem não encontrado.");
        }
    }

    private IEnumerator ReturnTextToPool(float _time)
    {
        yield return new WaitForSeconds(_time);
        gameObject.SetActive(false);
        ObjectPooler.ReturnToPool(gameObject);
    }
}
