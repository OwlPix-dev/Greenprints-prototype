using UnityEngine;

[CreateAssetMenu(fileName = "GreenprintsUIData", menuName = "Greenprints/UIData")]
public class GreenprintsClassNamesData : ScriptableObject
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
    [SerializeField] private string _nodeInputPinContainerClassName;
    [SerializeField] private string _nodeOutputPinContainerClassName;
    [SerializeField] private string _nodePinClassName;
    [SerializeField] private string _nodePinNameClassName;

    [Space]

    [SerializeField] private string _pinConnectionPointClassName;

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
    public string NodeInputPinContainerClassName => _nodeInputPinContainerClassName;
    public string NodeOutputPinContainerClassName => _nodeOutputPinContainerClassName;
    public string NodePinClassName => _nodePinClassName;
    public string NodePinNameClassName => _nodePinNameClassName;

    public string PinConnectionPointClassName => _pinConnectionPointClassName;
}
