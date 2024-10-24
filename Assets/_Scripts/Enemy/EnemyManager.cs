using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private float[] _lullStageDurations;
    [SerializeField] private Portal[] _portals;
    private int _currentStage;
    private int _enemyCount;

    private float _elapsedTime;

    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        var currentLullDuration = _lullStageDurations[_currentStage];
        if (_elapsedTime < currentLullDuration)
            return;

        foreach (var portal in _portals)
        {
            if (!portal.CanSpawn(_currentStage, _elapsedTime - currentLullDuration))
                return;

            foreach (var enemy in portal.GetEnemies(_currentStage))
            {
                _enemyCount++;
                var newEnemy = Instantiate(
                    enemy, portal.transform.position, Quaternion.identity
                );

                newEnemy.OnDied += RemoveEnemy;
                newEnemy.OnPathReached += RemoveEnemy;
            }
        }
    }

    private void RemoveEnemy(Enemy enemy)
    {
        _enemyCount--;
        Destroy(enemy.gameObject);

        if (_enemyCount != 0)
            return;

        foreach (var portal in _portals)
        {
            if (!portal.AllGroupsSpawned(_currentStage))
                return;
        }

        Debug.Log("Stage ended");
    }
}
