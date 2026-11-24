using System.Collections.Generic;
using UnityEngine.UIElements;

public abstract class Pin
{
    private string _pinName;

    private Greenprints _greenprints;

    private Node _owner;

    private VisualElement _pinContainer;

    public string PinName => _pinName;

    public Greenprints Greenprints => _greenprints;

    public Node Owner => _owner;

    public VisualElement PinContainer
    {
        get => _pinContainer;
        set => _pinContainer = value;
    }

    public Pin(string pinName, Node owner, Greenprints greenprints)
    {
        _pinName = pinName;

        _greenprints = greenprints;

        _owner = owner;
    }

    public virtual bool IsCanConnect(Pin connectPin)
    {
        if (connectPin != null &&
            connectPin != this &&
            _owner != connectPin.Owner)
        {
            return true;
        }

        return false;
    }

    public abstract bool IsConnected();
}
