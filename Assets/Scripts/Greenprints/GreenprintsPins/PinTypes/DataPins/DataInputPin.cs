using System.Linq;
using UnityEngine;

public class DataInputPin : DataPin
{
    private DataOutputPin _outputConnection;
    
    public DataOutputPin OutputConnection
    { 
        get => _outputConnection;
        set => _outputConnection = value;
    }

    public DataInputPin(string pinName, Node owner, Greenprints greenprints, GreenprintsValueType greenprintsValueType)
        : base(pinName, owner, greenprints, greenprintsValueType) { }

    public override bool IsCanConnect(Pin connectPin)
    {
        if (base.IsCanConnect(connectPin) &&
            connectPin is DataOutputPin dataOutputPin &&
            (dataOutputPin.GreenprintsValueType == GreenprintsValueType ||
            GreenprintsValueType.DerivedValueTypes.FirstOrDefault(valueType => valueType == dataOutputPin.GreenprintsValueType) != null) &&
            _outputConnection == null)
        {
            return true;
        }

        return false;
    }

    public override bool IsConnected()
    {
        return _outputConnection != null;
    }
}
