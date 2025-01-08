using System.Collections.Generic;
using UnityEngine;

public class TowerZapper : Tower
{
    [Header("Zapper")]
    [SerializeField] private float _multiplierPerEnemyInRange = .25f;

    protected override void Update()
    {
        var deltaTime = Time.deltaTime;
        UpdateState(deltaTime);

        if (!EnemyManager.Instance.HasEnemies)
        {
            if (_fireRateElapsedTime <= _damageRate * .5f)
                _fireRateElapsedTime += deltaTime;

            return;
        }

        int enemiesInRange = 0;
        foreach (Enemy enemy in EnemyManager.Instance.Enemies)
        {
            if (Vector2.Distance(enemy.transform.position, transform.position) < Range)
                enemiesInRange++;
        }

        if (enemiesInRange == 0)
            return;

        _fireRateElapsedTime += deltaTime * (1f + (_multiplierPerEnemyInRange * enemiesInRange));
        if (_fireRateElapsedTime < _damageRate)
            return;

        _fireRateElapsedTime %= _damageRate;

        List<Vector2> positions = new(enemiesInRange);
        for (int i = 0; i < EnemyManager.Instance.Enemies.Count; i++)
        {
            Enemy enemy = EnemyManager.Instance.Enemies[i];
            if (Vector2.Distance(enemy.transform.transform.position, transform.position) > Range)
                continue;

            DamageEnemy(enemy);
            positions.Add(enemy.transform.position);
        }

        StopIdle();
        ZapEffectManager.Instance.ZapEffect(transform.position, positions);
    }

    protected override void DamageEnemy(Enemy enemy)
    {
        enemy.Health.TakeDamage(_damage);
    }
}
