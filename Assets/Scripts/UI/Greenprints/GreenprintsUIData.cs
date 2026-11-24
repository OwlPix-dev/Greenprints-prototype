using UnityEngine;

[CreateAssetMenu(fileName = "GreenprintsUIData", menuName = "Greenprints/UIData")]
public class GreenprintsUIData : ScriptableObject
{
    [SerializeField] private string _workZoneClassName;

    [Space]

    [SerializeField] private string _canvasScrollerClassName;
    [SerializeField] private string _canvasClassName;

    [Space]

    [SerializeField] private string _nodesLayerName;
    [SerializeField] private string _pinsConnectionsLayerName;

    [Space]

    [SerializeField] private string _nodeManagerBtnClassName;
    [SerializeField] private string _availableNodesContainerClassName;
    [SerializeField] private string _availableNodeClassName;

    [Space]

    [SerializeField] private string _dragNodeNameClassName;

    [Space]

    [SerializeField] private string _nodeContainerClassName;
    [SerializeField] private string _nodeTitleClassName;
    [SerializeField] private string _nodePinsZoneClassName;
    [SerializeField] private string _nodePinsContainerClassName;
    [SerializeField] private string _nodePinsSpaceClassName;
    [SerializeField] private string _nodePinContainerClassName;
    [SerializeField] private string _nodePinClassName;
    [SerializeField] private string _nodePinNameClassName;

    [Space]

    [SerializeField] private PinUIData _intPinUIData;
    [SerializeField] private PinUIData _floatPinUIData;
    [SerializeField] private PinUIData _boolPinUIData;
    [SerializeField] private PinUIData _stringPinUIData;
    [SerializeField] private PinUIData _executionPinUIData;

    [Space]

    [SerializeField] private string _pinWirePointClassName;

    [SerializeField] private float _pinWireWidth = 4f;

    public string WorkZoneClassName => _workZoneClassName;

    public string CanvasScrollerClassName => _canvasScrollerClassName;
    public string CanvasClassName => _canvasClassName;

    public string NodesLayerName => _nodesLayerName;
    public string PinsConnectionsLayerName => _pinsConnectionsLayerName;

    public string NodeManagerBtnClassName => _nodeManagerBtnClassName;
    public string AvailableNodesContainerClassName => _availableNodesContainerClassName;
    public string AvailableNodeClassName => _availableNodeClassName;

    public string DragNodeNameClassName => _dragNodeNameClassName;

    public string NodeContainerClassName => _nodeContainerClassName;
    public string NodeTitleClassName => _nodeTitleClassName;
    public string NodePinsZoneClassName => _nodePinsZoneClassName;
    public string NodePinsContainerClassName => _nodePinsContainerClassName;
    public string NodePinsSpaceClassName => _nodePinsSpaceClassName;
    public string NodePinContainerClassName => _nodePinContainerClassName;
    public string NodePinClassName => _nodePinClassName;
    public string NodePinNameClassName => _nodePinNameClassName;

    public string PinWirePointClassName => _pinWirePointClassName;

    public PinUIData IntPinUIData => _intPinUIData;
    public PinUIData FloatPinUIData => _floatPinUIData;
    public PinUIData BoolPinUIData => _boolPinUIData;
    public PinUIData StringPinUIData => _stringPinUIData;
    public PinUIData ExecutionPinUIData => _executionPinUIData;

    public float PinWireWidth => _pinWireWidth;
}
