using UnityEngine;

public abstract class Tower : Building
{
    [Header("Tower")]
    [SerializeField] private Transform _range;
    [SerializeField] protected float _damage = 1f;
    [SerializeField] protected float _damageRate = 1f;
    [SerializeField] protected Transform _head;

    [Header("States")]
    [SerializeField] private float _rotationArc = 30f;
    [SerializeField] private float _rotationSpeed = 1.57f;
    [SerializeField] private float _stopDuration = .75f;

    protected float _fireRateElapsedTime;

    private float _stoppedAngle;
    private float _idleElapsedTime = 1f;
    private float _stopElapsedTime;

    public override bool CanBeRotated => false;
    public override bool CanBeDestroyed => true;

    protected float Range => _range.localScale.x * .5f;

    protected virtual void Update()
    {
        var deltaTime = Time.deltaTime;
        UpdateState(deltaTime);

        _fireRateElapsedTime += deltaTime;
        if (!EnemyManager.Instance.HasEnemies || _fireRateElapsedTime < _damageRate)
            return;

        float closestDistance = float.MaxValue;
        Enemy closestEnemy = null;
        foreach (Enemy enemy in EnemyManager.Instance.Enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
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

        _fireRateElapsedTime = 0f;
        Vector2 direction = (closestEnemy.transform.position - transform.position).normalized;
        _head.transform.eulerAngles = new(
            0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f
        );
        StopIdle();

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

    protected void StopIdle()
    {
        _stoppedAngle = _head.transform.eulerAngles.z;
        _idleElapsedTime = 0f;
    }

    protected void UpdateState(float deltaTime)
    {
        if (_idleElapsedTime <= float.Epsilon)
        {
            _stopElapsedTime += deltaTime;
            if (_stopElapsedTime >= _stopDuration)
            {
                _stopElapsedTime = 0f;
            }
            else
                return;
        }

        _idleElapsedTime += deltaTime;
        _head.eulerAngles = new(0f, 0f,
            _stoppedAngle + Mathf.Sin(_idleElapsedTime * _rotationSpeed) * _rotationArc
        );
    }

    protected abstract void DamageEnemy(Enemy enemy);
}
