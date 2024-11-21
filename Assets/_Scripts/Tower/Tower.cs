using UnityEngine;

public class Tower : Building
{
    [SerializeField] private float _range = 1f;
    [SerializeField] private float _damage = 1f;
    [SerializeField] private float _damageRate = 1f;
    private float _elapsedTime;

    public override bool CanBeRotated => false;
    public override bool CanBeDestroyed => true;

    public void Update()
    {
        _elapsedTime += Time.deltaTime;
        if (!EnemyManager.Instance.HasEnemies || _elapsedTime < _damageRate)
            return;

        var closestDistance = float.MaxValue;
        Enemy closestEnemy = null;
        foreach (var enemy in EnemyManager.Instance.Enemies)
        {
            var distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance > _range)
                continue;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy == null)
            return;

        _elapsedTime = 0f;
        closestEnemy.Health.TakeDamage(_damage);
    }
}
