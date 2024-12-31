using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectPool<T> where T : Component
{
    [SerializeField] private T _object;
    [SerializeField] private int _initialPoolSize;
    private Transform _poolParent;
    private readonly Queue<T> _inactiveObjects = new();

    public List<T> Objects { get; private set; } = new();

    private Action<T> _objectCreated;

    public T Object => _object;
    public Transform PoolParent => _poolParent;

    public ObjectPool(T object_, int initialPoolSize)
    {
        _object = object_;
        _initialPoolSize = initialPoolSize;
    }

    public ObjectPool(T object_) : this(object_, 10) { }

    public ObjectPool(int initialPoolSize) : this(null, initialPoolSize) { }

    public ObjectPool() { }

    ~ObjectPool()
    {
        _objectCreated = null;
        Objects.Clear();
    }

    public void InitPool(int size, Action<T> created, bool spawnActive = false)
    {
        if (_poolParent != null)
            return;

        _poolParent = new GameObject($"Pool_{_object.name}").transform;

        _object.gameObject.SetActive(spawnActive);

        _objectCreated += created;

        for (int i = 0; i < size; i++)
            _inactiveObjects.Enqueue(Instantiate());
    }

    public void InitPool(Action<T> created, bool spawnActive = false) 
        => InitPool(_initialPoolSize, created, spawnActive);

    public T GetObject()
    {
        if (_inactiveObjects.Count > 0)
            return _inactiveObjects.Dequeue();

        return Instantiate();
    }

    public void ReturnObject(T object_)
    {
        _inactiveObjects.Enqueue(object_);
    }

    public void Deinit(Action<T> created)
    {
        _objectCreated -= created;
        Objects.Clear();
    }

    private T Instantiate()
    {
        T object_ = UnityEngine.Object.Instantiate(_object, _poolParent);
        object_.name = $"{_object.name}_{Objects.Count}";
        Objects.Add(object_);
        _objectCreated?.Invoke(object_);
        return object_;
    }
}