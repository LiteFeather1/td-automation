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
    [SerializeField] private ObjectPool<ParticleStoppedCallback> _enemyHitParticlePool;
    [SerializeField] private ObjectPool<ParticleStoppedCallback> _enemyDiedParticlePool;
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
        foreach (KeyValuePair<int, SOObjectPoolEnemy> pool in _enemiesObjectPools)
        {
            ObjectPool<Enemy> objectPool = pool.Value.ObjectPool;
            objectPool.Object.ID = pool.Key;
            objectPool.ObjectCreated += EnemyCreated;
            objectPool.InitPool();
        }

        _enemyHitParticlePool.ObjectCreated += HitParticleCreated;
        _enemyHitParticlePool.InitPool();

        _enemyDiedParticlePool.ObjectCreated += DiedParticleCreated;
        _enemyDiedParticlePool.InitPool();
    }

    private void Start()
    {
        _lullSpawnTime = _lullStages[_currentStage].LullSpawnTime;
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        float currentLullDuration = _lullStages[_currentStage].LullDuration;
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

        foreach (Portal portal in _portals)
        {
            if (!portal.CanSpawn(_currentStage, _elapsedTime - currentLullDuration))
                continue;

            SpawnEnemy(portal, portal.GetEnemy(_currentStage));
        }

        void SpawnEnemy(Portal portal, Enemy enemy)
        {
            enemy.transform.position = portal.transform.position;
            enemy.ResetIt();
            enemy.PathFollow.SetPath(portal.Path);
            enemy.gameObject.SetActive(true);
            _enemies.Add(enemy);
        }
    }

    private void OnDisable()
    {
        foreach (SOObjectPoolEnemy pool in _enemiesObjectPools.Values)
        {
            pool.ObjectPool.ObjectCreated -= EnemyCreated;

            foreach (Enemy enemy in pool.ObjectPool.Objects)
            {
                enemy.OnDamaged -= EnemyDamaged;
                enemy.OnDied -= EnemyDied;
                enemy.OnPathReached -= EnemyReachedPathEnd;
            }

            pool.ObjectPool.Dispose();
        }

        _enemyHitParticlePool.ObjectCreated -= HitParticleCreated;
        _enemyHitParticlePool.Dispose();

        _enemyDiedParticlePool.ObjectCreated -= HitParticleCreated;
        _enemyDiedParticlePool.Dispose();
    }

    private void EnemyCreated(Enemy enemy)
    {
        enemy.OnDamaged += EnemyDamaged;
        enemy.OnDied += EnemyDied;
        enemy.OnPathReached += EnemyReachedPathEnd;
    }

    private void RemoveEnemy(Enemy enemy)
    {
        _enemies.Remove(enemy);
        enemy.gameObject.SetActive(false);
        _enemiesObjectPools[enemy.ID].ObjectPool.ReturnObject(enemy);

        PlayParticle(enemy, _enemyDiedParticlePool);

        if (_enemies.Count != 0)
            return;

        foreach (Portal portal in _portals)
        {
            if (!portal.AllGroupsSpawned(_currentStage))
                return;
        }

        foreach (Portal portal in _portals)
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

    private void ParticleCreated(ParticleStoppedCallback ps, ObjectPool<ParticleStoppedCallback> pool)
    {
        ps.OnStopped += ReturnHitParticle;

        void ReturnHitParticle(ParticleStoppedCallback psHit)
        {
            psHit.gameObject.SetActive(false);
            pool.ReturnObject(psHit);
        }
    }

    private void HitParticleCreated(ParticleStoppedCallback ps)
    {
        ParticleCreated(ps, _enemyHitParticlePool);
    }

    private void DiedParticleCreated(ParticleStoppedCallback ps)
    {
        ParticleCreated(ps, _enemyDiedParticlePool);
    }

    private void PlayParticle(Enemy enemy, ObjectPool<ParticleStoppedCallback> pool)
    {
        ParticleStoppedCallback ps = pool.GetObject();
        ps.transform.position = enemy.transform.position;
        ps.gameObject.SetActive(true);
    }

    private void EnemyDamaged(Enemy enemy)
    {
        PlayParticle(enemy, _enemyHitParticlePool);
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
