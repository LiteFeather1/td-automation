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

    public void AddPoint(BeltPath newBelt)
    {
        if (r_positionToBelt.Count == 0)
        {
            r_positionToBelt.Add(newBelt.Position, newBelt);
            return;
        }

        foreach (var direction in sr_directions)
        {
            if (!r_positionToBelt.TryGetValue(newBelt.Position + direction, out var belt))
                continue;

            if (newBelt.Position == belt.Position + sr_directionToVector[belt.OutDirection])
                belt.OutBelt = newBelt;
        }

        r_positionToBelt.Add(newBelt.Position, newBelt);
    }
}
