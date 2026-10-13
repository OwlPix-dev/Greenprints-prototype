using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PinConnection
{
    private GreenprintsUIController _greenprintsUIController;

    private Pin _wireOwner;
    private Pin _inputPin;

    private PinWireElement _pinWireElement;
    private List<PinConnectionPoint> _points = new List<PinConnectionPoint>();

    public Pin WireOwner => _wireOwner;
    public Pin InputPin => _inputPin;

    public PinWireElement PinWireElement => _pinWireElement;
    public List<PinConnectionPoint> Points => _points;

    public PinConnection(
          Pin owner
        , Pin connection
        , GreenprintsUIController greenprintsUIController
        , VisualElement outputPinTitle
        , Vector2 endPointPosition)
    {
        _greenprintsUIController = greenprintsUIController;

        _wireOwner = owner;
        _inputPin = connection;

        RenderPinWire();

        PinConnectionPinTitle pinTitle = new PinConnectionPinTitle(outputPinTitle, _pinWireElement, _greenprintsUIController);
        _points.Insert(0, new PinConnectionPoint(pinTitle, _pinWireElement));

        VisualElement endPointTitle = AddPoint(endPointPosition, 1);

        _greenprintsUIController.CurrentDragElement = new DragPinWirePoint(_greenprintsUIController, this, endPointTitle);
    }

    public void SetInputPin(Pin inputPin, VisualElement pinTitle)
    {
        _inputPin = inputPin;

        _pinWireElement.PickColor();
        _points[_points.Count - 1].PointElement = new PinConnectionPinTitle(pinTitle, _pinWireElement, _greenprintsUIController);
    }

    public void RenderPinWire()
    {
        _pinWireElement = new PinWireElement(_greenprintsUIController, this);

        _greenprintsUIController.PinWiresLayer.Add(_pinWireElement);
    }

    private VisualElement GetPointTitle(Vector2 pointPosition)
    {
        VisualElement point = new VisualElement();
        point.AddToClassList(GreenprintsUIAssetReferences.Instance.GreenprintsUIData.PinWirePointClassName);

        point.style.left = pointPosition.x;
        point.style.top = pointPosition.y;

        point.style.translate = new Translate(
            new Length(-50, LengthUnit.Percent),
            new Length(-50, LengthUnit.Percent)
        );

        point.RegisterCallback<PointerDownEvent>(evt =>
        {
            if (evt.button != _greenprintsUIController.DragMouseIndex) { return; }

            _greenprintsUIController.CurrentDragElement = new DragPinWirePoint(_greenprintsUIController, this, point);
        });

        _pinWireElement.Add(point);

        return point;
    }

    public VisualElement AddPoint(Vector2 pointPosition, int pointIndex)
    {
        VisualElement pointElement = GetPointTitle(pointPosition);

        void AddPointInList(GeometryChangedEvent evt)
        {
            PinConnectionPointTitle pointTitle = new PinConnectionPointTitle(pointElement, _pinWireElement, _greenprintsUIController);
            _points.Insert(pointIndex, new PinConnectionPoint(pointTitle, _pinWireElement));

            pointElement.UnregisterCallback<GeometryChangedEvent>(AddPointInList);
        }

        pointElement.RegisterCallback<GeometryChangedEvent>(AddPointInList);

        _pinWireElement.MarkDirtyRepaint();

        return pointElement;
    }

    public void RerenderPinWire(VisualElement outputPinTitle, VisualElement inputPinTitle)
    {
        RenderPinWire();

        _points[0].PointElement = new PinConnectionPinTitle(outputPinTitle, _pinWireElement, _greenprintsUIController);

        for (int pointIndex = 1; pointIndex < _points.Count - 1; pointIndex++)
        {
            PinConnectionPoint point = _points[pointIndex];

            point.PointElement = new PinConnectionPointTitle(GetPointTitle(point.Position), _pinWireElement, _greenprintsUIController);
        }

        _points[_points.Count - 1].PointElement = new PinConnectionPinTitle(inputPinTitle, _pinWireElement, _greenprintsUIController);

        _pinWireElement.MarkDirtyRepaint();
    }
}

public class PinConnectionPoint
{
    private Vector2 _position;
    private PinConnectionPointElement _pointElement;

    public Vector2 Position => _position;
    public PinConnectionPointElement PointElement
    {
        get => _pointElement;
        set
        {
            _pointElement?.RemoveElement();

            _pointElement = value;

            RegisterPointElement();
        }
    }

    public PinConnectionPoint(PinConnectionPointElement pointElement, PinWireElement pinWireElement)
    {
        _pointElement = pointElement;
        _position = pointElement.PointTitle.worldBound.position;

        RegisterPointElement();
    }

    private void RegisterPointElement()
    {
        void OnElementRemoved(DetachFromPanelEvent evt)
        {
            _position = _pointElement.PointTitle.layout.position;
        }

        _pointElement.PointTitle.RegisterCallback<DetachFromPanelEvent>(OnElementRemoved);
    }
}

public abstract class PinConnectionPointElement
{
    private VisualElement _pointTitle;
    private PinWireElement _pinWireElement;

    private GreenprintsUIController _greenprintsUIController;

    public VisualElement PointTitle => _pointTitle;
    public PinWireElement PinWireElement => _pinWireElement;

    public GreenprintsUIController GreenprintsUIController => _greenprintsUIController;

    public PinConnectionPointElement(VisualElement pointTitle, PinWireElement pinWireElement, GreenprintsUIController greenprintsUIController)
    {
        _pointTitle = pointTitle;
        _pinWireElement = pinWireElement;

        _greenprintsUIController = greenprintsUIController;

        RegisterCallbackMarkDirtyRepaint();
    }

    public abstract Vector2 GetPointElementPosition();

    public abstract void RegisterCallbackMarkDirtyRepaint();

    public virtual void RemoveElement() { }
}

public class PinConnectionPointTitle : PinConnectionPointElement
{
    public PinConnectionPointTitle(VisualElement pointElement, PinWireElement pinWireElement, GreenprintsUIController greenprintsUIController)
        : base(pointElement, pinWireElement, greenprintsUIController) { }

    public override Vector2 GetPointElementPosition()
    {
        return PointTitle.parent.WorldToLocal(PointTitle.worldBound.center);
    }

    public override void RegisterCallbackMarkDirtyRepaint()
    {
        PointTitle.RegisterCallback<GeometryChangedEvent>(evt => PinWireElement.MarkDirtyRepaint());
    }

    public override void RemoveElement()
    {
        PointTitle.RemoveFromHierarchy();
    }
}

public class PinConnectionPinTitle : PinConnectionPointElement
{
    public PinConnectionPinTitle(VisualElement pointElement, PinWireElement pinWireElement, GreenprintsUIController greenprintsUIController)
        : base(pointElement, pinWireElement, greenprintsUIController) { }

    public override Vector2 GetPointElementPosition()
    {
        return GreenprintsUIController.GetCanvasPosition(PointTitle.worldBound.center, Vector2.zero);
    }

    public override void RegisterCallbackMarkDirtyRepaint()
    {
        PointTitle.parent.parent.parent.parent.RegisterCallback<GeometryChangedEvent>(evt => PinWireElement.MarkDirtyRepaint());
    }
}