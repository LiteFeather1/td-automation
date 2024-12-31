using UnityEngine;

public class TowerFlamethrower : Tower
{
    [Header("Flamethrower tower")]
    [SerializeField] private float _fireDamageRadius = 2f;
    [SerializeField, Range(0f, 1f)] private float _fireDamagePercetage = .33f;
    [SerializeField] private ParticleSystem _ps;

    protected override void DamageEnemy(Enemy enemy)
    {
        _ps.Play();

        foreach (Enemy e in EnemyManager.Instance.Enemies)
        {
            if (Vector2.Distance(enemy.transform.position, e.transform.position) > _fireDamageRadius)
                continue;

            e.Health.TakeDamage(_damage * _fireDamagePercetage);
        }
    }
}
