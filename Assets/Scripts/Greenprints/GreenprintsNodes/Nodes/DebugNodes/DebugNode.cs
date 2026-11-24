using System.Collections.Generic;
using UnityEngine;

public class DebugNode : NodeConfig
{
    public override void Active(Node node)
    {
        string text = $"{GetDataInputPinValue(node, 1)}";
        Debug.Log($"Debug: {(text == null ? "Debug text is empty" : text)}");

        ActiveExecutionOutputPin(node, 2);
    }

    public override List<Pin> GetPins(Node owner, GreenprintsValueTypesData valueTypesData)
    {
        return new List<Pin>()
        {
              new ExecutionInputPin("", owner, Greenprints)
            , new DataInputPin ("Text", owner, Greenprints, valueTypesData.StringValue)
            , new ExecutionOutputPin("", owner, Greenprints)
        };
    }
}
