using System.Collections.Generic;
using UnityEngine;

public class BeltPathSystem : MonoBehaviour
{
    private static readonly Vector2Int[] sr_directions = new Vector2Int[4]
    {
        new(-1, 0), new(1, 0), new (0, 1), new(0, -1)
    };
    private static readonly Dictionary<Direction, Vector2Int> sr_directionToVector = new()
    {
        { Direction.Left, sr_directions[0] },
        { Direction.Right, sr_directions[1] },
        { Direction.Up, sr_directions[2] },
        { Direction.Down, sr_directions[3] },
    };

    private readonly Dictionary<Vector2Int, BeltPath> r_positionToBelt = new();
    private readonly Dictionary<Vector2Int, ResourceCollector> r_positionToCollectors = new();

    public void AddBelt(BeltPath newBelt)
    {
        r_positionToBelt.Add(newBelt.Position, newBelt);

        if (r_positionToBelt.Count == 0)
            return;

        if (r_positionToBelt.TryGetValue(
            newBelt.Position + sr_directionToVector[newBelt.OutDirection], out var outBelt
        ))
        {
            newBelt.Input = outBelt;
        }

        foreach (var direction in sr_directions)
        {
            if (r_positionToBelt.TryGetValue(newBelt.Position + direction, out var belt))
            {
                if (newBelt.Position == belt.Position + sr_directionToVector[belt.OutDirection])
                {
                    belt.Input = newBelt;
                }
            }
            else if (r_positionToCollectors.TryGetValue(newBelt.Position + direction, out var collector))
            {
                if (newBelt.Position == collector.Position + sr_directionToVector[collector.OutDirection])
                {
                    collector.Input = newBelt;
                }
            }
        }
    }

    public void AddCollector(ResourceCollector newCollector)
    {
        r_positionToCollectors.Add(newCollector.Position, newCollector);

        if (r_positionToBelt.TryGetValue(
            newCollector.Position + sr_directionToVector[newCollector.OutDirection], out var belt
        ))
        {
            newCollector.Input = belt;
        }
    }
}
