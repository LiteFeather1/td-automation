using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private float _range;
    [SerializeField] private float _damage = 1f;
    [SerializeField] private float _damageRate = 1f;
    private float _elapsedTime;

    public void Update()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime < _damageRate)
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

        print("Damaged Enemy");
        _elapsedTime = 0f;
        closestEnemy.Health.TakeDamage(_damage);
    }
}
