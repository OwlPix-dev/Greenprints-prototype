using UnityEngine;

[System.Serializable]
public class PinIconsData
{
    [SerializeField] private GreenprintsValueType _valueType;

    [SerializeField] private Sprite _connectInputIcon;
    [SerializeField] private Sprite _connectOutputIcon;

    [SerializeField] private Sprite _unconnectInputIcon;
    [SerializeField] private Sprite _unconnectOutputIcon;

    public GreenprintsValueType ValueType => _valueType;

    public Sprite ConnectInputIcon => _connectInputIcon;
    public Sprite ConnectOutputIcon => _connectOutputIcon;

    public Sprite UnconnectInputIcon => _unconnectInputIcon;
    public Sprite UnconnectOutputIcon => _unconnectOutputIcon;
}
