using UnityEngine.UIElements;

public class UINode
{
    private Node _node;
    private VisualElement _nodeTitle;

    public Node Node => _node;
    public VisualElement NodeTitle
    {
        get => _nodeTitle;
        set
        {
            _nodeTitle = value;
            RegisterTitle();
        }
    }

    public UINode(Node node, VisualElement nodeTitle)
    {
        _node = node;
        _nodeTitle = nodeTitle;

        RegisterTitle();
    }

    private void RegisterTitle()
    {
        void OnElementRemoved(DetachFromPanelEvent evt)
        {
            _node.Position = _nodeTitle.layout.position;
        }

        _nodeTitle.RegisterCallback<DetachFromPanelEvent>(OnElementRemoved);
    }
}
