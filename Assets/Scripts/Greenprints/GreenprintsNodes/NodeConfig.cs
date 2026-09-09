using System.Collections.Generic;
using UnityEngine;

public abstract class NodeConfig : MonoBehaviour
{
    [SerializeField] private Greenprints _greenprints;

    [SerializeField] private NodeAffiliation _nodeAffiliation;

    [SerializeField] private string _nodeName;
    [SerializeField] private string _titleName;

    [SerializeField] private PinConfig[] _inputPins;
    [SerializeField] private PinConfig[] _outputPins;

    [SerializeField] private float _delayNextNode = 0.02f;

    public Greenprints Greenprints => _greenprints;

    public NodeAffiliation NodeAffiliation => _nodeAffiliation;

    public string NodeName => _nodeName;
    public string TitleName => _titleName;

    public PinConfig[] InputPins => _inputPins;
    public PinConfig[] OutputPins => _outputPins;

    public float DelayNextNode => _delayNextNode;

    public abstract void Active(Node node);

    public Node GetNode(Vector2 position, Greenprints greenprints)
    {
        return new Node(this, _inputPins, _outputPins, position, greenprints);
    }
}
