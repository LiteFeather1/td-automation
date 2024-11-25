using UnityEngine;

public class TowerProjectile : Tower
{
    [Header("Projectile Tower")]
    [SerializeField] private GameObject _projectile;

    protected override void DamageEnemy(Enemy enemy)
    {
        // TODO Spawn projectile to damage enemy
        enemy.Health.TakeDamage(_damage);
    }
}
