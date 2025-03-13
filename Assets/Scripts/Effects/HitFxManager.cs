using UnityEngine;

public class HitFxManager : MonoBehaviour
{
    public static HitFxManager instance;
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
