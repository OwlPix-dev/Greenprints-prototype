using System.Collections.Generic;
using UnityEngine;

public class Get1NumberNode : NodeConfig
{
    public override void Active(Node node)
    {
        SetDataOutputPinValue(node, 0, 1);
    }

    public override List<Pin> GetPins(Node owner, GreenprintsValueTypesData valueTypesData)
    {
        return new List<Pin>()
        {
            new DataOutputPin("1" , owner, Greenprints, valueTypesData.FloatValue)
        };
    }
}
