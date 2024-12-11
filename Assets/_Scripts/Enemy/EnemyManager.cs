using System;
using System.Collections.Generic;
using UnityEngine;
using LTF.SerializedDictionary;
using Random = UnityEngine.Random;

public class EnemyManager : Singleton<EnemyManager>
{
    [SerializeField] private LullStages[] _lullStages;
    [SerializeField] private Portal[] _portals;
    [SerializeField] private SerializedDictionary<int, SOObjectPoolEnemy> _enemiesObjectPools;
    private int _currentStage;
    private bool _waveInProgress;
    private readonly List<Enemy> _enemies = new();

    private float _elapsedTime;
    private float _lullSpawnTime;

    public Action OnWaveStarted { get; set; }
    public Action<int> OnStageEnded { get; set; }
    public Action OnAllStagesEnded { get; set; }
    public Action<float> OnEnemyReachedPathEnd { get; set; }
    public Action OnEnemyKilled { get; set; }

    public List<Enemy> Enemies => _enemies;

    public int CurrentStage => _currentStage;
    public bool WaveInProgress => _waveInProgress;

    public bool HasEnemies => _enemies.Count > 0;

    public float TimeToWave => _lullStages[_currentStage].LullDuration - _elapsedTime;
    public bool AllStagesCompleted => _currentStage == _lullStages.Length;

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

    private void Start()
    {
        _lullSpawnTime = _lullStages[_currentStage].LullSpawnTime;
    }

    public void Update()
    {
        _elapsedTime += Time.deltaTime;
        var currentLullDuration = _lullStages[_currentStage].LullDuration;
        if (_elapsedTime < currentLullDuration)
        {
            if (_elapsedTime >= _lullSpawnTime)
            {
                _lullSpawnTime = _lullStages[_currentStage].LullSpawnTime + _elapsedTime;
                Portal portal;
                do
                    portal = _portals[Random.Range(0, _portals.Length)];
                while (portal.AllGroupsSpawned(_currentStage));

                SpawnEnemy(portal, portal.GetRandomEnemy(_currentStage));
            }

            return;
        }

        if (!_waveInProgress)
        {
            _waveInProgress = true;
            OnWaveStarted?.Invoke();
        }

        foreach (var portal in _portals)
        {
            if (!portal.CanSpawn(_currentStage, _elapsedTime - currentLullDuration))
                continue;

            SpawnEnemy(portal, portal.GetEnemy(_currentStage));
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

        print("Stage ended");
        _elapsedTime = 0f;
        _currentStage++;
        _waveInProgress = false;
        OnStageEnded?.Invoke(_currentStage);

        if (AllStagesCompleted)
        {
            print("All Stages Completed");
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

    private void SpawnEnemy(Portal portal, Enemy enemy)
    {
        enemy.transform.position = portal.transform.position;
        enemy.ResetIt();
        enemy.PathFollow.SetPath(portal.Path);
        enemy.gameObject.SetActive(true);
        _enemies.Add(enemy);
    }

    [Serializable]
    private struct LullStages
    {
        [SerializeField] private float _lullDuration;
        [SerializeField] private Vector2 _lullSpawnTimeRange;

        public readonly float LullDuration => _lullDuration;
        public readonly float LullSpawnTime
            => Random.Range(_lullSpawnTimeRange.x, _lullSpawnTimeRange.y);
    }
}
