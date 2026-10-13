using System.Collections.Generic;
using System.Linq;

public class ExecutionInputPin : ExecutionPin
{
    private List<ExecutionOutputPin> _outputConnections = new List<ExecutionOutputPin>();

    public List<ExecutionOutputPin> OutputConnections => _outputConnections;

    public ExecutionInputPin(string pinName, Node owner, Greenprints greenprints) : base(pinName, owner, greenprints) { }

    public override bool IsCanConnect(Pin connectPin)
    {
        if (base.IsCanConnect(connectPin) &&
            connectPin is ExecutionOutputPin)
        {
            return true;
        }

        return false;
    }

    public override bool IsConnected()
    {
        return _outputConnections.Count > 0;
    }
}
