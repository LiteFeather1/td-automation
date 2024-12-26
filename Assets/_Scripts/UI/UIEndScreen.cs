using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using LTF.SerializedDictionary;

public class UIEndScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI t_result;
    [SerializeField] private Color _victoryColour, _defeatColour;
    [SerializeField] private UIEndScreenStat _elapsedTime;
    [SerializeField] private UIEndScreenStat _enemies;
    [SerializeField] private SerializedDictionary<ResourceType, UIEndScreenStat> _resourceStats;

    private int _enemiesKilledCount;
    private readonly Dictionary<ResourceType, int> r_resourceCount = new();

    public void Init()
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            r_resourceCount.Add(type, 0);
    }

    public void Enable(bool victory, float time)
    {
        // Add Colour for defeat and victory
        if (victory)
        {
            t_result.text = "Victory!";
            t_result.color = _victoryColour;
        }
        else
        {
            t_result.text = "Defeat!";
            t_result.color = _defeatColour;
        }

        _elapsedTime.SetStat($"{Mathf.FloorToInt(time / 60f):00}:{Mathf.FloorToInt(time % 60f):00}");
        _enemies.SetStat($"{_enemiesKilledCount}");

        foreach (KeyValuePair<ResourceType, int> resource in r_resourceCount)
            _resourceStats[resource.Key].SetStat($"{resource.Value:000000}");

        gameObject.SetActive(true);
    }

    public void AddEnemyKilled() => _enemiesKilledCount++;

    public void AddResource(ResourceType type, int amount) => r_resourceCount[type] += amount;

    public void AddResources(IDictionary<ResourceType, int> resouces)
    {
        foreach (KeyValuePair<ResourceType, int> resource in resouces)
            AddResource(resource.Key, -resource.Value);
    }

    public void ButtonExit()
    {
        SceneManager.LoadScene(SceneNamesIDs.MAIN_MENU_ID);
    }
}
