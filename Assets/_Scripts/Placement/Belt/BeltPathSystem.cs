using System.Collections.Generic;
using UnityEngine;

public class BeltPathSystem : MonoBehaviour
{
    private readonly Dictionary<Vector2Int, BeltPath> r_positionToBelt = new();
    private static readonly Dictionary<Direction, Vector2Int> sr_directionToVector = new()
    {
        { Direction.Left, new(-1, 0) },
        { Direction.Right, new(1, 0) },
        { Direction.Up, new (0, 1) },
        { Direction.Down, new(0, -1) },
    };
    
    public void AddPoint(Vector2Int point, BeltPath beltPath)
    {
        if (r_positionToBelt.Count == 0)
        {
            r_positionToBelt.Add(point, beltPath);
            return;
        }

        if (beltPath.InDirection!= Direction.None
            && r_positionToBelt.TryGetValue(sr_directionToVector[beltPath.InDirection], out var inBelt))
            beltPath.InBelt = inBelt;

        if (beltPath.OutDirection != Direction.None
            && r_positionToBelt.TryGetValue(sr_directionToVector[beltPath.OutDirection], out var outBelt))
            beltPath.OutBelt = outBelt;

        r_positionToBelt.Add(point, beltPath);
    }
}
