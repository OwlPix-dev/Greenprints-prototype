using Unity.VisualScripting;
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
        Debug.Log("Active");

        node.Pins[0].SendSignal(null);
    }
}
