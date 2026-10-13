using System.Collections.Generic;
using System.Linq;

public class DataOutputPin : DataPin
{
    private object _value;

    private List<PinConnection> _inputConditionalConnections = new List<PinConnection>();

    public object Value
    {
        get
        {
            if (_value == null)
            {
                if (Owner.Pins.FirstOrDefault(pin => pin is ExecutionPin) == null)
                {
                    Owner.NodeConfig.Active(Owner);
                }
            }

            return _value;
        }
        set => _value = value;
    }

    public List<PinConnection> InputConditionalConnections => _inputConditionalConnections;
    public List<PinConnection> InputConnections => _inputConditionalConnections.Where(
        conditionalConnection => conditionalConnection.InputPin != null).ToList();

    public DataOutputPin(string pinName, Node owner, Greenprints greenprints, GreenprintsValueType greenprintsValueType)
        : base(pinName, owner, greenprints, greenprintsValueType) { }

    public override bool IsCanConnect(Pin connectPin)
    {
        if (base.IsCanConnect(connectPin) &&
            connectPin is DataInputPin)
        {
            return true;
        }

        return false;
    }

    public override bool IsConnected()
    {
        return _inputConditionalConnections.Count > 0;
    }
}
