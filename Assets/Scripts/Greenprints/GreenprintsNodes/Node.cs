using System.Collections.Generic;
using UnityEngine;

public class Node
{
    private Greenprints _greenprints;
    private NodeConfig _nodeConfig;
    private List<Pin> _pins = new List<Pin>();
    private Vector2 _position;

    public Greenprints Greenprints => _greenprints;
    public NodeConfig NodeConfig => _nodeConfig;
    public List<Pin> Pins => _pins;
    public Vector2 Position
    {
        get => _position;
        set => _position = value;
    }

    public Node(Greenprints greenprints, NodeConfig nodeConfig, Vector2 position)
    {
        _greenprints = greenprints;
        _nodeConfig = nodeConfig;
        _position = position;

        _pins = _nodeConfig.GetPins(this);
    }
}
