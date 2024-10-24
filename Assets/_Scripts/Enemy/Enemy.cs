using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _damage;

    [Header("References")]
    [SerializeField] private Health _health;
    [SerializeField] private PathFollow _pathFollow;

    public float Damage => _damage;

    public PathFollow PathFollow => _pathFollow;

    public Action<Enemy> OnDied { get; set; }
    public Action<Enemy> OnPathReached { get; set; }

    private void OnEnable()
    {
        _health.OnDied += Died;
        _pathFollow.OnPathFinished += PathFinished;
    }

    private void OnDisable()
    {
        _health.OnDied -= Died;
        _pathFollow.OnPathFinished -= PathFinished;
    }

    private void Died()
    {
        OnDied?.Invoke(this);
    }

    private void PathFinished()
    {
        OnPathReached?.Invoke(this);
    }
}
