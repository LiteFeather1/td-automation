
public class PersistentSingleton<T> : Singleton<T> where T : PersistentSingleton<T>
{
    protected override void InitialiseInstance()
    {
        base.InitialiseInstance();
        DontDestroyOnLoad(gameObject);
    }
}
