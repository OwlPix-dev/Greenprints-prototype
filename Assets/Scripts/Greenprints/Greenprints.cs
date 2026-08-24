using System.Collections.Generic;
using UnityEngine;

public enum PinDirection { Input, Output }

public class Greenprints : MonoBehaviour
{
    private List<Node> _nodes = new List<Node>();

    private List<Node> _nodesInWork = new List<Node>();

    private float _timer;

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
        }
        else
        {
            _timer -= Time.deltaTime;
        }
    }
}
