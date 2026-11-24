using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class MathOperationNode : NodeConfig
{
    public override void Active(Node node)
    {
        float num1 = Convert.ToSingle(GetDataInputPinValue(node, 0));
        float num2 = Convert.ToSingle(GetDataInputPinValue(node, 1));

        SetDataOutputPinValue(node, 2, GetMathOperation(num1, num2));
    }

    public override List<Pin> GetPins(Node owner, GreenprintsValueTypesData valueTypesData)
    {
        return new List<Pin>()
        {
              new DataInputPin("Number 1", owner, Greenprints, valueTypesData.FloatValue)
            , new DataInputPin("Number 2", owner, Greenprints, valueTypesData.FloatValue)
            , new DataOutputPin("Result", owner, Greenprints, valueTypesData.FloatValue)
        };
    }

    public abstract float GetMathOperation(float number1, float number2);
}
