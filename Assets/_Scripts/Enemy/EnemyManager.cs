using System;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private float[] _lullStageDurations;
    [SerializeField] private Portal[] _portals;
    private int _currentStage;
    private int _enemyCount;

    private float _elapsedTime;

    public Action OnStageEnded { get; set; }
    public Action OnAllStagesEnded { get; set; }

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

                newEnemy.PathFollow.SetPath(portal.Path);

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
        _currentStage++;
        OnStageEnded?.Invoke();
        if (_currentStage == _lullStageDurations.Length)
        {
            Debug.Log("All Stages Completed");
            enabled = false;
            OnAllStagesEnded?.Invoke();
        }
    }
}
