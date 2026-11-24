using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Greenprints : MonoBehaviour
{
    [SerializeField] private NodeConfig[] _availableNodes;

    [SerializeField] private PinUIData[] _pinsIconsData;

    [SerializeField] private Vector2 _canvasSize = new Vector2(2000, 1600);

    private List<Node> _nodes = new List<Node>();

    private List<Node> _nodesInWork = new List<Node>();

    private float _timer;

    public NodeConfig[] AvailableNodes => _availableNodes;

    public PinUIData[] PinsIconsData => _pinsIconsData;

    public Vector2 CanvasSize => _canvasSize;

    public List<Node> Nodes => _nodes;

    public List<Node> NodesInWork => _nodesInWork;

    private void Update()
    {
        if (_nodesInWork.Count == 0) { return; }

        if (_timer <= 0)
        {
            Node workNode = _nodesInWork[0];
            NodeConfig nodeConfig = workNode.NodeConfig;

            nodeConfig.Active(workNode);
            _timer = nodeConfig.DelayNextNode;

            _nodesInWork.Remove(workNode);

            if (_nodesInWork.Count <= 0)
            {
                ResetOutputDataPins();
            }
        }
        else
        {
            _timer -= Time.deltaTime;
        }
    }

    public void ResetOutputDataPins()
    {
        foreach (Node node in Nodes)
        {
            foreach (Pin pin in node.Pins)
            {
                if (pin is DataOutputPin dataOutputPin)
                {
                    dataOutputPin.Value = null;
                }
            }
        }
    }
}
