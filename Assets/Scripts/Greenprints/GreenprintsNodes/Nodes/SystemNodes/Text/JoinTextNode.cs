using System.Collections.Generic;

public class JoinTextNode : NodeConfig
{
    public override void Active(Node node)
    {
        string text1 = $"{GetDataInputPinValue(node, 0)}";
        string text2 = $"{GetDataInputPinValue(node, 1)}";

        SetDataOutputPinValue(node, 2, $"{text1}{text2}");
    }

    public override List<Pin> GetPins(Node owner)
    {
        GreenprintsValueTypesAssetReferences valueTypes = GreenprintsValueTypesAssetReferences.Instance;

        return new List<Pin>()
        {
              new DataInputPin("Text 1", owner, Greenprints, valueTypes.StringValue)
            , new DataInputPin("Text 2", owner, Greenprints, valueTypes.StringValue)
            , new DataOutputPin("Result", owner, Greenprints, valueTypes.StringValue)
        };
    }
}
