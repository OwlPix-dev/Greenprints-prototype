using NewPowerSupply.Geenprints.Pins;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GreenprintsUIController : MonoBehaviour
{
    [SerializeField] private UIDocument _uIDocument;

    [SerializeField] private ButtonsContainerData _titleBarBtns;

    [SerializeField] private string _openAvailableNodesBtnText = "►Add node";
    [SerializeField] private string _closeAvailableNodesBtnText = "►Add node";
    [SerializeField] private string _deleteNodeBtnText = "►Add node";

    [SerializeField] private Vector2Int _nodeGridSize = new Vector2Int(20, 20);
    [SerializeField] private Vector2Int _pinWirePointGridSize = new Vector2Int(5, 5);

    [SerializeField] private Vector2 _availableNodeOffset = new Vector2(40f, 40f);
    [SerializeField] private Vector2 _pinWirePointOffset = Vector2.zero;

    [SerializeField] private int _dragMouseIndex;
    [SerializeField] private int _removePinWireMouseIndex = 1;

    private VisualElement _root;

    private ScrollView _canvasScroller;
    private VisualElement _canvas;

    private VisualElement _nodesLayer;
    private VisualElement _pinWiresLayer;

    private Button _nodeManagerBtn;
    private VisualElement _availableNodesContainer;

    private Greenprints _greenprints;

    private GreenprintsUIData _uIData;
    private GreenprintsValueTypesData _valueTypesData;

    private GreenprintsDragElement _currentDragElement;

    private bool _isDragging => _currentDragElement != null;

    public Vector2Int NodeGridSize => _nodeGridSize;
    public Vector2Int PinWirePointGridSize => _pinWirePointGridSize;

    public Vector2 PinWirePointOffset => _pinWirePointOffset;

    public int DragMouseIndex => _dragMouseIndex;
    public int RemovePinWireMouseIndex => _removePinWireMouseIndex;

    public VisualElement Root => _root;

    public ScrollView CanvasScroller => _canvasScroller;
    public VisualElement Canvas => _canvas;

    public VisualElement NodesLayer => _nodesLayer;
    public VisualElement PinWiresLayer => _pinWiresLayer;

    public Button NodeManagerBtn => _nodeManagerBtn;

    public Greenprints Greenprints => _greenprints;

    public GreenprintsUIData UIData => _uIData;
    public GreenprintsValueTypesData ValueTypesData => _valueTypesData;

    public GreenprintsDragElement CurrentDragElement
    {
        get => _currentDragElement;
        set => _currentDragElement = value;
    }

    public bool IsActive => _greenprints != null;

    public Action OnCanvasHover;
    public Action OnCanvasLeave;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log($"Debug nodes {_greenprints.Nodes.Count}");

            foreach (Node node in _greenprints.Nodes)
            {
                Debug.Log($"- node {node}, {node.NodeConfig.NodeName}");

                foreach (Pin pin in node.Pins)
                {
                    if (!PinUtility.IsPinInput(pin))
                    {
                        Debug.Log("-- output Pin (connections: )");

                        foreach (PinConnection pinConnection in PinUtility.GetPinConditionalConnectionsFromOutputPin(pin))
                        {
                            Debug.Log($"--- {pinConnection.InputPin}");
                        }
                    }
                }
            }
        }

        if (!_isDragging) { return; }

        _currentDragElement.MoveDragElement();
    }

    private void LateUpdate()
    {
        if (!_isDragging) { return; }

        if (Input.GetMouseButtonUp(_dragMouseIndex))
        {
            _currentDragElement.DropDragElement();

            _currentDragElement = null;

            _nodeManagerBtn.text = _openAvailableNodesBtnText;
        }
    }

    public void OpenGreenprints(Greenprints greenprints)
    {
        SetActive(true, greenprints);

        _uIData = GreenprintsUIAssetReferences.Instance.GreenprintsUIData;
        _valueTypesData = GreenprintsValueTypesAssetReferences.Instance.GreenprintsValueTypesData;

        _root = _uIDocument.rootVisualElement;

        void GeometryChanged(GeometryChangedEvent evt)
        {
            _titleBarBtns.DrawButtons(_root);

            _canvasScroller = _root.Q<ScrollView>(className: _uIData.CanvasScrollerClassName);
            _canvas = _root.Q<VisualElement>(className: _uIData.CanvasClassName);

            _nodesLayer = _canvas.Q<VisualElement>(_uIData.NodesLayerName);
            _pinWiresLayer = _canvas.Q<VisualElement>(_uIData.PinsConnectionsLayerName);

            _availableNodesContainer = _root.Q<VisualElement>(className: _uIData.AvailableNodesContainerClassName);

            foreach (NodeConfig availableNode in _greenprints.AvailableNodes)
            {
                Label availableNodeName = new Label();
                availableNodeName.AddToClassList(_uIData.AvailableNodeClassName);

                availableNodeName.text = availableNode.TitleName;
                availableNodeName.style.backgroundColor = new StyleColor(availableNode.NodeAffiliation.AffiliationColor);

                availableNodeName.RegisterCallback<PointerDownEvent>(evt =>
                {
                    if (evt.button != _dragMouseIndex) { return; }

                    _currentDragElement = new DragNode(
                          this
                        , null
                        , availableNode
                        , null
                        , _availableNodeOffset
                        , Vector2.zero);

                    ConfigNodeManagerBtnOnDeleteNode();
                });

                _availableNodesContainer.Add(availableNodeName);
            }

            _canvas.style.width = _greenprints.CanvasSize.x;
            _canvas.style.height = _greenprints.CanvasSize.y;

            _canvas.RegisterCallback<PointerEnterEvent>(evt => OnCanvasHover?.Invoke());
            _canvas.RegisterCallback<PointerLeaveEvent>(evt => OnCanvasLeave?.Invoke());

            _availableNodesContainer.style.visibility = Visibility.Hidden;

            VisualElement workZone = _root.Q<VisualElement>(className: _uIData.WorkZoneClassName);

            _nodeManagerBtn = new Button();
            _nodeManagerBtn.AddToClassList(_uIData.NodeManagerBtnClassName);

            _nodeManagerBtn.text = _openAvailableNodesBtnText;
            _nodeManagerBtn.clicked += () =>
            {
                Visibility availableNodesContainerVisibility = Visibility.Hidden;
                string nodeManagerBtnText = _openAvailableNodesBtnText;

                if (_availableNodesContainer.style.visibility == Visibility.Hidden)
                {
                    availableNodesContainerVisibility = Visibility.Visible;
                    nodeManagerBtnText = _closeAvailableNodesBtnText;
                }

                ConfigNodeManagerBtn(nodeManagerBtnText, availableNodesContainerVisibility);
            };

            workZone.Add(_nodeManagerBtn);

            RenderGreenprints();

            _root.UnregisterCallback<GeometryChangedEvent>(GeometryChanged);
        }

        _root.RegisterCallback<GeometryChangedEvent>(GeometryChanged);
    }

    private void ConfigNodeManagerBtn(string btnText, Visibility btnVisibility)
    {
        _nodeManagerBtn.text = btnText;
        _availableNodesContainer.style.visibility = btnVisibility;
    }

    public void ConfigNodeManagerBtnOnDeleteNode()
    {
        ConfigNodeManagerBtn(_deleteNodeBtnText, Visibility.Hidden);
    }

    public void CloseGreenprints()
    {
        foreach (Node node in _greenprints.Nodes)
        {
            foreach (Pin pin in node.Pins)
            {
                if (!PinUtility.IsPinInput(pin))
                {
                    List<PinConnection> pinConnections = PinUtility.GetPinConditionalConnectionsFromOutputPin(pin);

                    for (int pinConnectionIndex = 0; pinConnectionIndex < pinConnections.Count; pinConnectionIndex++)
                    {
                        if (pinConnections[pinConnectionIndex].InputPin == null)
                        {
                            PinUtility.RemovePinWire(pin, pinConnectionIndex, _uIData, _valueTypesData);
                        }
                        
                    }
                }
            }
        }

        SetActive(false, null);
    }

    private void SetActive(bool isActive, Greenprints greenprints)
    {
        _greenprints = greenprints;
        _uIDocument.enabled = isActive;
    }

    private void RenderGreenprints()
    {
        foreach (Node node in _greenprints.Nodes)
        {
            NodeVisualElement nodeTitle = new NodeVisualElement(node, this);

            node.NodeTitle = nodeTitle;

            _nodesLayer.Add(nodeTitle);
        }

        VisualElement GetPinIcon(Node node, Pin pin)
        {
            return node.NodeTitle.Query<VisualElement>(
                className: _uIData.NodePinContainerClassName).ToList().FirstOrDefault(
                pinContainer => pinContainer == pin.PinContainer).Q<VisualElement>(className: _uIData.NodePinClassName);
        }

        foreach (Node node in _greenprints.Nodes)
        {
            foreach (Pin pin in node.Pins)
            {
                if (!PinUtility.IsPinInput(pin))
                {
                    foreach (PinConnection pinConnection in PinUtility.GetPinConnectionsFromOutputPin(pin))
                    {
                        VisualElement outputPinIcon = GetPinIcon(node, pin);

                        Node inputPinUINode = _greenprints.Nodes.FirstOrDefault(node => node == pinConnection.InputPin.Owner);

                        VisualElement inputPinIcon = GetPinIcon(inputPinUINode, pinConnection.InputPin);

                        pinConnection.RerenderPinWire(outputPinIcon, inputPinIcon);
                    }
                }
            }
        }
    }
}