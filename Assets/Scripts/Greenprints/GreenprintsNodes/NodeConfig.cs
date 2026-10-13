using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;
using static UnityEngine.Rendering.DebugUI;

public abstract class NodeConfig : MonoBehaviour
{
    [SerializeField] private Greenprints _greenprints;

    [SerializeField] private NodeAffiliation _nodeAffiliation;

    [SerializeField] private string _nodeName;
    [SerializeField] private string _titleName;

    [SerializeField] private float _delayNextNode = 0.02f;

    public Greenprints Greenprints => _greenprints;

    public NodeAffiliation NodeAffiliation => _nodeAffiliation;

    public string NodeName => _nodeName;
    public string TitleName => _titleName;

    public float DelayNextNode => _delayNextNode;

    public abstract void Active(Node node);

    public abstract List<Pin> GetPins(Node owner);

    public Node GetNode(Vector2 position, Greenprints greenprints)
    {
        return new Node(greenprints, this, position);
    }

    public void ActiveExecutionOutputPin(Node node, int pinIndex)
    {
        ((ExecutionOutputPin)node.Pins[pinIndex]).SendSignal();
    }

    public void SetDataOutputPinValue(Node node, int pinIndex, object signal)
    {
        ((DataOutputPin)node.Pins[pinIndex]).Value = signal;
    }

    public object GetDataInputPinValue(Node node, int pinIndex)
    {
        object value = null;

        DataInputPin input = (DataInputPin)node.Pins[pinIndex];
        DataOutputPin output = input.OutputConnection;

        if (output == null)
        {
            Type type = Type.GetType(input.GreenprintsValueType.ValueName);
            value = type.IsValueType ? Activator.CreateInstance(type) : null;
        }
        else
        {
            value = output.Value;
        }

        return value;
    }
}
