using UnityEngine;
using UnityEngine.UIElements;

public class DragNode : GreenprintsDragElement
{
    private Node _node;

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
        _nodeTitle = nodeTitle;

        _node = node ?? nodeConfig.GetNode(Vector2.zero, GreenprintsUIController.Greenprints);

        _nodeOffset = nodeOffset;
        _startPosition = startPosition;

        string nodeNameClassName = GreenprintsUIAssetReferences.Instance.GreenprintsUIData.DragNodeNameClassName;
        _nodeName = GreenprintsUIController.Root.Q<Label>(className: nodeNameClassName);

        _nodeName.style.backgroundColor = _node.NodeConfig.NodeAffiliation.AffiliationColor;
        _nodeName.text = _node.NodeConfig.TitleName;

        SetCurrentDragElement(_nodeTitle ?? _nodeName, true);

        GreenprintsUIController.OnCanvasHover += PickNodeTitle;
        GreenprintsUIController.OnCanvasLeave += PickNodeName;
    }

    public override void MoveDragElement()
    {
        Vector2 position = GreenprintsUIController.GetMousePosition();

        if (_currentDragElement == _nodeTitle)
        {
            position = GreenprintsUIController.GetDragElementPosition(
                  _nodeOffset
                , GreenprintsUIController.NodeGridSize
                , _nodeTitle
                , position);
        }

        _currentDragElement.style.left = position.x;
        _currentDragElement.style.top = position.y;
    }

    public override void DropDragElement()
    {
        bool isNodeInList = GreenprintsUIController.GetUINodeFromTitle(_nodeTitle) != null;

        if (_currentDragElement == _nodeTitle)
        {
            if (!isNodeInList)
            {
                GreenprintsUIController.Greenprints.UINodes.Add(new UINode(_node, _nodeTitle));
            }
        }
        else
        {
            VisualElement pickElement = GreenprintsUIController.GetPickElement();

            if (pickElement == GreenprintsUIController.NodeManagerBtn)
            {
                if (isNodeInList)
                {
                    GreenprintsUIController.Greenprints.RemoveNodePinsConnections(_node, GreenprintsUIController);

                    UINode removeUINode = GreenprintsUIController.GetUINodeFromTitle(_nodeTitle);

                    GreenprintsUIController.Greenprints.UINodes.Remove(removeUINode);
                }

                _nodeTitle?.RemoveFromHierarchy();

                SetCurrentDragElement(null);
            }
            else
            {
                if (isNodeInList)
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
            _nodeTitle = GreenprintsUIController.RenderNode(_node);

            GreenprintsUIController.NodesLayer.Add(_nodeTitle);
        }

        SetCurrentDragElement(_nodeTitle, true);
    }

    private void PickNodeName()
    {
        SetCurrentDragElement(_nodeName, true);
    }
}
