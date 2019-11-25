using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] private bool _DontDestroyOnLoad;

    public static T Instance;

    protected virtual void Awake()
    {
        if (ReferenceEquals(Instance, null))
        {
            Instance = this as T;
        }
        else if (Instance != this as T)
        {
            Destroy(gameObject);
            return;
        }

        if (_DontDestroyOnLoad)
        {
            DontDestroyOnLoad(Instance);
        }
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this as T)
        {
            Instance = null;
        }
    }
}
