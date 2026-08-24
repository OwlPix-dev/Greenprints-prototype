public class DataPin : Pin
{
    private GreenprintsValueType _greenprintsValueType;
    private object _value;

    public GreenprintsValueType GreenprintsValueType => _greenprintsValueType;
    public object Value
    {
        get
        {
            return _value;
        }
        set => _value = value;
    }

    public DataPin(string pinName, PinDirection pinDirection, Node owner, GreenprintsValueType greenprintsValueType) :
        base(pinName, pinDirection, owner)
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
}
