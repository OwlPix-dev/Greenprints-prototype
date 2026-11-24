using UnityEngine;

[CreateAssetMenu(fileName = "GreenprintsValueTypesData", menuName = "Greenprints/ValueTypesData")]
public class GreenprintsValueTypesData : ScriptableObject
{
    [SerializeField] private GreenprintsValueType _intValue;
    [SerializeField] private GreenprintsValueType _floatValue;
    [SerializeField] private GreenprintsValueType _boolValue;
    [SerializeField] private GreenprintsValueType _stringValue;

    public GreenprintsValueType IntValue => _intValue;
    public GreenprintsValueType FloatValue => _floatValue;
    public GreenprintsValueType BoolValue => _boolValue;
    public GreenprintsValueType StringValue => _stringValue;
}
