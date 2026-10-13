using UnityEngine;

public abstract class ExecutionPin : Pin
{
    public ExecutionPin(string pinName, Node owner, Greenprints greenprints) : base(pinName, owner, greenprints) { }
}
