using UnityEngine;

[System.Serializable]
public class PinUIData
{
    [SerializeField] private Sprite _connectInputIcon;
    [SerializeField] private Sprite _connectOutputIcon;

    [SerializeField] private Sprite _unconnectInputIcon;
    [SerializeField] private Sprite _unconnectOutputIcon;

    [SerializeField] private Color _pinWireColor;

    public Sprite ConnectInputIcon => _connectInputIcon;
    public Sprite ConnectOutputIcon => _connectOutputIcon;

    public Sprite UnconnectInputIcon => _unconnectInputIcon;
    public Sprite UnconnectOutputIcon => _unconnectOutputIcon;

    public Color PinWireColor => _pinWireColor;
}
