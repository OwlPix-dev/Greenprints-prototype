using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class NodeConfig
{
    [SerializeField] private string _nodeName;

    [SerializeField] private PinConfig[] _inputPins;
    [SerializeField] private PinConfig[] _outputPins;

    [SerializeField] private float _workTime = 0.02f;

    public string NodeName => _nodeName;
    public float WorkTime => _workTime;

    public abstract void Active(Node node);

    public Node GetNode(Vector2 position)
    {
        return new Node(this, _inputPins, _outputPins, position);
    }
}
