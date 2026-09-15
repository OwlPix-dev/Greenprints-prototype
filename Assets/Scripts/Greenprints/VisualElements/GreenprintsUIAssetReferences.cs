using System;
using UnityEngine;
using UnityEngine.UIElements;

public class GreenprintsUIAssetReferences : MonoBehaviour
{
    [SerializeField] private GreenprintsClassNamesData _greenprintsUIData;

    private static GreenprintsUIAssetReferences _instance;

    public GreenprintsClassNamesData GreenprintsUIData => _greenprintsUIData;

    public static GreenprintsUIAssetReferences Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<GreenprintsUIAssetReferences>(nameof(GreenprintsUIAssetReferences));
            }

            return _instance;
        }
    }
}
