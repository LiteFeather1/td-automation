using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Portal : MonoBehaviour
{
    [SerializeField] private Path<SegmentBehaviour> _path;
    [SerializeField] private Stage[] _stages;
    private int _currentEnemyGroup;

    public IPath Path => _path;

    public bool AllGroupsSpawned(int currentStage)
    {
        return GetStage(currentStage).EnemySpawns.Length == _currentEnemyGroup;
    }

    public bool CanSpawn(int currentStage, float elapsedTime)
    {
        if (AllGroupsSpawned(currentStage))
            return false;

        return GetStage(currentStage).EnemySpawns[_currentEnemyGroup].CanSpawn(elapsedTime);
    }

    public Enemy GetEnemy(int currentStage)
    {
        return GetStage(currentStage).EnemySpawns[_currentEnemyGroup++].Enemy;
    }

    public Enemy GetRandomEnemy(int currentStage)
    {
        return GetStage(currentStage).GetRandomEnemy();
    }

    private Stage GetStage(int currentStage)
    {
        int index = Mathf.Clamp(currentStage, 0, _stages.Length - 1);
        return _stages[index];
    }

    public void StageEnded()
    {
        _currentEnemyGroup = 0;
    }

    public void OnDrawGizmosSelected()
    {
        foreach (SegmentBehaviour segment in _path.Segments)
        {
            if (segment == null)
                continue;

            segment.OnDrawGizmosSelected();
        }
    }

    [Serializable]
    public class Stage
    {
        [SerializeField] private EnemySpawn[] _enemySpawns;

        public EnemySpawn[] EnemySpawns => _enemySpawns;

        public Enemy GetRandomEnemy() => _enemySpawns[Random.Range(0, _enemySpawns.Length)].Enemy;
    }

    [Serializable]
    public class EnemySpawn
    {
        [SerializeField] private float _spawnTime;
        [SerializeField] private SOObjectPoolEnemy _enemyPool;

        public Enemy Enemy => _enemyPool.ObjectPool.GetObject();

        public bool CanSpawn(float elapsedTime) => elapsedTime > _spawnTime;
    }
}
