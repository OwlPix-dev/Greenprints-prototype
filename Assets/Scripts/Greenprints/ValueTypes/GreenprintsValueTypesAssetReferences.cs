using UnityEngine;

public class GreenprintsValueTypesAssetReferences : MonoBehaviour
{
    [SerializeField] private GreenprintsValueTypesData _greenprintsValueTypesData;

    private static GreenprintsValueTypesAssetReferences _instance;

    public GreenprintsValueTypesData GreenprintsValueTypesData => _greenprintsValueTypesData;

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
