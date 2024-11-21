using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    [SerializeField] private float[] _lullStageDurations;
    [SerializeField] private Portal[] _portals;
    private int _currentStage;
    private readonly List<Enemy> _enemies = new();

    private float _elapsedTime;

    public Action OnStageEnded { get; set; }
    public Action OnAllStagesEnded { get; set; }
    public Action<float> OnEnemyReachedPathEnd { get; set; }

    public List<Enemy> Enemies => _enemies;

    public void Update()
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
                var newEnemy = Instantiate(
                    enemy, portal.transform.position, Quaternion.identity
                );
                _enemies.Add(newEnemy);

                newEnemy.PathFollow.SetPath(portal.Path);

                newEnemy.OnDied += RemoveEnemy;
                newEnemy.OnPathReached += EnemyReachedPathEnd;
            }
        }
    }

    public void OnDisable()
    {
        foreach (var enemy in _enemies)
        {
            enemy.OnDied -= RemoveEnemy;
            enemy.OnPathReached -= EnemyReachedPathEnd;
        }
    }

    private void RemoveEnemy(Enemy enemy)
    {
        _enemies.Remove(enemy);
        Destroy(enemy.gameObject);

        if (_enemies.Count != 0)
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

    private void EnemyReachedPathEnd(Enemy enemy)
    {
        OnEnemyReachedPathEnd?.Invoke(enemy.Damage);
        RemoveEnemy(enemy);
    }
}
