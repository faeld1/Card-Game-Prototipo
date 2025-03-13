using UnityEngine;

public class TestNewObjectPoolManager : MonoBehaviour
{
    public static TestNewObjectPoolManager instance;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject newInstance = Pooler.GetInstanceFromPool();
            newInstance.transform.position = Vector3.zero;
            newInstance.SetActive(true);

        }
    }
}
