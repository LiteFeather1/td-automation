using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float _maxHP;
    [SerializeField] private float _defence;
    private float _hp;

    public float HP => _hp;
    public float MaxHP => _maxHP;
    public float Defence => _defence;

    public Action<float> OnDamageTaken { get; set;}
    public Action<float> OnHealed { get; set;}
    public Action OnDied { get; set; }

    private void Start()
    {
        _hp = _maxHP;
    }

    public void TakeDamage(float damage)
    {
        damage *= 100f / (100f + _defence);
        _hp -= damage;

        OnDamageTaken?.Invoke(damage);
        if (_hp < 0f)
        {
            OnDied?.Invoke();
        }
    }

    public void Heal(float heal)
    {
        if (_hp + heal > _maxHP)
            heal = _maxHP - _hp;

        _hp += heal;
        OnHealed?.Invoke(heal);
    }
}
