using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Greenprints : MonoBehaviour
{
    [SerializeField] private NodeConfig[] _availableNodes;

    [SerializeField] private PinUIData[] _pinsIconsData;

    [SerializeField] private Vector2 _canvasSize = new Vector2(2000, 1600);

    private List<UINode> _uINodes = new List<UINode>();

    private List<Node> _nodesInWork = new List<Node>();

    private float _timer;

    public NodeConfig[] AvailableNodes => _availableNodes;

    public PinUIData[] PinsIconsData => _pinsIconsData;

    public Vector2 CanvasSize => _canvasSize;

    public List<UINode> UINodes => _uINodes;

    public List<Node> NodesInWork => _nodesInWork;

    public List<Node> Nodes => _uINodes.Select(uINode => uINode.Node).ToList();

    private void Update()
    {
        if (_nodesInWork.Count == 0) { return; }

        if (_timer <= 0)
        {
            Node workNode = _nodesInWork[0];
            NodeConfig nodeConfig = workNode.NodeConfig;

            nodeConfig.Active(workNode);
            _timer = nodeConfig.DelayNextNode;

            _nodesInWork.Remove(workNode);

            if (_nodesInWork.Count <= 0)
            {
                ResetOutputDataPins();
            }
        }
        else
        {
            _timer -= Time.deltaTime;
        }
    }

    public bool IsPinInput(Pin pin)
    {
        return pin is DataInputPin || pin is ExecutionInputPin;
    }

    public List<PinConnection> GetPinConnectionsFromOutputPin(Pin pin)
    {
        if (pin is DataOutputPin dataOutputPin)
        {
            return dataOutputPin.InputConnections;
        }
        else if (pin is ExecutionOutputPin executionOutputPin)
        {
            List<PinConnection> returnList = new List<PinConnection>();

            PinConnection inputConnection = executionOutputPin.InputConnection;

            if (inputConnection != null)
            {
                returnList.Add(inputConnection);
            }

            return returnList;
        }

        return null;
    }

    public List<PinConnection> GetPinConditionalConnectionsFromOutputPin(Pin pin)
    {
        if (pin is DataOutputPin dataOutputPin)
        {
            return dataOutputPin.InputConditionalConnections;
        }
        else if (pin is ExecutionOutputPin executionOutputPin)
        {
            List<PinConnection> returnList = new List<PinConnection>();

            PinConnection inputConditionalConnection = executionOutputPin.InputConditionalConnection;

            if (inputConditionalConnection != null)
            {
                returnList.Add(inputConditionalConnection);
            }

            return returnList;
        }

        return null;
    }

    public List<Pin> GetPinConnectionsFromInputPin(Pin pin)
    {
        if (pin is DataInputPin dataInputPin)
        {
            List<Pin> returnList = new List<Pin>();

            Pin outputConnection = dataInputPin.OutputConnection;

            if (outputConnection != null)
            {
                returnList.Add(outputConnection);
            }

            return returnList;
        }
        else if (pin is ExecutionInputPin executionInputPin)
        {
            return executionInputPin.OutputConnections.Select(connection => (Pin)connection).ToList();
        }

        return null;
    }

    public void AddPinConnectionToOutputPin(Pin pin, PinConnection pinConnection, GreenprintsUIController greenprintsUIController)
    {
        bool isChangeIcon = !pin.IsConnected();

        if (pin is DataOutputPin dataOutputPin)
        {
            dataOutputPin.InputConditionalConnections.Add(pinConnection);
        }
        else if (pin is ExecutionOutputPin executionOutputPin)
        {
            executionOutputPin.InputConditionalConnection = pinConnection;
        }

        if (isChangeIcon)
        {
            greenprintsUIController.SetPinIcon(pin);
        }
    }

    public void AddPinConnectionToInputPin(Pin pin, Pin pinConnection, GreenprintsUIController greenprintsUIController)
    {
        bool isChangeIcon = !pin.IsConnected();

        if (pin is DataInputPin dataInputPin && dataInputPin.OutputConnection == null)
        {
            dataInputPin.OutputConnection = (DataOutputPin)pinConnection;
        }
        else if (pin is ExecutionInputPin executionInputPin)
        {
            executionInputPin.OutputConnections.Add((ExecutionOutputPin)pinConnection);
        }

        if (isChangeIcon)
        {
            greenprintsUIController.SetPinIcon(pin);
        }
    }

    private void RemovePinConnectionFromOutputPin(Pin pin, int index, GreenprintsUIController greenprintsUIController)
    {
        bool isChangeIcon = GetPinConditionalConnectionsFromOutputPin(pin).Count == 1;

        if (pin is DataOutputPin dataOutputPin)
        {
            dataOutputPin.InputConditionalConnections.RemoveAt(index);
        }
        else if (pin is ExecutionOutputPin executionOutputPin)
        {
            executionOutputPin.InputConditionalConnection = null;
        }

        if (isChangeIcon)
        {
            greenprintsUIController.SetPinIcon(pin);
        }
    }

    private void RemovePinConnectionFromInputPin(Pin pin, int index, GreenprintsUIController greenprintsUIController)
    {
        bool isChangeIcon = GetPinConnectionsFromInputPin(pin).Count == 1;
        
        if (pin is DataInputPin dataInputPin)
        {
            dataInputPin.OutputConnection = null;
        }
        else if (pin is ExecutionInputPin executionInputPin)
        {
            executionInputPin.OutputConnections.RemoveAt(index);
        }

        if (isChangeIcon)
        {
            greenprintsUIController.SetPinIcon(pin);
        }
    }

    public void RemovePinWire(Pin outputPin, int pinWireIndex, GreenprintsUIController greenprintsUIController)
    {
        PinConnection pinConnection = GetPinConditionalConnectionsFromOutputPin(outputPin)[pinWireIndex];

        pinConnection.PinWireElement?.RemoveFromHierarchy();

        Pin inputPin = pinConnection.InputPin;
        if (inputPin != null)
        {
            List<Pin> inputPinConnections = GetPinConnectionsFromInputPin(inputPin);
            int outputPinIndexInInputPinConnections = Array.IndexOf(inputPinConnections.ToArray(), outputPin);

            RemovePinConnectionFromInputPin(inputPin, outputPinIndexInInputPinConnections, greenprintsUIController);
        }

        RemovePinConnectionFromOutputPin(outputPin, pinWireIndex, greenprintsUIController);
    }

    public void RemoveNodePinsConnections(Node node, GreenprintsUIController greenprintsUIController)
    {
        foreach (Pin inputPin in node.Pins.Where(pin => IsPinInput(pin)).ToList())
        {
            List<Pin> inputPinConnections = GetPinConnectionsFromInputPin(inputPin);

            for (int pinConnectionIndex = 0; pinConnectionIndex < inputPinConnections.Count; pinConnectionIndex++)
            {
                Pin outputPin = inputPinConnections[pinConnectionIndex];
                List<PinConnection> outputPinConnections = GetPinConnectionsFromOutputPin(outputPin);
                PinConnection outputPinConnection = outputPinConnections.FirstOrDefault(pinConnection => pinConnection.InputPin == inputPin);
                int inputPinIndexInOutputPinConnections = Array.IndexOf(outputPinConnections.ToArray(), outputPinConnection);

                RemovePinWire(outputPin, inputPinIndexInOutputPinConnections, greenprintsUIController);
            }
        }

        foreach (Pin outputPin in node.Pins.Where(pin => !IsPinInput(pin)).ToList())
        {
            for (int pinConnectionIndex = 0; pinConnectionIndex < GetPinConnectionsFromOutputPin(outputPin).Count; pinConnectionIndex++)
            {
                RemovePinWire(outputPin, pinConnectionIndex, greenprintsUIController);
            }
        }
    }

    public void ResetOutputDataPins()
    {
        foreach (Node node in Nodes)
        {
            foreach (Pin pin in node.Pins)
            {
                if (pin is DataOutputPin dataOutputPin)
                {
                    dataOutputPin.Value = null;
                }
            }
        }
    }
}
