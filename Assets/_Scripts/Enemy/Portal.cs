using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Portal : MonoBehaviour
{
    [SerializeField] private Path<SegmentBehaviour> _path;
    [SerializeField] private Wave[] _waves;
    private int _currentEnemyGroup;

    public IPath Path => _path;

    public bool AllGroupsSpawned(int currentWave)
    {
        return GetStage(currentWave).EnemySpawns.Length == _currentEnemyGroup;
    }

    public bool CanSpawn(int currentWave, float elapsedTime)
    {
        if (AllGroupsSpawned(currentWave))
            return false;

        return GetStage(currentWave).EnemySpawns[_currentEnemyGroup].CanSpawn(elapsedTime);
    }

    public Enemy GetEnemy(int currentWave)
    {
        return GetStage(currentWave).EnemySpawns[_currentEnemyGroup++].Enemy;
    }

    public Enemy GetRandomEnemy(int currentWave)
    {
        return GetStage(currentWave).GetRandomEnemy();
    }

    private Wave GetStage(int currentWave)
    {
        int index = Mathf.Clamp(currentWave, 0, _waves.Length - 1);
        return _waves[index];
    }

    public void StageEnded()
    {
        _currentEnemyGroup = 0;
    }

#if UNITY_EDITOR
    public void OnDrawGizmosSelected()
    {
        foreach (SegmentBehaviour segment in _path.Segments)
        {
            if (segment == null)
                continue;

            segment.OnDrawGizmosSelected();
        }
    }
#endif

    [Serializable]
    public class Wave
    {
        [SerializeField] private EnemySpawn[] _enemySpawns;

        public EnemySpawn[] EnemySpawns => _enemySpawns;

        public Enemy GetRandomEnemy() => _enemySpawns[Random.Range(0, _enemySpawns.Length)].Enemy;

        [Serializable]
        public class EnemySpawn
        {
            [SerializeField] private SOObjectPoolEnemy _enemyPool;
            [SerializeField] private float _spawnTime;

            public Enemy Enemy => _enemyPool.ObjectPool.GetObject();

            public bool CanSpawn(float elapsedTime) => elapsedTime > _spawnTime;
        }
    }

}
