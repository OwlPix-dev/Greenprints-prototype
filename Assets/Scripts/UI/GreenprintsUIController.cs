using System.Linq;
using System;
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
    [SerializeField] private Vector2Int _pinConnectionPointGridSize = new Vector2Int(5, 5);

    [SerializeField] private Vector2 _availableNodeOffset = new Vector2(40f, 40f);
    [SerializeField] private Vector2 _pinConnectionPointOffset = Vector2.zero;

    [SerializeField] private int _dragMouseIndex;

    private VisualElement _root;

    private ScrollView _canvasScroller;
    private VisualElement _canvas;

    private VisualElement _nodesLayer;
    private VisualElement _pinConnectionsLayer;

    private Button _nodeManagerBtn;
    private VisualElement _availableNodesContainer;

    private GreenprintsDragElement _currentDragElement;

    private Greenprints _greenprints;

    private PinConnection _pinConnectionDebug;

    private bool _isDragging => _currentDragElement != null;

    public Vector2Int NodeGridSize => _nodeGridSize;
    public Vector2Int PinConnectionPointGridSize => _pinConnectionPointGridSize;

    public VisualElement Root => _root;

    public VisualElement NodesLayer => _nodesLayer;
    public VisualElement PinConnectionsLayer => _pinConnectionsLayer;

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
                Debug.Log($"--- node {uINode.Node}, {uINode.NodeTitle.worldBound.size}");
            }

            _pinConnectionDebug = new PinConnection(
                  this
                , Color.white
                , 4f
                , new Vector2(100, 100)
                , new Vector2(250, 200));

            _pinConnectionsLayer.Add(_pinConnectionDebug);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            _pinConnectionDebug.AddPoint(1, new Vector2(200, 100));
        }

        if (!_isDragging) { return; }

        _currentDragElement.MoveDragElement();

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

            GreenprintsClassNamesData uIData = GetUIData();

            _canvasScroller = _root.Q<ScrollView>(className: uIData.CanvasScrollerClassName);
            _canvas = _root.Q<VisualElement>(className: uIData.CanvasClassName);

            _nodesLayer = _canvas.Q<VisualElement>(uIData.NodesLayerName);
            _pinConnectionsLayer = _canvas.Q<VisualElement>(uIData.PinsConnectionsLayerName);

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
        SetActive(false, null);
    }

    private void SetActive(bool isActive, Greenprints greenprints)
    {
        _greenprints = greenprints;
        _uIDocument.enabled = isActive;
    }

    private GreenprintsClassNamesData GetUIData()
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
        Vector2 range = new Vector2(
                  _canvasScroller.contentContainer.layout.width - _canvasScroller.layout.width
                , _canvasScroller.contentContainer.layout.height - _canvasScroller.layout.height);

        Vector2 scrollFactor = new Vector2(
              range.x > 0 ? _canvasScroller.scrollOffset.x / range.x : 0f
            , range.y > 0 ? _canvasScroller.scrollOffset.y / range.y : 0f);

        Rect canvasScrollerTransform = _canvasScroller.contentViewport.worldBound;

        position += new Vector2(
              (_greenprints.CanvasSize.x - canvasScrollerTransform.width) * scrollFactor.x - canvasScrollerTransform.x
            , (_greenprints.CanvasSize.y - canvasScrollerTransform.height) * scrollFactor.y - canvasScrollerTransform.y);

        position -= new Vector2(
              position.x - MathF.Round(position.x / gridSize.x) * gridSize.x
            , position.y - MathF.Round(position.y / gridSize.y) * gridSize.y);

        return position;
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

    public VisualElement RenderNodeConfig(NodeConfig nodeConfig)
    {
        GreenprintsClassNamesData uIData = GetUIData();

        VisualElement RenderPinsContainer(PinConfig[] pinConfigs, PinDirection pinDirection, string pinContainerClassName)
        {
            VisualElement pinsContainer = new VisualElement();
            pinsContainer.AddToClassList(uIData.NodePinsContainerClassName);

            foreach (PinConfig pinConfig in pinConfigs)
            {
                VisualElement pinContainer = new VisualElement();
                pinContainer.AddToClassList(pinContainerClassName);

                VisualElement pinIcon = new VisualElement();
                pinIcon.AddToClassList(uIData.NodePinClassName);

                Sprite icon = null;

                foreach (PinIconsData pinIconsData in _greenprints.PinsIconsData)
                {
                    if (pinIconsData.ValueType == pinConfig.ValueType)
                    {
                        icon = pinDirection switch
                        {
                            PinDirection.Input => pinIconsData.UnconnectInputIcon,
                            PinDirection.Output => pinIconsData.UnconnectOutputIcon,
                            _ => null,
                        };
                    }
                }

                pinIcon.style.backgroundImage = new StyleBackground(icon);

                Label pinName = new Label();
                pinName.AddToClassList(uIData.NodePinNameClassName);

                pinName.text = pinConfig.PinName;

                pinContainer.Add(pinIcon);
                pinContainer.Add(pinName);

                pinsContainer.Add(pinContainer);
            }

            return pinsContainer;
        }

        VisualElement nodeContainer = new VisualElement();
        nodeContainer.AddToClassList(uIData.NodeContainerClassName);

        Label nodeTitle = new Label();
        nodeTitle.AddToClassList(uIData.NodeTitleClassName);

        nodeTitle.text = nodeConfig.NodeName;
        nodeTitle.style.backgroundColor = new StyleColor(nodeConfig.NodeAffiliation.AffiliationColor);

        VisualElement nodePinsZone = new VisualElement();
        nodePinsZone.AddToClassList(uIData.NodePinsZoneClassName);

        VisualElement inputPinsContainer = RenderPinsContainer(
              nodeConfig.InputPins
            , PinDirection.Input
            , uIData.NodeInputPinContainerClassName);

        VisualElement pinsSpace = new VisualElement();
        pinsSpace.AddToClassList(uIData.NodePinsSpaceClassName);

        VisualElement outputPinsContainer = RenderPinsContainer(
              nodeConfig.OutputPins
            , PinDirection.Output
            , uIData.NodeOutputPinContainerClassName);

        nodePinsZone.Add(inputPinsContainer);
        nodePinsZone.Add(pinsSpace);
        nodePinsZone.Add(outputPinsContainer);

        nodeContainer.Add(nodeTitle);
        nodeContainer.Add(nodePinsZone);

        void ChangeSize(GeometryChangedEvent evt)
        {
            Vector2 deltaSize = new Vector2(
                  nodeContainer.worldBound.width - MathF.Round(nodeContainer.worldBound.width / _nodeGridSize.x) * _nodeGridSize.x
                , nodeContainer.worldBound.height - MathF.Round(nodeContainer.worldBound.height / _nodeGridSize.y) * _nodeGridSize.y);

            pinsSpace.style.width = pinsSpace.worldBound.width - deltaSize.x;
            pinsSpace.style.height = pinsSpace.worldBound.height - deltaSize.y;

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

        return nodeContainer;
    }

    private VisualElement RenderNode(Node node)
    {
        VisualElement nodeContainer = RenderNodeConfig(node.NodeConfig);

        nodeContainer.style.left = node.Position.x;
        nodeContainer.style.top = node.Position.y;

        return nodeContainer;
    }

    private void RenderGreenprints()
    {
        foreach (UINode uINode in _greenprints.UINodes)
        {
            VisualElement nodeTitle = RenderNode(uINode.Node);

            uINode.NodeTitle = nodeTitle;

            _nodesLayer.Add(nodeTitle);
        }
    }
}

[System.Serializable]
public class GreenprintsManagerBtnState
{
    [SerializeField] private string _btnText;

    public string BtnText => _btnText;
}
