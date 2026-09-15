using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Node
{
    private NodeConfig _nodeConfig;
    private List<Pin> _pins = new List<Pin>();
    private Vector2 _position;

    public NodeConfig NodeConfig => _nodeConfig;
    public List<Pin> Pins => _pins;
    public Vector2 Position
    {
        get => _position;
        set => _position = value;
    }

    public Node(NodeConfig nodeConfig, PinConfig[] inputPins, PinConfig[] outputPins, Vector2 position, Greenprints greenprints)
    {
        _nodeConfig = nodeConfig;

        void CreatePins(PinConfig[] pinsConfig, PinDirection pinDirection)
        {
            foreach (PinConfig pinConfig in pinsConfig)
            {
                string name = pinConfig.PinName;
                GreenprintsValueType valueType = pinConfig.ValueType;

                _pins.Add(valueType != null ?
                    new DataPin(name, pinDirection, this, greenprints, valueType) :
                    new ExecutionPin(name, pinDirection, this, greenprints));
            }
        }

        CreatePins(inputPins, PinDirection.Input);
        CreatePins(outputPins, PinDirection.Output);

        _position = position;
    }
}
