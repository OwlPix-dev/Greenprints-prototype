using UnityEngine;

public class SubtractNumbersNode : MathOperationNode
{
    public override float GetMathOperation(float number1, float number2)
    {
        return number1 - number2;
    }
}
