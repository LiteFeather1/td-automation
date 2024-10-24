using System;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private Path _path;
    [SerializeField] private Stage[] _stages;
    private int _currentEnemyGroup;

    public Path Path => _path;

    public bool AllGroupsSpawned(int currentStage)
    {
        return GetStage(currentStage).EnemyGroups.Length == _currentEnemyGroup;
    }

    public bool CanSpawn(int currentStage, float elapsedTime)
    {
        if (AllGroupsSpawned(currentStage))
            return false;

        return GetStage(currentStage).EnemyGroups[_currentEnemyGroup].CanSpawn(elapsedTime);
    }

    public Enemy[] GetEnemies(int currentStage)
    {
        return GetStage(currentStage).EnemyGroups[_currentEnemyGroup++].Enemies;
    }

    private Stage GetStage(int currentStage)
    {
        return _stages[Mathf.Min(currentStage, _stages.Length - 1)];
    }

    public void StageEnded()
    {
        _currentEnemyGroup = 0;
    }

    [Serializable]
    public class Stage
    {
        [SerializeField] private EnemyGroup[] _enemyGroups;

        public EnemyGroup[] EnemyGroups => _enemyGroups;
    }

    [Serializable]
    public class EnemyGroup
    {
        [SerializeField] private float _spawnTime;
        [SerializeField] private Enemy[] _enemies;

        public Enemy[] Enemies => _enemies;

        public bool CanSpawn(float elapsedTime) => elapsedTime > _spawnTime;
    }
}
