using System.Collections.Generic;

public abstract class Pin
{
    private string _pinName;
    private PinDirection _pinDirection;
    private List<Pin> _connections = new List<Pin>();
    private Node _owner;
    private Greenprints _greenprints;

    public string PinName => _pinName;
    public PinDirection PinDirection => _pinDirection;
    public List<Pin> Connections => _connections;
    public Node Owner => _owner;
    public Greenprints Greenprints => _greenprints;

    public Pin(string pinName, PinDirection pinDirection, Node owner, Greenprints greenprints)
    {
        _pinName = pinName;
        _pinDirection = pinDirection;
        _owner = owner;
        _greenprints = greenprints;
    }

    public virtual bool IsCanConnect(Pin connectPin)
    {
        if (connectPin == null ||
            connectPin == this ||
            _pinDirection == connectPin.PinDirection ||
            _owner == connectPin.Owner ||
            _connections.Contains(connectPin))
        {
            return false;
        }

        return true;
    }

    public abstract void SendSignal(object sendValue = null);

    public abstract void ReceiveSignal(object receiveValue = null);
}
