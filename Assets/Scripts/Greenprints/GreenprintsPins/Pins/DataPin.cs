public class DataPin : Pin
{
    private GreenprintsValueType _greenprintsValueType;
    private object _value;

    public GreenprintsValueType GreenprintsValueType => _greenprintsValueType;
    public object Value
    {
        get
        {
            return _value;//
        }
        set => _value = value;
    }

    public DataPin(
          string pinName
        , PinDirection pinDirection
        , Node owner
        , Greenprints greenprints
        , GreenprintsValueType greenprintsValueType) :
        base(pinName, pinDirection, owner, greenprints)
    {
        _greenprintsValueType = greenprintsValueType;
    }

    public override bool IsCanConnect(Pin connectPin)
    {
        if (!base.IsCanConnect(connectPin) ||
            !(connectPin is DataPin) ||
            (PinDirection == PinDirection.Input && Connections.Count > 0))
        {
            return false;
        }

        return true;
    }

    public override void SendSignal(object sendValue = null)
    {
        foreach (Pin pin in Connections)
        {
            pin.ReceiveSignal(sendValue);
        }
    }

    public override void ReceiveSignal(object receiveValue = null)
    {
        _value = receiveValue;
    }
}
