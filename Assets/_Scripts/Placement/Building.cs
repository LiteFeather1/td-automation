﻿using UnityEngine;

public abstract class Building : MonoBehaviour, IPlaceable
{
    public Vector2Int Position { get; set; }
    public abstract bool CanBeRotated { get; }
    public abstract bool CanBeDestroyed { get; }

    public virtual void Destroy()
    {
        Destroy(gameObject);
    }
}
