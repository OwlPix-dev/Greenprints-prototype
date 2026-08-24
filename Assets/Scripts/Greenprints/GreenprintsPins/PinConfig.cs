using UnityEngine;

[System.Serializable]
public class PinConfig
{
    [SerializeField] private string _pinName;
    [SerializeField] private GreenprintsValueType _valueType;

    public string PinName => _pinName;
    public GreenprintsValueType ValueType => _valueType;
}
