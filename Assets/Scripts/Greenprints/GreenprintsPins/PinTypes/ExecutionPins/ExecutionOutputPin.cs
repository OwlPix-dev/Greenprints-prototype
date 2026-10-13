using System.Collections.Generic;
using UnityEngine;

public class ExecutionOutputPin : ExecutionPin
{
    private PinConnection _inputConditionalConnection;

    public PinConnection InputConditionalConnection
    {
        get => _inputConditionalConnection;
        set => _inputConditionalConnection = value;
    }
    public PinConnection InputConnection => _inputConditionalConnection != null ?
        _inputConditionalConnection.InputPin != null ? _inputConditionalConnection : null : null;

    public ExecutionOutputPin(string pinName, Node owner, Greenprints greenprints) : base(pinName, owner, greenprints) { }

    public void SendSignal()
    {
        if (InputConnection != null && InputConnection.InputPin != null)
        {
            Greenprints.NodesInWork.Add(InputConnection.InputPin.Owner);
        }
    }

    public override bool IsCanConnect(Pin connectPin)
    {
        if (base.IsCanConnect(connectPin) && 
            connectPin is ExecutionInputPin &&
            InputConnection == null)
        {
            return true;
        }

        return false;
    }

    public override bool IsConnected()
    {
        return _inputConditionalConnection != null;
    }
}
