using System.Collections;
using UnityEngine;

public class CircleTestPool : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(ReturnTextToPool(1f));
    }

    private IEnumerator ReturnTextToPool(float _time)
    {
        yield return new WaitForSeconds(_time);
        gameObject.SetActive(false);
        ObjectPooler.ReturnToPool(gameObject);
    }
}
