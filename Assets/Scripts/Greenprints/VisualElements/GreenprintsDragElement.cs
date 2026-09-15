public class GreenprintsDragElement
{
    private GreenprintsUIController _greenprintsUIController;

    public GreenprintsUIController GreenprintsUIController => _greenprintsUIController;

    public GreenprintsDragElement(GreenprintsUIController greenprintsUIController)
    {
        _greenprintsUIController = greenprintsUIController;
    }

    public virtual void MoveDragElement()
    {

    }

    public virtual void DropDragElement()
    {

    }
}
