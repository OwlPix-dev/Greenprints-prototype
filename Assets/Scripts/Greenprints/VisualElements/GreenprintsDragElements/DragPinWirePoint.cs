using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

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
        Vector2 position = GreenprintsUIController.GetDragElementPosition(
              GreenprintsUIController.PinWirePointOffset
            , GreenprintsUIController.PinWirePointGridSize
            , _pointTitle);

        _pointTitle.style.left = position.x;
        _pointTitle.style.top = position.y;
    }

    public override void DropDragElement()
    {
        //if (_pointTitle != _pinConnection.Points[_pinConnection.Points.Count - 1].PointTitle) { return; }

        //VisualElement pickElement = GreenprintsUIController.GetPickElement();

        //if (pickElement == )

        _pointTitle.pickingMode = PickingMode.Position;
    }
}
