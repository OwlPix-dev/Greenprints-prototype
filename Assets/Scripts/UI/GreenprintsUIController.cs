using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GreenprintsUIController : MonoBehaviour
{
    [SerializeField] private UIDocument _uIDocument;

    [SerializeField] private ButtonsContainerData _titleBarBtns;

    [SerializeField] private GreenprintsManagerBtnState _managerBtnOpenAvailableNodesState;
    [SerializeField] private GreenprintsManagerBtnState _managerBtnCloseAvailableNodesState;
    [SerializeField] private GreenprintsManagerBtnState _managerBtnDeleteNodeState;

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

    private GreenprintsDragElement _currentDragElement;

    private Greenprints _greenprints;

    private bool _isDragging => _currentDragElement != null;

    public Vector2Int NodeGridSize => _nodeGridSize;
    public Vector2Int PinWirePointGridSize => _pinWirePointGridSize;

    public Vector2 PinWirePointOffset => _pinWirePointOffset;

    public int DragMouseIndex => _dragMouseIndex;

    public VisualElement Root => _root;

    public VisualElement NodesLayer => _nodesLayer;
    public VisualElement PinWiresLayer => _pinWiresLayer;

    public Button NodeManagerBtn => _nodeManagerBtn;

    public Greenprints Greenprints => _greenprints;

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
            Debug.Log($"Debug nodes {_greenprints.UINodes.Count}");

            foreach (UINode uINode in _greenprints.UINodes)
            {
                Debug.Log($"- node {uINode.Node}, {uINode.Node.NodeConfig.NodeName}");

                foreach (Pin pin in uINode.Node.Pins)
                {
                    if (!_greenprints.IsPinInput(pin))
                    {
                        Debug.Log("-- output Pin (connections: )");

                        foreach (PinConnection pinConnection in _greenprints.GetPinConditionalConnectionsFromOutputPin(pin))
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

            SetManagerBtnState(_managerBtnOpenAvailableNodesState);
        }
    }

    public void OpenGreenprints(Greenprints greenprints)
    {
        SetActive(true, greenprints);

        _root = _uIDocument.rootVisualElement;

        void GeometryChanged(GeometryChangedEvent evt)
        {
            _titleBarBtns.DrawButtons(_root);

            GreenprintsUIData uIData = GetUIData();

            _canvasScroller = _root.Q<ScrollView>(className: uIData.CanvasScrollerClassName);
            _canvas = _root.Q<VisualElement>(className: uIData.CanvasClassName);

            _nodesLayer = _canvas.Q<VisualElement>(uIData.NodesLayerName);
            _pinWiresLayer = _canvas.Q<VisualElement>(uIData.PinsConnectionsLayerName);

            _availableNodesContainer = _root.Q<VisualElement>(className: uIData.AvailableNodesContainerClassName);

            foreach (NodeConfig availableNode in _greenprints.AvailableNodes)
            {
                Label availableNodeName = new Label();
                availableNodeName.AddToClassList(uIData.AvailableNodeClassName);

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

                    SetManagerBtnState(_managerBtnDeleteNodeState);
                });

                _availableNodesContainer.Add(availableNodeName);
            }

            _canvas.style.width = _greenprints.CanvasSize.x;
            _canvas.style.height = _greenprints.CanvasSize.y;

            _canvas.RegisterCallback<PointerEnterEvent>(evt => OnCanvasHover?.Invoke());
            _canvas.RegisterCallback<PointerLeaveEvent>(evt => OnCanvasLeave?.Invoke());

            _availableNodesContainer.style.visibility = Visibility.Hidden;

            VisualElement workZone = _root.Q<VisualElement>(className: uIData.WorkZoneClassName);

            _nodeManagerBtn = new Button();
            _nodeManagerBtn.AddToClassList(uIData.NodeManagerBtnClassName);

            SetManagerBtnState(_managerBtnOpenAvailableNodesState);
            _nodeManagerBtn.clicked += () =>
            {
                ChangeActiveAvailableNodesContainer(_availableNodesContainer.style.visibility == Visibility.Hidden);
            };

            workZone.Add(_nodeManagerBtn);

            RenderGreenprints();

            _root.UnregisterCallback<GeometryChangedEvent>(GeometryChanged);
        }

        _root.RegisterCallback<GeometryChangedEvent>(GeometryChanged);
    }

    public void CloseGreenprints()
    {
        foreach (UINode uINode in _greenprints.UINodes)
        {
            foreach (Pin pin in uINode.Node.Pins)
            {
                if (!_greenprints.IsPinInput(pin))
                {
                    List<PinConnection> pinConnections = _greenprints.GetPinConditionalConnectionsFromOutputPin(pin);

                    for (int pinConnectionIndex = 0; pinConnectionIndex < pinConnections.Count; pinConnectionIndex++)
                    {
                        if (pinConnections[pinConnectionIndex].InputPin == null)
                        {
                            _greenprints.RemovePinWire(pin, pinConnectionIndex, this);
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

    private GreenprintsUIData GetUIData()
    {
        return GreenprintsUIAssetReferences.Instance.GreenprintsUIData;
    }

    public void SetManagerBtnState(GreenprintsManagerBtnState btnState)
    {
        if (btnState == _managerBtnDeleteNodeState)
        {
            ChangeActiveAvailableNodesContainer(false);
        }

        _nodeManagerBtn.text = btnState.BtnText;
    }

    private void ChangeActiveAvailableNodesContainer(bool isActive)
    {
        if (isActive)
        {
            SetManagerBtnState(_managerBtnCloseAvailableNodesState);
            _availableNodesContainer.style.visibility = Visibility.Visible;
        }
        else
        {
            SetManagerBtnState(_managerBtnOpenAvailableNodesState);
            _availableNodesContainer.style.visibility = Visibility.Hidden;
        }
    }

    public Vector2 GetCanvasPosition(Vector2 position, Vector2 gridSize)
    {
        Rect contentViewportRect = _canvasScroller.contentViewport.worldBound;

        Vector2 range = new Vector2(
              _canvasScroller.contentContainer.worldBound.width - contentViewportRect.width
            , _canvasScroller.contentContainer.worldBound.height - contentViewportRect.height);

        Vector2 scrollFactor = new Vector2(
              range.x > 0 ? _canvasScroller.scrollOffset.x / range.x : 0f
            , range.y > 0 ? _canvasScroller.scrollOffset.y / range.y : 0f);

        position += new Vector2(
              (_greenprints.CanvasSize.x - contentViewportRect.width) * scrollFactor.x - contentViewportRect.x
            , (_greenprints.CanvasSize.y - contentViewportRect.height) * scrollFactor.y - contentViewportRect.y);

        if (gridSize != Vector2.zero)
        {
            position -= new Vector2(
                  position.x - MathF.Round(position.x / gridSize.x) * gridSize.x
                , position.y - MathF.Round(position.y / gridSize.y) * gridSize.y);
        }

        return position;
    }

    public Vector2 GetDragElementPosition(
          Vector2 offset
        , Vector2 gridSize
        , VisualElement dragElement
        , Vector2? position = null)
    {
        position ??= GetMousePosition();

        position -= offset;

        position = GetCanvasPosition(position.Value, gridSize);

        Vector2 maxPosition = new Vector2(
              _canvas.worldBound.width - dragElement.worldBound.width
            , _canvas.worldBound.height - dragElement.worldBound.height);

        position = new Vector2(
              Mathf.Clamp(position.Value.x, 0, maxPosition.x)
            , Mathf.Clamp(position.Value.y, 0, maxPosition.y));

        return position.Value;
    }

    public Vector2 GetMousePosition()
    {
        Vector2 mousePosition = Input.mousePosition;
        mousePosition.y = Screen.height - mousePosition.y;
        return mousePosition;
    }

    public VisualElement GetPickElement(Vector2? position = null)
    {
        position ??= GetMousePosition();

        return _root.panel.Pick(position.Value);
    }

    public UINode GetUINodeFromTitle(VisualElement title)
    {
        return _greenprints.UINodes.FirstOrDefault(uINode => title == uINode.NodeTitle);
    }

    public VisualElement GetPinIconFromPin(Pin pin, GreenprintsUIData uIData = null)
    {
        uIData ??= GetUIData();

        return pin.PinContainer.Q<VisualElement>(className: uIData.NodePinClassName);
    }

    public PinUIData GetPinUIData(GreenprintsValueType valueType)
    {
        GreenprintsValueTypesAssetReferences valueTypesData = GreenprintsValueTypesAssetReferences.Instance;
        GreenprintsUIData uIData = GetUIData();

        if (valueType == valueTypesData.IntValue)
        {
            return uIData.IntPinUIData;
        }
        else if (valueType == valueTypesData.FloatValue)
        {
            return uIData.FloatPinUIData;
        }
        else if (valueType == valueTypesData.BoolValue)
        {
            return uIData.BoolPinUIData;
        }
        else if (valueType == valueTypesData.StringValue)
        {
            return uIData.StringPinUIData;
        }
        else if (valueType == null)
        {
            return uIData.ExecutionPinUIData;
        }

        return null;
    }

    public void SetPinIcon(Pin pin, VisualElement pinIcon = null)
    {
        GreenprintsValueType valueType = pin is DataPin dataPin ? dataPin.GreenprintsValueType : null;

        PinUIData pinUIData = GetPinUIData(valueType);

        Sprite icon = (_greenprints.IsPinInput(pin), pin.IsConnected()) switch
        {
            (true, false) => pinUIData.UnconnectInputIcon,
            (true, true) => pinUIData.ConnectInputIcon,
            (false, false) => pinUIData.UnconnectOutputIcon,
            (false, true) => pinUIData.ConnectOutputIcon
        };

        pinIcon ??= GetPinIconFromPin(pin);

        pinIcon.style.backgroundImage = new StyleBackground(icon);
    }

    public VisualElement RenderNode(Node node)
    {
        GreenprintsUIData uIData = GetUIData();

        NodeConfig nodeConfig = node.NodeConfig;

        VisualElement nodeContainer = new VisualElement();
        nodeContainer.AddToClassList(uIData.NodeContainerClassName);

        Label nodeTitle = new Label();
        nodeTitle.AddToClassList(uIData.NodeTitleClassName);

        nodeTitle.text = nodeConfig.NodeName;
        nodeTitle.style.backgroundColor = new StyleColor(nodeConfig.NodeAffiliation.AffiliationColor);

        VisualElement nodePinsZone = new VisualElement();
        nodePinsZone.AddToClassList(uIData.NodePinsZoneClassName);

        VisualElement inputPinsContainer = new VisualElement();
        inputPinsContainer.AddToClassList(uIData.NodePinsContainerClassName);

        VisualElement pinsSpace = new VisualElement();
        pinsSpace.AddToClassList(uIData.NodePinsSpaceClassName);

        VisualElement outputPinsContainer = new VisualElement();
        outputPinsContainer.AddToClassList(uIData.NodePinsContainerClassName);

        foreach (Pin pin in node.Pins)
        {
            VisualElement pinContainer = new VisualElement();
            pinContainer.AddToClassList(uIData.NodePinContainerClassName);

            VisualElement pinIcon = new VisualElement();
            pinIcon.AddToClassList(uIData.NodePinClassName);

            SetPinIcon(pin, pinIcon);

            if (_greenprints.IsPinInput(pin))
            {
                pinIcon.RegisterCallback<PointerUpEvent>(evt =>
                {
                    if (_currentDragElement is DragPinWirePoint dragPinWirePoint && evt.button == _dragMouseIndex)
                    {
                        Pin wireOwner = dragPinWirePoint.PinConnection.WireOwner;

                        if (wireOwner.IsCanConnect(pin) && pin.IsCanConnect(wireOwner))
                        {
                            dragPinWirePoint.PinConnection.SetInputPin(pin, pinIcon);

                            _greenprints.AddPinConnectionToInputPin(pin, wireOwner, this);

                            evt.StopPropagation();
                        }
                        else
                        {
                            // _greenprints.GetPinConditionalConnectionsFromOutputPin(wireOwner).Remove(dragPinWirePoint.PinConnection);
                            PinConnection[] pinConnections = _greenprints.GetPinConditionalConnectionsFromOutputPin(wireOwner).ToArray();
                            int pinConnectionIndex = Array.IndexOf(pinConnections, dragPinWirePoint.PinConnection);

                            _greenprints.RemovePinWire(wireOwner, pinConnectionIndex, this);
                            dragPinWirePoint.PinConnection.PinWireElement.RemoveFromHierarchy();
                        }
                    }
                });
            }
            else
            {
                pinIcon.RegisterCallback<PointerDownEvent>(evt =>
                {
                    if (evt.button == _dragMouseIndex)
                    {
                        Color wireColor = pin is DataPin dataPin ? dataPin.GreenprintsValueType.ValueColor : Color.white;

                        Vector2 startPointPosition = GetCanvasPosition(pinIcon.worldBound.center, Vector2.zero);

                        PinConnection newPinConnection = new PinConnection(
                              pin
                            , null
                            , this
                            , pinIcon
                            , GetMousePosition());

                        _greenprints.AddPinConnectionToOutputPin(pin, newPinConnection, this);

                        evt.StopPropagation();
                    }
                    else if (evt.button == _removePinWireMouseIndex)
                    {
                        List<PinConnection> pinConnections = _greenprints.GetPinConditionalConnectionsFromOutputPin(pin);
                        int inputPinIndex = pinConnections.Count - 1;
                        Pin inputPin = pinConnections[inputPinIndex].InputPin;

                        _greenprints.RemovePinWire(pin, inputPinIndex, this);
                    }
                });
            }

            string pinNameText = pin.PinName;
            VisualElement pinName = null;

            if (pinNameText != "")
            {
                pinName = new Label();
                pinName.AddToClassList(uIData.NodePinNameClassName);

                ((Label)pinName).text = pinNameText;
            }
            else
            {
                pinName = new VisualElement();
                pinName.style.flexGrow = 1;
            }

            void ConfigurePin(VisualElement container, params VisualElement[] pinContents)
            {
                pinContainer.Add(pinContents[0]);
                pinContainer.Add(pinContents[1]);

                container.Add(pinContainer);
            }

            if (_greenprints.IsPinInput(pin))
            {
                ConfigurePin(inputPinsContainer, pinIcon, pinName);
            }
            else
            {
                ConfigurePin(outputPinsContainer, pinName, pinIcon);
            }

            pin.PinContainer = pinContainer;
        }

        nodePinsZone.Add(inputPinsContainer);
        nodePinsZone.Add(pinsSpace);
        nodePinsZone.Add(outputPinsContainer);

        nodeContainer.Add(nodeTitle);
        nodeContainer.Add(nodePinsZone);

        void ChangeSize(GeometryChangedEvent evt)
        {
            Vector2 deltaSize = new Vector2(
                  nodeContainer.worldBound.width - Mathf.CeilToInt(nodeContainer.worldBound.width / _nodeGridSize.x) * _nodeGridSize.x
                , nodeContainer.worldBound.height - Mathf.CeilToInt(nodeContainer.worldBound.height / _nodeGridSize.y) * _nodeGridSize.y);

            nodeContainer.style.width = nodeContainer.worldBound.width - deltaSize.x;
            nodeContainer.style.height = nodeContainer.worldBound.height - deltaSize.y;

            nodeContainer.UnregisterCallback<GeometryChangedEvent>(ChangeSize);
        }

        nodeContainer.RegisterCallback<GeometryChangedEvent>(ChangeSize);

        nodeContainer.RegisterCallback<PointerDownEvent>(evt =>
        {
            if (evt.button != _dragMouseIndex) { return; }

            _currentDragElement = new DragNode(
                  this
                , GetUINodeFromTitle(nodeContainer).Node
                , nodeConfig
                , nodeContainer
                , GetMousePosition() - nodeContainer.worldBound.position
                , nodeContainer.layout.position);

            SetManagerBtnState(_managerBtnDeleteNodeState);
        });

        nodeContainer.style.left = node.Position.x;
        nodeContainer.style.top = node.Position.y;

        return nodeContainer;
    }

    private void RenderGreenprints()
    {
        foreach (UINode uINode in _greenprints.UINodes)
        {
            Node node = uINode.Node;
            VisualElement nodeTitle = RenderNode(node);

            uINode.NodeTitle = nodeTitle;

            _nodesLayer.Add(nodeTitle);
        }

        GreenprintsUIData uIData = GetUIData();

        VisualElement GetPinIcon(UINode uINode, Pin pin)
        {
            return uINode.NodeTitle.Query<VisualElement>(
                className: uIData.NodePinContainerClassName).ToList().FirstOrDefault(
                pinContainer => pinContainer == pin.PinContainer).Q<VisualElement>(className: uIData.NodePinClassName);
        }

        foreach (UINode uINode in _greenprints.UINodes)
        {
            foreach (Pin pin in uINode.Node.Pins)
            {
                if (!_greenprints.IsPinInput(pin))
                {
                    foreach (PinConnection pinConnection in _greenprints.GetPinConnectionsFromOutputPin(pin))
                    {
                        VisualElement outputPinIcon = GetPinIcon(uINode, pin);

                        UINode inputPinUINode = _greenprints.UINodes.FirstOrDefault(uINode => uINode.Node == pinConnection.InputPin.Owner);

                        VisualElement inputPinIcon = GetPinIcon(inputPinUINode, pinConnection.InputPin);

                        pinConnection.RerenderPinWire(outputPinIcon, inputPinIcon);
                    }
                }
            }
        }
    }
}

[System.Serializable]
public class GreenprintsManagerBtnState
{
    [SerializeField] private string _btnText;

    public string BtnText => _btnText;
}
