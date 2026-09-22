using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    private readonly Dictionary<GameObject, IObjectPool<GameObject>> _pools = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (Instance != null) return;
        var go = new GameObject("PoolManager");
        Instance = go.AddComponent<PoolManager>();
        DontDestroyOnLoad(go);
    }

    public GameObject Get(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
    {
        if (!_pools.TryGetValue(prefab, out var pool))
        {
            pool = CreatePool(prefab);
            _pools[prefab] = pool;
        }

        GameObject instance = pool.Get();
        instance.transform.SetParent(parent, false);
        instance.transform.SetPositionAndRotation(position, rotation);
        
        if (instance.TryGetComponent(out Rigidbody rb))
        {
            rb.position = position;
            rb.rotation = rotation;
        }
        
        return instance;
    }

    public void Release(GameObject instance)
    {
        if (instance.TryGetComponent(out PooledObject pooled) &&
            _pools.TryGetValue(pooled.SourcePrefab, out var pool))
        {
            pool.Release(instance);
        }
        else
        {
            Destroy(instance); // not a pooled instance — fall back safely
        }
    }

    private IObjectPool<GameObject> CreatePool(GameObject prefab)
    {
        return new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject obj = Instantiate(prefab);
                obj.AddComponent<PooledObject>().SourcePrefab = prefab;
                return obj;
            },
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: Destroy,
            collectionCheck: true,
            defaultCapacity: 10,
            maxSize: 100
        );
    }

    private static void OnGet(GameObject obj)
    {
        obj.SetActive(true);
        
        if (obj.TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        
        if (obj.TryGetComponent(out TimerVisual timerVisual)) timerVisual.ResetVisual();
        if (obj.TryGetComponent(out Food food)) food.ResetForReuse();
        // add more ResetForReuse-style hooks here as other pooled prefabs pick up reusable state
    }

    private static void OnRelease(GameObject obj) => obj.SetActive(false);
}

