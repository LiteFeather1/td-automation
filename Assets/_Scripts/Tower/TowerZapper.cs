using UnityEngine;

public class TowerZapper : Tower
{
    [Header("Zapper")]
    [SerializeField] private float _multiplierPerEnemyInRange = .25f;

    protected override void Update()
    {
        if (!EnemyManager.Instance.HasEnemies)
            return;

        int enemiesInRange = 0;
        foreach (Enemy enemy in EnemyManager.Instance.Enemies)
        {
            if (Vector2.Distance(enemy.transform.position, transform.position) < Range)
                enemiesInRange++;
        }

        if (enemiesInRange == 0)
            return;

        _elapsedTime += Time.deltaTime * (1f + (_multiplierPerEnemyInRange * enemiesInRange));
        if (_elapsedTime < _damageRate)
            return;

        _elapsedTime %= _damageRate;

        Vector2[] positions = new Vector2[enemiesInRange];
        for (int i = 0; i < EnemyManager.Instance.Enemies.Count; i++)
        {
            Enemy enemy = EnemyManager.Instance.Enemies[i];
            if (Vector2.Distance(enemy.transform.transform.position, transform.position) > Range)
                continue;

            DamageEnemy(enemy);
            positions[i] = enemy.transform.position;
        }

        ZapEffectManager.Instance.ZapEffect(transform.position, positions);
    }

    protected override void DamageEnemy(Enemy enemy)
    {
        enemy.Health.TakeDamage(_damage);
    }
}
