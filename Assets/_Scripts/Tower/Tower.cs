using UnityEngine;

public abstract class Tower : Building
{
    [Header("Tower")]
    [SerializeField] private Transform _range;
    [SerializeField] protected float _damage = 1f;
    [SerializeField] protected float _damageRate = 1f;
    private float _elapsedTime;

    public override bool CanBeRotated => false;
    public override bool CanBeDestroyed => true;

    private float Range => _range.localScale.x * .5f;

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
            if (distance > Range)
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
        DamageEnemy(closestEnemy);
    }

    public override void Place()
    {
        _range.gameObject.SetActive(false);
    }

    public override void Hover()
    {
        _range.gameObject.SetActive(true);
    }

    public override void Unhover()
    {
        _range.gameObject.SetActive(false);
    }

    protected abstract void DamageEnemy(Enemy enemy);
}
