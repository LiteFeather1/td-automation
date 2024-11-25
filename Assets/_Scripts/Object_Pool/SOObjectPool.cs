using UnityEngine;

public abstract class SOObjectPool<T> : ScriptableObject where T : Component
{
	[SerializeField] private ObjectPool<T> _objectPool;

	public ObjectPool<T> ObjectPool => _objectPool;
}
