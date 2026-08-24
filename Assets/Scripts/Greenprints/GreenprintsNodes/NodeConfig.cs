using System.Collections.Generic;
using UnityEngine;

public abstract class NodeConfig : MonoBehaviour
{
    [SerializeField] private string _nodeName;

    [SerializeField] private PinConfig[] _inputPins;
    [SerializeField] private PinConfig[] _outputPins;

    [SerializeField] private float _delayNextNode = 0.02f;

    public string NodeName => _nodeName;
    public float DelayNextNode => _delayNextNode;

    public abstract void Active(Node node);

    public Node GetNode(Vector2 position, Greenprints greenprints)
    {
        return new Node(this, _inputPins, _outputPins, position, greenprints);
    }
}
