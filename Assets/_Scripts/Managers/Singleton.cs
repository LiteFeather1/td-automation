using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance { get; protected set; }

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        InitialiseInstance();
    }

    protected virtual void InitialiseInstance()
    {
        Instance = (T)this;
    }
}
