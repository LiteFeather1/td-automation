using System.Collections.Generic;
using UnityEngine;

public class TowerZapper : Tower
{
    [Header("Zapper")]
    [SerializeField] private float _multiplierPerEnemyInRange = .25f;
    
    protected override void Update()
    {
        if (!EnemyManager.Instance.HasEnemies)
            return;

        var enemiesInRange = new List<Enemy>();
        foreach (var enemy in EnemyManager.Instance.Enemies)
        {
            if (Vector2.Distance(enemy.transform.position, transform.position) < Range)
                enemiesInRange.Add(enemy);
        }

        if (enemiesInRange.Count == 0)
            return;

        _elapsedTime += Time.deltaTime * (1f + (_multiplierPerEnemyInRange * enemiesInRange.Count));
        if (_elapsedTime < _damageRate)
            return;

        foreach (var enemy in enemiesInRange)
            DamageEnemy(enemy);

        _elapsedTime %= _damageRate;
    }

    protected override void DamageEnemy(Enemy enemy)
    {
        enemy.Health.TakeDamage(_damage);
    }
}
