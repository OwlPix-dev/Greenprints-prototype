using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PinConnection : VisualElement
{
    private List<VisualElement> _points = new List<VisualElement>();
    private Color _connectionColor;
    private float _connectionWidth;

    private GreenprintsUIController _greenprintsUIController;

    public PinConnection(
          GreenprintsUIController greenprintsUIController
        , Color color
        , float width
        , Vector2 startPointPosition
        , Vector2 endPointPosition)
    {
        _greenprintsUIController = greenprintsUIController;

        _connectionColor = color;
        _connectionWidth = width;

        AddPoint(0, startPointPosition, Visibility.Hidden);
        AddPoint(1, endPointPosition, Visibility.Hidden);

        generateVisualContent += OnGenerateVisualContent;
    }

    public void AddPoint(int pointIndex, Vector2 pointPosition, Visibility pointVisibility = Visibility.Visible)
    {
        VisualElement pointTitle = new VisualElement();
        pointTitle.AddToClassList(GreenprintsUIAssetReferences.Instance.GreenprintsUIData.PinConnectionPointClassName);

        pointTitle.style.left = pointPosition.x;
        pointTitle.style.top = pointPosition.y;

        pointTitle.style.visibility = pointVisibility;

        pointTitle.RegisterCallback<PointerDownEvent>(evt =>
        {
            _greenprintsUIController.CurrentDragElement = new DragPinConnectionPoint(_greenprintsUIController);
            //if (_dragPoint != null) { return; }

            //_dragPoint = pointTitle;

            //pointTitle.CapturePointer(evt.pointerId);
        });

        //pointTitle.RegisterCallback<PointerMoveEvent>(evt =>
        //{
        //    if (_dragPoint != pointTitle) { return; }

        //    Vector2 position = _greenprintsUIController.GetMousePosition();
        //    position -= new Vector2(pointTitle.layout.width / 2, pointTitle.layout.height / 2);
        //    position = _greenprintsUIController.GetCanvasPosition(position, _gridSize);

        //    pointTitle.style.left = position.x;
        //    pointTitle.style.top = position.y;
        //});

        //pointTitle.RegisterCallback<PointerUpEvent>(evt =>
        //{
        //    pointTitle.ReleasePointer(evt.pointerId);

        //    _dragPoint = null;
        //});

        pointTitle.RegisterCallback<GeometryChangedEvent>(evt => MarkDirtyRepaint());

        _points.Insert(pointIndex, pointTitle);
        Insert(pointIndex, pointTitle);

        MarkDirtyRepaint();
    }

    private void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        if (_points == null || _points.Count < 2) { return; }

        Painter2D painter = mgc.painter2D;

        painter.strokeColor = _connectionColor;
        painter.lineWidth = _connectionWidth;
        painter.lineJoin = LineJoin.Round;

        painter.BeginPath();

        painter.MoveTo(GetCenterPointPosition(_points[0]));

        foreach (VisualElement point in _points)
        {
            painter.LineTo(GetCenterPointPosition(point));
        }

        painter.Stroke();
    }

    private Vector2 GetCenterPointPosition(VisualElement point)
    {
        return point.layout.position + new Vector2(_points[0].layout.width / 2, _points[0].layout.height / 2);
    }
}
