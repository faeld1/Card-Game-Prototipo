using Unity.VisualScripting;
using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager instance;
    public ObjectPooler Pooler { get; set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        Pooler = GetComponent<ObjectPooler>();
    }
}
