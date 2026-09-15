using UnityEngine;
using UnityEngine.UIElements;

public class DragPinConnectionPoint : GreenprintsDragElement
{
    private VisualElement _point;

    private Vector2 _pointOffset;

    public DragPinConnectionPoint(
        GreenprintsUIController greenprintsUIController)
        : base(greenprintsUIController)
    {

    }
}
