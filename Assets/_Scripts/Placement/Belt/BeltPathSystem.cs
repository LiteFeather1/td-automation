using System.Collections.Generic;
using UnityEngine;

public class BeltPathSystem : MonoBehaviour
{
    private static readonly Vector2Int[] sr_vectors = new Vector2Int[4]
    {
        new(-1, 0), new(1, 0), new(0, 1), new(0, -1)
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

    [SerializeField] private float _moveItemSpeed = 2f;

    private readonly Dictionary<Vector2Int, IInPort> r_inPorts = new();
    private readonly Dictionary<Vector2Int, IOutPort> r_outPorts = new();

    internal void Update()
    {
        var deltaTime = Time.deltaTime;
        foreach (var inPort in r_inPorts.Values)
        {
            if (inPort.Resource == null)
                continue;

            var transform = inPort.Resource.transform;
            if (Vector2.Distance(inPort.Position, transform.position) > float.Epsilon)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position, inPort.Position, deltaTime * _moveItemSpeed
                );
            }
            else
            {
                inPort.ResourceCentralized();
            }
        }
    }

    public void AddInPort(IInPort newInPort)
    {
        r_inPorts.Add(newInPort.Position, newInPort);

        if (newInPort.InDirection == Direction.Any)
        {
            if (newInPort is not IOutPort newOutPort)
                return;

            foreach (var directionVector in sr_directionToVector)
            {
                if (directionVector.Key == newOutPort.OutDirection
                    || !r_outPorts.TryGetValue(newInPort.Position + directionVector.Value, out var outPort)
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
        else if (r_outPorts.TryGetValue(
            newInPort.Position + sr_directionToVector[newInPort.InDirection], out var outPort
        ))
        {
            if (outPort.OutDirection == Direction.Any)
            {
                if (outPort is not IInPort inPort
                    || inPort.InDirection == sr_vectorToDirection[newInPort.Position - inPort.Position]
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
        r_outPorts.Add(newOutPort.Position, newOutPort);

        if (newOutPort.OutDirection == Direction.Any)
        {
            if (newOutPort is not IInPort newInPort)
                return;

            foreach (var directionVector in sr_directionToVector)
            {
                if (newInPort.InDirection == directionVector.Key
                    || !r_inPorts.TryGetValue(newOutPort.Position + directionVector.Value, out var inPort)
                )
                    continue;

                if (inPort.InDirection == Direction.Any
                    || inPort.InDirection == sr_directionToOpposite[directionVector.Key]
                )
                {
                    newOutPort.SetPort(inPort);
                }
            }
        }
        else if (r_inPorts.TryGetValue(
            newOutPort.Position + sr_directionToVector[newOutPort.OutDirection], out var inPort
        ))
        {
            if (inPort.InDirection == Direction.Any)
            {
                if (inPort is not IOutPort outPort)
                    newOutPort.SetPort(inPort);
                else if (outPort.OutDirection == sr_vectorToDirection[newOutPort.Position - outPort.Position])
                    return;

                newOutPort.SetPort(inPort);
            }
            else if (inPort.InDirection == sr_directionToOpposite[newOutPort.OutDirection])
            {
                newOutPort.SetPort(inPort);
            }
        }
    }

    public void TryRemovePosition(Vector2Int position)
    {
        if (r_inPorts.ContainsKey(position))
            r_inPorts.Remove(position);

        if (r_outPorts.ContainsKey(position))
            r_outPorts.Remove(position);
    }

    public bool HasOutPortAt(Vector2Int position, Direction inDirection)
    {
        if (!r_outPorts.TryGetValue(position + sr_directionToVector[inDirection], out var outPort))
            return false;

        if (outPort.OutDirection == Direction.Any)
        {
            return outPort is IInPort inPort
                && sr_directionToOpposite[inDirection] != inPort.InDirection;
        }
        else
        {
            return inDirection == sr_directionToOpposite[outPort.OutDirection];
        }
    }

    public Direction OppositeDirection(Direction direction) => sr_directionToOpposite[direction];
}
