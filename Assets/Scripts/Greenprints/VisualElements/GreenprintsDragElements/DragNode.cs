using UnityEngine;
using UnityEngine.UIElements;

public class DragNode : GreenprintsDragElement
{
    private Node _node;
    private NodeConfig _nodeConfig;

    private VisualElement _nodeTitle;
    private Label _nodeName;

    private Vector2 _nodeOffset;
    private Vector2 _startPosition;

    private VisualElement _currentDragElement;

    public DragNode(
          GreenprintsUIController greenprintsUIController
        , Node node
        , NodeConfig nodeConfig
        , VisualElement nodeTitle
        , Vector2 nodeOffset
        , Vector2 startPosition)
        : base(greenprintsUIController)
    {
        _node = node;
        _nodeConfig = nodeConfig;

        _nodeTitle = nodeTitle;

        _nodeOffset = nodeOffset;
        _startPosition = startPosition;

        string nodeNameClassName = GreenprintsUIAssetReferences.Instance.GreenprintsUIData.DragNodeNameClassName;
        _nodeName = GreenprintsUIController.Root.Q<Label>(className: nodeNameClassName);

        _nodeName.style.backgroundColor = _nodeConfig.NodeAffiliation.AffiliationColor;
        _nodeName.text = _nodeConfig.TitleName;

        SetCurrentDragElement(_nodeTitle ?? _nodeName, true);

        GreenprintsUIController.OnCanvasHover += PickNodeTitle;
        GreenprintsUIController.OnCanvasLeave += PickNodeName;
    }

    public override void MoveDragElement()
    {
        Vector2 position = GreenprintsUIController.GetMousePosition();

        if (_currentDragElement == _nodeTitle)
        {
            position -= _nodeOffset;

            position = GreenprintsUIController.GetCanvasPosition(position, GreenprintsUIController.NodeGridSize);
            
            Vector2 maxPosition = new Vector2(
              GreenprintsUIController.NodesLayer.worldBound.width - _nodeTitle.worldBound.width
            , GreenprintsUIController.NodesLayer.worldBound.height - _nodeTitle.worldBound.height);

            position = new Vector2(
              Mathf.Clamp(position.x, 0, maxPosition.x)
            , Mathf.Clamp(position.y, 0, maxPosition.y));
        }

        _currentDragElement.style.left = position.x;
        _currentDragElement.style.top = position.y;
    }

    public override void DropDragElement()
    {
        if (_currentDragElement == _nodeTitle)
        {
            if (_node == null)
            {
                Node newNode = _nodeConfig.GetNode(_nodeTitle.worldBound.position, GetGreenprints());

                GreenprintsUIController.Greenprints.UINodes.Add(new UINode(newNode, _nodeTitle));
            }
        }
        else
        {
            VisualElement pickElement = GreenprintsUIController.GetPickElement();

            if (pickElement == GreenprintsUIController.NodeManagerBtn)
            {
                if (_node != null)
                {
                    UINode removeUINode = GreenprintsUIController.GetUINodeFromTitle(_nodeTitle);

                    GreenprintsUIController.Greenprints.UINodes.Remove(removeUINode);
                }

                _nodeTitle?.RemoveFromHierarchy();

                SetCurrentDragElement(null);
            }
            else
            {
                if (_node != null)
                {
                    _nodeTitle.style.left = _startPosition.x;
                    _nodeTitle.style.top = _startPosition.y;

                    SetCurrentDragElement(_nodeTitle);
                }
                else
                {
                    SetCurrentDragElement(null);
                }
            }
        }

        GreenprintsUIController.OnCanvasHover -= PickNodeTitle;
        GreenprintsUIController.OnCanvasLeave -= PickNodeName;
    }

    private Greenprints GetGreenprints()
    {
        return GreenprintsUIController.Greenprints;
    }

    private void SetCurrentDragElement(VisualElement dragElement, bool isMoveDragElement = false)
    {
        void SetCurrentDragElementVisibility(Visibility visibility)
        {
            if (_currentDragElement != null)
            {
                _currentDragElement.style.visibility = visibility;
            }
        }

        SetCurrentDragElementVisibility(Visibility.Hidden);
        _currentDragElement = dragElement;
        SetCurrentDragElementVisibility(Visibility.Visible);

        if (isMoveDragElement)
        {
            MoveDragElement();
        }
    }

    private void PickNodeTitle()
    {
        if (_nodeTitle == null)
        {
            _nodeTitle = GreenprintsUIController.RenderNodeConfig(_nodeConfig);

            GreenprintsUIController.NodesLayer.Add(_nodeTitle);
        }

        SetCurrentDragElement(_nodeTitle, true);
    }

    private void PickNodeName()
    {
        SetCurrentDragElement(_nodeName, true);
    }
}
