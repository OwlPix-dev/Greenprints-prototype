using UnityEngine;

public class GreenprintsValueTypesAssetReferences : MonoBehaviour
{
    [SerializeField] private GreenprintsValueType _intValue;
    [SerializeField] private GreenprintsValueType _floatValue;
    [SerializeField] private GreenprintsValueType _boolValue;
    [SerializeField] private GreenprintsValueType _stringValue;

    private static GreenprintsValueTypesAssetReferences _instance;

    public GreenprintsValueType IntValue => _intValue;
    public GreenprintsValueType FloatValue => _floatValue;
    public GreenprintsValueType BoolValue => _boolValue;
    public GreenprintsValueType StringValue => _stringValue;

    public static GreenprintsValueTypesAssetReferences Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<GreenprintsValueTypesAssetReferences>(
                    nameof(GreenprintsValueTypesAssetReferences));
            }

            return _instance;
        }
    }
}
