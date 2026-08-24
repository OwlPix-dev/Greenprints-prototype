public class ExecutionPin : Pin
{
    public ExecutionPin(string pinName, PinDirection pinDirection, Node owner) : base(pinName, pinDirection, owner) { }

    public override bool IsCanConnect(Pin connectPin)
    {
        if (!base.IsCanConnect(connectPin) ||
            !(connectPin is ExecutionPin) ||
            (PinDirection == PinDirection.Output && Connections.Count > 0))
        {
            return false;
        }

        return true;
    }
}
