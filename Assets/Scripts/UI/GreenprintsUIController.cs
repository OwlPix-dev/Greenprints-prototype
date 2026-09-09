using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GreenprintsUIController : MonoBehaviour
{
    [SerializeField] private UIDocument _uIDocument;

    [SerializeField] private ButtonsContainerData _titleBarBtns;

    [SerializeField] private string _workZoneClassName;

    [SerializeField] private string _canvasScrollerClassName;
    [SerializeField] private string _canvasClassName;

    [SerializeField] private string _nodeManagerBtnClassName;
    [SerializeField] private string _availableNodesContainerClassName;
    [SerializeField] private string _availableNodeClassName;

    [SerializeField] private string _dragNodeNameClassName;

    [SerializeField] private string _nodeContainerClassName;
    [SerializeField] private string _nodeTitleClassName;
    [SerializeField] private string _nodePinsZoneClassName;
    [SerializeField] private string _nodePinsContainerClassName;
    [SerializeField] private string _nodePinsSpaceClassName;
    [SerializeField] private string _nodeInputPinContainerClassName;
    [SerializeField] private string _nodeOutputPinContainerClassName;
    [SerializeField] private string _nodePinClassName;
    [SerializeField] private string _nodePinNameClassName;

    [SerializeField] private string _nodeManagerOpenAvailableNodesBtnText = "►Add node";
    [SerializeField] private string _nodeManagerCloseAvailableNodesBtnText = "▼Add node";
    [SerializeField] private string _nodeManagerDeleteNodeBtnText = "Delete Node";

    [SerializeField] private Vector2Int _gridSize = new Vector2Int(20, 20);
    [SerializeField] private Vector2 _availableNodeOffset = new Vector2(40f, 40f);

    [SerializeField] private int _dragMouseIndex;

    private VisualElement _root;

    private ScrollView _canvasScroller;
    private VisualElement _canvas;

    private Button _nodeManagerBtn;
    private VisualElement _availableNodesContainer;

    private VisualElement _dragNodeTitle;
    private Label _dragNodeName;

    private VisualElement _currentDragNodeTitle;

    private NodeConfig _dragNodeConfig;
    private Node _dragNode;

    private Greenprints _greenprints;

    private Vector2 _oldNodePosition;
    private Vector2 _nodeTitleOffset;

    private bool _isDragging => _dragNodeConfig != null;

    public bool IsActive => _greenprints != null;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log($"Debug nodes {_greenprints.UINodes.Count}");

            foreach (UINode uINode in _greenprints.UINodes)
            {
                Debug.Log($"--- node {uINode.Node}");
            }
        }

        if (!_isDragging) { return; }

        SetDragNodePosition(GetMousePosition());

        if (Input.GetMouseButtonUp(_dragMouseIndex))
        {
            VisualElement pickElement = _root.panel.Pick(_currentDragNodeTitle.worldBound.position);

            if (pickElement == _canvas)
            {
                if (_dragNode == null)
                {
                    Node newNode = _dragNodeConfig.GetNode(_currentDragNodeTitle.worldBound.position, _greenprints);

                    _greenprints.UINodes.Add(new UINode(newNode, _dragNodeTitle));
                }
            }
            else if (pickElement == _nodeManagerBtn)
            {
                _greenprints.UINodes.Remove(GetUINodeFromTitle(_dragNodeTitle));

                _dragNodeTitle.RemoveFromHierarchy();
            }
            else
            {
                if (_dragNode != null)
                {
                    CancelDrag();
                }
            }

            SetPickingModeNode(PickingMode.Position, _dragNodeTitle);

            EndDrag();
        }
    }

    public void OpenGreenprints(Greenprints greenprints)
    {
        SetActive(true, greenprints);

        _root = _uIDocument.rootVisualElement;

        void GeometryChanged(GeometryChangedEvent evt)
        {
            _titleBarBtns.DrawButtons(_root);

            _canvasScroller = _root.Q<ScrollView>(className: _canvasScrollerClassName);
            _canvas = _root.Q<VisualElement>(className: _canvasClassName);

            _availableNodesContainer = _root.Q<VisualElement>(className: _availableNodesContainerClassName);

            _dragNodeName = _root.Q<Label>(className: _dragNodeNameClassName);

            foreach (NodeConfig availableNode in _greenprints.AvailableNodes)
            {
                Label availableNodeName = new Label();
                availableNodeName.AddToClassList(_availableNodeClassName);

                availableNodeName.text = availableNode.TitleName;
                availableNodeName.style.backgroundColor = new StyleColor(availableNode.NodeAffiliation.AffiliationColor);

                availableNodeName.RegisterCallback<PointerDownEvent>(evt =>
                {
                    if (evt.button == _dragMouseIndex)
                    {
                        _nodeTitleOffset = _availableNodeOffset;

                        PickNodeConfig(availableNode);

                        SetCurrentDragNodeTitle(_dragNodeName);

                        _canvas.RegisterCallback<PointerEnterEvent>(CanvasHover);
                        _canvas.UnregisterCallback<PointerLeaveEvent>(CanvasHoverLeave);
                    }
                });

                _availableNodesContainer.Add(availableNodeName);
            }

            _canvas.style.width = _greenprints.CanvasSize.x;
            _canvas.style.height = _greenprints.CanvasSize.y;

            _availableNodesContainer.style.visibility = Visibility.Hidden;

            VisualElement workZone = _root.Q<VisualElement>(className: _workZoneClassName);

            _nodeManagerBtn = new Button();
            _nodeManagerBtn.AddToClassList(_nodeManagerBtnClassName);

            _nodeManagerBtn.text = _nodeManagerOpenAvailableNodesBtnText;
            _nodeManagerBtn.clicked += () =>
            {
                ChangeActiveAvailableNodesContainer(_availableNodesContainer.style.visibility == Visibility.Hidden);
            };

            workZone.Add(_nodeManagerBtn);

            _root.UnregisterCallback<GeometryChangedEvent>(GeometryChanged);
        }

        _root.RegisterCallback<GeometryChangedEvent>(GeometryChanged);
    }

    private void CanvasHover(PointerEnterEvent evt)
    {
        if (!_isDragging) { return; }

        SetCurrentDragNodeTitle(_dragNodeTitle);

        _canvas.UnregisterCallback<PointerEnterEvent>(CanvasHover);
        _canvas.RegisterCallback<PointerLeaveEvent>(CanvasHoverLeave);
    }

    private void CanvasHoverLeave(PointerLeaveEvent evt)
    {
        if (!_isDragging) { return; }

        SetCurrentDragNodeTitle(_dragNodeName);

        _canvas.RegisterCallback<PointerEnterEvent>(CanvasHover);
        _canvas.UnregisterCallback<PointerLeaveEvent>(CanvasHoverLeave);
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

    private void ChangeActiveAvailableNodesContainer(bool isActive)
    {
        if (isActive)
        {
            _nodeManagerBtn.text = _nodeManagerCloseAvailableNodesBtnText;
            _availableNodesContainer.style.visibility = Visibility.Visible;
        }
        else
        {
            _nodeManagerBtn.text = _nodeManagerOpenAvailableNodesBtnText;
            _availableNodesContainer.style.visibility = Visibility.Hidden;
        }
    }

    private void PickNodeConfig(NodeConfig nodeConfig)
    {
        _dragNodeConfig = nodeConfig;

        _dragNodeName.text = _dragNodeConfig.NodeName;
        _dragNodeName.style.backgroundColor = _dragNodeConfig.NodeAffiliation.AffiliationColor;
        _dragNodeName.pickingMode = PickingMode.Ignore;

        _dragNodeTitle ??= RenderNodeConfig(nodeConfig);
        SetPickingModeNode(PickingMode.Ignore, _dragNodeTitle);

        ChangeActiveAvailableNodesContainer(false);
        _nodeManagerBtn.text = _nodeManagerDeleteNodeBtnText;

        _canvas.Add(_dragNodeTitle);
    }

    private void PickNode(Node node)
    {
        _dragNode = node;

        PickNodeConfig(node.NodeConfig);

        _canvas.RegisterCallback<PointerLeaveEvent>(CanvasHoverLeave);
        _canvas.UnregisterCallback<PointerEnterEvent>(CanvasHover);
    }

    private void SetDragNodePosition(Vector2 position, bool isMustConsiderOffset = true)
    {
        if (_currentDragNodeTitle == _dragNodeTitle)
        {
            Vector2 range = new Vector2(
                  _canvasScroller.contentContainer.layout.width - _canvasScroller.contentViewport.layout.width
                , _canvasScroller.contentContainer.layout.height - _canvasScroller.contentViewport.layout.height);

            Vector2 scrollFactor = new Vector2(
                  range.x > 0 ? _canvasScroller.scrollOffset.x / range.x : 0f
                , range.y > 0 ? _canvasScroller.scrollOffset.y / range.y : 0f);

            Rect canvasScrollerTransform = _canvasScroller.contentViewport.worldBound;

            position += new Vector2(
                  (_greenprints.CanvasSize.x - canvasScrollerTransform.width) * scrollFactor.x - canvasScrollerTransform.x
                , (_greenprints.CanvasSize.y - canvasScrollerTransform.height) * scrollFactor.y - canvasScrollerTransform.y);

            if (isMustConsiderOffset)
            {
                position += _nodeTitleOffset;
            }

            position -= new Vector2(
                  position.x - MathF.Round(position.x / _gridSize.x) * _gridSize.x
                , position.y - MathF.Round(position.y / _gridSize.y) * _gridSize.y);

            Vector2 maxPosition = new Vector2(
                  _canvas.worldBound.width - _dragNodeTitle.worldBound.width
                , _canvas.worldBound.height - _dragNodeTitle.worldBound.height);

            position = new Vector2(
                  Mathf.Clamp(position.x, 0, maxPosition.x)
                , Mathf.Clamp(position.y, 0, maxPosition.y));
        }

        _currentDragNodeTitle.style.left = position.x;
        _currentDragNodeTitle.style.top = position.y;
    }

    private void CancelDrag()
    {
        SetCurrentDragNodeTitle(_dragNodeTitle);
        SetDragNodePosition(_oldNodePosition, false);
    }

    private void EndDrag()
    {
        _dragNodeName.style.visibility = Visibility.Hidden;
        _dragNodeTitle = null;

        _dragNodeConfig = null;
        _dragNode = null;

        _nodeManagerBtn.text = _nodeManagerOpenAvailableNodesBtnText;
    }

    private Vector2 GetMousePosition()
    {
        Vector2 mousePosition = Input.mousePosition;
        mousePosition.y = Screen.height - mousePosition.y;
        return mousePosition;
    }

    private void SetCurrentDragNodeTitle(VisualElement value)
    {
        _dragNodeName.style.visibility = Visibility.Hidden;
        _dragNodeTitle.style.visibility = Visibility.Hidden;

        _currentDragNodeTitle = value;
        _currentDragNodeTitle.style.visibility = Visibility.Visible;
        SetDragNodePosition(GetMousePosition());
    }

    private void SetPickingModeNode(PickingMode pickingMode, VisualElement node)
    {
        node.pickingMode = pickingMode;

        Label nodeTitle = node.Q<Label>(className: _nodeTitleClassName);
        nodeTitle.pickingMode = pickingMode;
    }

    private UINode GetUINodeFromTitle(VisualElement title)
    {
        return _greenprints.UINodes.FirstOrDefault(uINode => title == uINode.NodeTitle);
    }

    private VisualElement RenderNodeConfig(NodeConfig nodeConfig)
    {
        VisualElement RenderPinsContainer(PinConfig[] pinConfigs, PinDirection pinDirection, string pinContainerClassName)
        {
            VisualElement pinsContainer = new VisualElement();
            pinsContainer.AddToClassList(_nodePinsContainerClassName);

            foreach (PinConfig pinConfig in pinConfigs)
            {
                VisualElement pinContainer = new VisualElement();
                pinContainer.AddToClassList(pinContainerClassName);

                VisualElement pinIcon = new VisualElement();
                pinIcon.AddToClassList(_nodePinClassName);

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
                pinName.AddToClassList(_nodePinNameClassName);

                pinName.text = pinConfig.PinName;

                pinContainer.Add(pinIcon);
                pinContainer.Add(pinName);

                pinsContainer.Add(pinContainer);
            }

            return pinsContainer;
        }

        VisualElement nodeContainer = new VisualElement();
        nodeContainer.AddToClassList(_nodeContainerClassName);

        Label nodeTitle = new Label();
        nodeTitle.AddToClassList(_nodeTitleClassName);

        nodeTitle.text = nodeConfig.NodeName;
        nodeTitle.style.backgroundColor = new StyleColor(nodeConfig.NodeAffiliation.AffiliationColor);

        VisualElement nodePinsZone = new VisualElement();
        nodePinsZone.AddToClassList(_nodePinsZoneClassName);

        VisualElement inputPinsContainer = RenderPinsContainer(
              nodeConfig.InputPins
            , PinDirection.Input
            , _nodeInputPinContainerClassName);

        VisualElement pinsSpace = new VisualElement();
        pinsSpace.AddToClassList(_nodePinsSpaceClassName);

        VisualElement outputPinsContainer = RenderPinsContainer(
              nodeConfig.OutputPins
            , PinDirection.Output
            , _nodeOutputPinContainerClassName);

        nodePinsZone.Add(inputPinsContainer);
        nodePinsZone.Add(pinsSpace);
        nodePinsZone.Add(outputPinsContainer);

        nodeContainer.Add(nodeTitle);
        nodeContainer.Add(nodePinsZone);

        void ChangeSize(GeometryChangedEvent evt)
        {
            Vector2 deltaSize = new Vector2(
                  nodeContainer.worldBound.width - MathF.Round(nodeContainer.worldBound.width / _gridSize.x) * _gridSize.x
                , nodeContainer.worldBound.height - MathF.Round(nodeContainer.worldBound.height / _gridSize.y) * _gridSize.y);

            pinsSpace.style.width = pinsSpace.worldBound.width - deltaSize.x;
            pinsSpace.style.height = pinsSpace.worldBound.height - deltaSize.y;

            nodeContainer.UnregisterCallback<GeometryChangedEvent>(ChangeSize);
        }

        nodeContainer.RegisterCallback<GeometryChangedEvent>(ChangeSize);

        nodeContainer.RegisterCallback<PointerDownEvent>(evt =>
        {
            _dragNodeTitle = nodeContainer;

            _oldNodePosition = nodeContainer.worldBound.position;
            _nodeTitleOffset = _dragNodeTitle.worldBound.position - GetMousePosition();

            PickNode(GetUINodeFromTitle(nodeContainer).Node);

            SetCurrentDragNodeTitle(_dragNodeTitle);
        });

        return nodeContainer;
    }

    private VisualElement RenderNode(Node node)
    {
        VisualElement nodeContainer = RenderNodeConfig(node.NodeConfig);

        return nodeContainer;
    }
}

public class UINode
{
    private Node _node;
    private VisualElement _nodeTitle;

    public Node Node => _node;
    public VisualElement NodeTitle => _nodeTitle;

    public UINode(Node node, VisualElement nodeTitle)
    {
        _node = node;
        _nodeTitle = nodeTitle;
    }
}