using UnityEngine;

public class TowerProjectile : Tower
{
    [Header("Projectile Tower")]
    [SerializeField] private LineRenderer _line;
    [SerializeField] private Transform _firePoint;

    protected override void DamageEnemy(Enemy enemy)
    {
        _line.SetPosition(0, _firePoint.position);
        _line.SetPosition(1, enemy.transform.position);
        enemy.Health.TakeDamage(_damage);
    }
}
