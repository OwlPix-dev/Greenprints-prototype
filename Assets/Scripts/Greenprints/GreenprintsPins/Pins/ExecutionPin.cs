public class ExecutionPin : Pin
{
    public ExecutionPin(string pinName, PinDirection pinDirection, Node owner, Greenprints greenprints) :
        base(pinName, pinDirection, owner, greenprints) { }

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

    public override void SendSignal(object sendValue = null)
    {
        if (Connections.Count > 0)
        {
            Connections[0].ReceiveSignal();
        }
    }

    public override void ReceiveSignal(object receiveValue = null)
    {
        Greenprints.NodesInWork.Add(Owner);
    }
}
