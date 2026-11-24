using UnityEngine;
using UnityEngine.UIElements;
using NewPowerSupply.UI;

public class DragPinWirePoint : GreenprintsDragElement
{
    private PinConnection _pinConnection;

    private VisualElement _pointTitle;

    public PinConnection PinConnection => _pinConnection;

    public DragPinWirePoint(
          GreenprintsUIController greenprintsUIController
        , PinConnection pinConnection
        , VisualElement pointTitle)
        : base(greenprintsUIController)
    {
        _pinConnection = pinConnection;

        _pointTitle = pointTitle;

        _pointTitle.pickingMode = PickingMode.Ignore;
    }

    public override void MoveDragElement()
    {
        Vector2 position = DragAndDropUtility.GetDragElementPosition(
              GreenprintsUIController.PinWirePointOffset
            , GreenprintsUIController.PinWirePointGridSize
            , _pointTitle
            , GreenprintsUIController.CanvasScroller
            , GreenprintsUIController.Canvas);

        _pointTitle.style.left = position.x;
        _pointTitle.style.top = position.y;
    }

    public override void DropDragElement()
    {
        _pointTitle.pickingMode = PickingMode.Position;
    }
}
