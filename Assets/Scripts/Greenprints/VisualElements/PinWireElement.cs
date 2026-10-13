using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class PinWireElement : VisualElement
{
    private GreenprintsUIController _greenprintsUIController;
    private PinConnection _pinConnection;

    private Color _wireColor;
    private float _wireWidth;

    public PinWireElement(GreenprintsUIController greenprintsUIController, PinConnection pinConnection)
    {
        _greenprintsUIController = greenprintsUIController;
        _pinConnection = pinConnection;

        PickColor();

        _wireWidth = GreenprintsUIAssetReferences.Instance.GreenprintsUIData.PinWireWidth;

        generateVisualContent += OnGenerateVisualContent;
    }

    //public VisualElement AddPoint(int pointIndex, Vector2 pointPosition, Visibility pointVisibility = Visibility.Visible)
    //{
    //    VisualElement pointTitle = new VisualElement();
    //    pointTitle.AddToClassList(GreenprintsUIAssetReferences.Instance.GreenprintsUIData.PinConnectionPointClassName);

    //    pointTitle.style.left = pointPosition.x;
    //    pointTitle.style.top = pointPosition.y;

    //    pointTitle.style.translate = new Translate(
    //        new Length(-50, LengthUnit.Percent),
    //        new Length(-50, LengthUnit.Percent)
    //    );

    //    pointTitle.style.visibility = pointVisibility;

    //    pointTitle.RegisterCallback<PointerDownEvent>(evt =>
    //    {
    //        if (evt.button != _greenprintsUIController.DragMouseIndex) { return; }

    //        _greenprintsUIController.CurrentDragElement = new DragPinWirePoint(
    //              _greenprintsUIController
    //            , pointTitle);
    //    });

    //    pointTitle.RegisterCallback<GeometryChangedEvent>(evt => MarkDirtyRepaint());

    //    _pinConnection.Points.Insert(pointIndex, pointTitle);
    //    Insert(pointIndex, pointTitle);

    //    MarkDirtyRepaint();

    //    return pointTitle;
    //}

    private void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        if (_pinConnection.Points.Count < 2) { return; }

        Painter2D painter = mgc.painter2D;

        painter.strokeColor = _wireColor;
        painter.lineWidth = _wireWidth;

        painter.lineJoin = LineJoin.Round;

        painter.BeginPath();

        painter.MoveTo(_pinConnection.Points[0].PointElement.GetPointElementPosition());

        for (int pointIndex = 1; pointIndex < _pinConnection.Points.Count; pointIndex++)
        {
            painter.LineTo(_pinConnection.Points[pointIndex].PointElement.GetPointElementPosition());
        }

        painter.Stroke();
    }

    public void PickColor()
    {
        GreenprintsValueType GetValueType(Pin pin)
        {
            if (pin is DataPin dataPin)
            {
                return dataPin.GreenprintsValueType;
            }
            else
            {
                return null;
            }
        }

        Pin inputPin = _pinConnection.InputPin;

        GreenprintsValueType valueType = GetValueType(inputPin != null ? inputPin : _pinConnection.WireOwner);

        PinUIData pinUIData = _greenprintsUIController.GetPinUIData(valueType);

        _wireColor = pinUIData.PinWireColor;
    }
}
