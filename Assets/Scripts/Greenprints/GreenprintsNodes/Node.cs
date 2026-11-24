using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Node
{
    private NodeConfig _nodeConfig;
    private List<Pin> _pins = new List<Pin>();

    private NodeVisualElement _nodeTitle;
    private Vector2 _position;

    public NodeConfig NodeConfig => _nodeConfig;
    public List<Pin> Pins => _pins;

    public NodeVisualElement NodeTitle
    {
        get => _nodeTitle;
        set
        {
            _nodeTitle = value;
            RegisterTitle();
        }
    }

    public Vector2 Position
    {
        get => _position;
        set => _position = value;
    }

    public Node(NodeConfig nodeConfig, NodeVisualElement nodeTitle, Vector2 position, GreenprintsValueTypesData valueTypesData)
    {
        _nodeConfig = nodeConfig;

        _nodeTitle = nodeTitle;
        _position = position;

        _pins = _nodeConfig.GetPins(this, valueTypesData);

        if (_nodeTitle != null) { RegisterTitle(); }
    }

    private void RegisterTitle()
    {
        void OnElementRemoved(DetachFromPanelEvent evt)
        {
            _position = _nodeTitle.layout.position;
        }

        _nodeTitle.RegisterCallback<DetachFromPanelEvent>(OnElementRemoved);
    }
}
