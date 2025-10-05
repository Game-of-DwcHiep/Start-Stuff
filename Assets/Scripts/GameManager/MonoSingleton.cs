using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : Component
{
    private static T instance;
    private static bool isDestroyed = false;

    public static T Instance
    {
        get
        {
            if (isDestroyed)
            {
                Debug.LogWarning($"{typeof(T)} destroy.");
                return null;
            }

            if (instance == null)
            {
                instance = FindObjectOfType<T>();

                if (instance == null)
                {
                    Debug.LogWarning($"{typeof(T)}  Scene.");
                }
            }

            return instance;
        }
    }

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(instance.gameObject);
            isDestroyed = false;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public virtual void OnDestroy()
    {
        if (instance == this)
        {
            isDestroyed = true;
            instance = null;
        }
    }

    public static bool Exists()
    {
        return instance != null && !isDestroyed;
    }
}
