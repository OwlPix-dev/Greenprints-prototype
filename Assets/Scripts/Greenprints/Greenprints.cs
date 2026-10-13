using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Greenprints : MonoBehaviour
{
    [SerializeField] private NodeConfig[] _availableNodes;

    [SerializeField] private PinUIData[] _pinsIconsData;

    [SerializeField] private Vector2 _canvasSize = new Vector2(2000, 1600);

    private List<UINode> _uINodes = new List<UINode>();

    private List<Node> _nodesInWork = new List<Node>();

    private float _timer;

    public NodeConfig[] AvailableNodes => _availableNodes;

    public PinUIData[] PinsIconsData => _pinsIconsData;

    public Vector2 CanvasSize => _canvasSize;

    public List<UINode> UINodes => _uINodes;

    public List<Node> NodesInWork => _nodesInWork;

    public List<Node> Nodes => _uINodes.Select(uINode => uINode.Node).ToList();

    private void Update()
    {
        if (_nodesInWork.Count == 0) { return; }

        if (_timer <= 0)
        {
            Node workNode = _nodesInWork[0];
            NodeConfig nodeConfig = workNode.NodeConfig;
            Debug.Log($"Work node {nodeConfig.NodeName}");

            nodeConfig.Active(workNode);
            _timer = nodeConfig.DelayNextNode;

            _nodesInWork.Remove(workNode);
        }
        else
        {
            _timer -= Time.deltaTime;
        }
    }
}
