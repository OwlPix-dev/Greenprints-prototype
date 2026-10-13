using System.Collections.Generic;
using UnityEngine;

public class ActiveNode : NodeConfig
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Greenprints.Nodes.ForEach(node =>
            {
                if (node.NodeConfig == this)
                {
                    Greenprints.NodesInWork.Add(node);
                }
            });
        }
    }

    public override void Active(Node node)
    {
        ActiveExecutionOutputPin(node, 0);
    }

    public override List<Pin> GetPins(Node owner)
    {
        return new List<Pin>()
        {
            new ExecutionOutputPin("", owner, Greenprints)
        };
    }
}
