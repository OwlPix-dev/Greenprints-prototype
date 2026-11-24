using UnityEngine;

[CreateAssetMenu(fileName = "Value", menuName = "Greenprints/Value")]
public class GreenprintsValueType : ScriptableObject
{
    [SerializeField] private string _valueTitleName;
    [SerializeField] private string _valueName;

    [SerializeField] private Color _valueColor;

    [SerializeField] private GreenprintsValueType[] _derivedValueTypes;

    public string ValueTitleName => _valueTitleName;
    public string ValueName => _valueName;

    public Color ValueColor => _valueColor;

    public GreenprintsValueType[] DerivedValueTypes => _derivedValueTypes;
}
