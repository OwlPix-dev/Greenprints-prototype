using UnityEngine;

public abstract class DataPin : Pin
{
    private GreenprintsValueType _greenprintsValueType;

    public GreenprintsValueType GreenprintsValueType => _greenprintsValueType;

    public DataPin(string pinName, Node owner, Greenprints greenprints, GreenprintsValueType greenprintsValueType)
        : base(pinName, owner, greenprints)
    {
        _greenprintsValueType = greenprintsValueType;
    }
}
