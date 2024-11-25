using System;
using System.Collections.Generic;
using UnityEngine;
using LTF.SerializedDictionary;

public class EnemyManager : Singleton<EnemyManager>
{
    [SerializeField] private float[] _lullStageDurations;
    [SerializeField] private Portal[] _portals;
    [SerializeField] private SerializedDictionary<int, SOObjectPoolEnemy> _enemiesObjectPools;
    private int _currentStage;
    private bool _waveInProgress;
    private readonly List<Enemy> _enemies = new();

    private float _elapsedTime;

    public Action OnWaveStarted { get; set; }
    public Action<int> OnStageEnded { get; set; }
    public Action OnAllStagesEnded { get; set; }
    public Action<float> OnEnemyReachedPathEnd { get; set; }
    public Action OnEnemyKilled { get; set; }

    public List<Enemy> Enemies => _enemies;

    public int CurrentStage => _currentStage;
    public bool WaveInProgress => _waveInProgress;

    public bool HasEnemies => _enemies.Count > 0;

    public float TimeToWave => _lullStageDurations[_currentStage] - _elapsedTime;
    public bool AllStagesCompleted => _currentStage == _lullStageDurations.Length;

    internal void OnEnable()
    {
        foreach (var pool in _enemiesObjectPools)
        {
            var objectPool = pool.Value.ObjectPool;
            objectPool.Object.ID = pool.Key;
            objectPool.ObjectCreated += EnemyCreated;
            objectPool.InitPool();
        }
    }

    public void Update()
    {
        _elapsedTime += Time.deltaTime;
        var currentLullDuration = _lullStageDurations[_currentStage];
        if (_elapsedTime < currentLullDuration)
            return;

        if (!_waveInProgress)
        {
            _waveInProgress = true;
            OnWaveStarted?.Invoke();
        }

        foreach (var portal in _portals)
        {
            if (!portal.CanSpawn(_currentStage, _elapsedTime - currentLullDuration))
                continue;

            var newEnemy = portal.GetEnemy(_currentStage);
            newEnemy.transform.position = portal.transform.position;
            newEnemy.ResetIt();
            newEnemy.PathFollow.SetPath(portal.Path);
            newEnemy.gameObject.SetActive(true);

            _enemies.Add(newEnemy);
        }
    }

    public void OnDisable()
    {
        foreach (var pool in _enemiesObjectPools.Values)
        {
            pool.ObjectPool.ObjectCreated -= EnemyCreated;

            foreach (var enemy in pool.ObjectPool.Objects)
            {
                enemy.OnDied -= EnemyDied;
                enemy.OnPathReached -= EnemyReachedPathEnd;
            }

            pool.ObjectPool.Dispose();
        }
    }

    private void EnemyCreated(Enemy enemy)
    {
        enemy.OnDied += EnemyDied;
        enemy.OnPathReached += EnemyReachedPathEnd;
    }

    private void RemoveEnemy(Enemy enemy)
    {
        _enemies.Remove(enemy);
        enemy.gameObject.SetActive(false);
        _enemiesObjectPools[enemy.ID].ObjectPool.ReturnObject(enemy);

        if (_enemies.Count != 0)
            return;

        foreach (var portal in _portals)
        {
            if (!portal.AllGroupsSpawned(_currentStage))
                return;
        }

        foreach (var portal in _portals)
            portal.StageEnded();

        Debug.Log("Stage ended");
        _elapsedTime = 0f;
        _currentStage++;
        _waveInProgress = false;
        OnStageEnded?.Invoke(_currentStage);
        if (_currentStage == _lullStageDurations.Length)
        {
            Debug.Log("All Stages Completed");
            enabled = false;
            OnAllStagesEnded?.Invoke();
        }
    }

    private void EnemyDied(Enemy enemy)
    {
        OnEnemyKilled?.Invoke();
        RemoveEnemy(enemy);
    }

    private void EnemyReachedPathEnd(Enemy enemy)
    {
        OnEnemyReachedPathEnd?.Invoke(enemy.Damage);
        RemoveEnemy(enemy);
    }
}
