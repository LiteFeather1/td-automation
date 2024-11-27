using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [field: SerializeField] public int ID { get; set; }
    [SerializeField] private float _damage;

    [Header("References")]
    [SerializeField] private Health _health;
    [SerializeField] private PathFollow _pathFollow;
    [SerializeField] private HitFlash _hitFlash;

    public Action<Enemy> OnDied { get; set; }
    public Action<Enemy> OnPathReached { get; set; }

    public float Damage => _damage;

    public Health Health => _health;
    public PathFollow PathFollow => _pathFollow;


    public void Awake()
    {
        _health.OnDied += Died;
        _health.OnDamageTaken += DamageTaken;
        _pathFollow.OnPathFinished += PathFinished;
    }

    public void Destroy()
    {
        _health.OnDied -= Died;
        _health.OnDamageTaken -= DamageTaken;
        _pathFollow.OnPathFinished -= PathFinished;
    }

    public void ResetIt()
    {
        _health.ResetHealth();
        _pathFollow.ResetIt();
    }

    private void Died()
    {
        OnDied?.Invoke(this);
    }

    private void DamageTaken(float _, IDamageable __)
    {
        _hitFlash.DoFlash();
    }

    private void PathFinished()
    {
        OnPathReached?.Invoke(this);
    }
}
