using System.Collections.Generic;
using UnityEngine;

public class BeltPathSystem : MonoBehaviour
{
    private static readonly Vector2Int[] sr_vectors = new Vector2Int[4]
    {
        new(-1, 0), new(1, 0), new (0, 1), new(0, -1)
    };

    private static readonly Dictionary<Direction, Vector2Int> sr_directionToVector = new()
    {
        { Direction.Left, sr_vectors[0] },
        { Direction.Right, sr_vectors[1] },
        { Direction.Up, sr_vectors[2] },
        { Direction.Down, sr_vectors[3] },
    };

    private static readonly Dictionary<Vector2Int, Direction> sr_vectorToDirection = new()
    {
        { sr_vectors[0], Direction.Left },
        { sr_vectors[1], Direction.Right },
        { sr_vectors[2], Direction.Up },
        { sr_vectors[3], Direction.Down },
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

        if (newInPort.InDirection == Direction.Any)
        {
            if (newInPort is not IOutPort newOutPort)
                return;

            foreach (var directionVector in sr_directionToVector)
            {
                if (directionVector.Key == newOutPort.OutDirection
                    || !r_outPort.TryGetValue(newInPort.Position + directionVector.Value, out var outPort)
                )
                    continue;

                if (outPort.OutDirection == Direction.Any
                    || outPort.OutDirection == sr_vectorToDirection[newInPort.Position - outPort.Position]
                )
                {
                    outPort.SetPort(newInPort);
                }
            }
        }
        else if (r_outPort.TryGetValue(
            newInPort.Position + sr_directionToVector[newInPort.InDirection], out var outPort
        ))
        {
            if (outPort.OutDirection == Direction.Any)
            {
                if (outPort is not IInPort inPort
                    || newInPort.Position == outPort.Position + sr_directionToVector[inPort.InDirection]
                )
                    return;

                outPort.SetPort(newInPort);
            }
            else if (outPort.OutDirection == sr_directionToOpposite[newInPort.InDirection])
            {
                outPort.SetPort(newInPort);
            }
        }
    }

    public void AddOutPort(IOutPort newOutPort)
    {
        r_outPort.Add(newOutPort.Position, newOutPort);

        if (newOutPort.OutDirection == Direction.Any)
        {
            foreach (var directionVector in sr_directionToVector)
            {
                if (directionVector.Key == newOutPort.OutDirection
                    || !r_inPorts.TryGetValue(newOutPort.Position + directionVector.Value, out var inPort)
                    || inPort.InDirection != sr_directionToOpposite[directionVector.Key]
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
        }
        else
        {
            if (r_inPorts.TryGetValue(
                newOutPort.Position + sr_directionToVector[newOutPort.OutDirection], out var inPort
            ) && (
                inPort.InDirection == Direction.Any
                ||  inPort.InDirection == sr_directionToOpposite[newOutPort.OutDirection]
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
