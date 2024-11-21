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
                newOutPort.Port = inPort;
            }
        }

        foreach (var direction in sr_directions)
        {
            if (r_inPorts.TryGetValue(newInPort.Position + direction, out var inPort))
            {
                if (inPort is IOutPort outPort
                    && newInPort.Position == inPort.Position + sr_direction[outPort.OutDirection])
                {
                    outPort.Port = newInPort;
                }
            }
            else if (r_outPort.TryGetValue(newInPort.Position + direction, out var outPort))
            {
                if (newInPort.Position == outPort.Position + sr_direction[outPort.OutDirection])
                {
                    outPort.Port = newInPort;
                }
            }
        }
    }

    public void AddOutPort(IOutPort newOutPort)
    {
        r_outPort.Add(newOutPort.Position, newOutPort);

        if (r_inPorts.TryGetValue(
            newOutPort.Position + sr_direction[newOutPort.OutDirection], out var inPort
        ))
        {
            newOutPort.Port = inPort;
        }
    }

    public void TryRemovePosition(Vector2Int position)
    {
        if (r_inPorts.ContainsKey(position))
            r_inPorts.Remove(position);
        else if (r_outPort.ContainsKey(position))
            r_outPort.Remove(position);
    }
}
