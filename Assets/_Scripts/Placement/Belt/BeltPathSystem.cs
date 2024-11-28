using System.Collections.Generic;
using UnityEngine;

public class BeltPathSystem : MonoBehaviour
{
    private static readonly Vector2Int[] sr_directions = new Vector2Int[4]
    {
        new(-1, 0), new(1, 0), new (0, 1), new(0, -1)
    };
    private static readonly Dictionary<Direction, Vector2Int> sr_direction = new()
    {
        { Direction.Left, sr_directions[0] },
        { Direction.Right, sr_directions[1] },
        { Direction.Up, sr_directions[2] },
        { Direction.Down, sr_directions[3] },
    };

    private static readonly Dictionary<Direction, Direction> sr_directionToOpposite = new()
    {
        { Direction.Left, Direction.Right},
        { Direction.Right, Direction.Left},
        { Direction.Up, Direction.Down},
        { Direction.Down, Direction.Up},
    };

    private readonly Dictionary<Vector2Int, IInPort> r_inPorts = new();
    private readonly Dictionary<Vector2Int, IOutPort> r_outPort = new();

    public void AddIInPort(IInPort newInPort)
    {
        r_inPorts.Add(newInPort.Position, newInPort);

        if (r_inPorts.Count == 1)
            return;

        if (newInPort is IOutPort newOutPort)
        {
            if (r_inPorts.TryGetValue(
                newInPort.Position + sr_direction[newOutPort.OutDirection], out var inPort
            ))
            {
                newOutPort.SetPort(inPort, 0);
            }
        }

        foreach (var direction in sr_directions)
        {
            if (r_inPorts.TryGetValue(newInPort.Position + direction, out var inPort))
            {
                if (inPort is IOutPort outPort
                    && newInPort.Position == inPort.Position + sr_direction[outPort.OutDirection])
                {
                    outPort.SetPort(newInPort, 0);
                }
            }
            else if (r_outPort.TryGetValue(newInPort.Position + direction, out var outPort))
            {
                if (newInPort.Position == outPort.Position + sr_direction[outPort.OutDirection])
                {
                    outPort.SetPort(newInPort, 0);
                }
            }
        }
    }

    public void AddOutPort(IOutPort newOutPort)
    {
        r_outPort.Add(newOutPort.Position, newOutPort);

        if (newOutPort.OutDirection == Direction.Any)
        {
            foreach (var direction in sr_direction)
            {
                if (direction.Key == newOutPort.OutDirection
                    || !r_inPorts.TryGetValue(newOutPort.Position + direction.Value, out var inPort)
                    || inPort.InDirection != sr_directionToOpposite[direction.Key]
                )
                    continue;

                for (var i = 0; i < 3; i++)
                {
                    if (newOutPort.GetPort(i) == null)
                    {
                        newOutPort.SetPort(inPort, i);
                        break;
                    }
                }
            }

            return;
        }
        else
        {
            if (r_inPorts.TryGetValue(
                newOutPort.Position + sr_direction[newOutPort.OutDirection], out var inPort
            ))
            {
                newOutPort.SetPort(inPort, 0);
            }
        }
    }

    public void TryRemovePosition(Vector2Int position)
    {
        if (r_inPorts.ContainsKey(position))
            r_inPorts.Remove(position);

        if (r_outPort.ContainsKey(position))
            r_outPort.Remove(position);
    }
}
