using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using NewPowerSupply.UI.UIGreenprints;
using NewPowerSupply.Geenprints.Pins;
using NewPowerSupply.UI;

public class NodeVisualElement : VisualElement
{
    private Node _node;
    private GreenprintsUIController _greenprintsUIController;

    public NodeVisualElement(Node node, GreenprintsUIController greenprintsUIController)
    {
        _node = node;
        _greenprintsUIController = greenprintsUIController;

        NodeConfig nodeConfig = _node.NodeConfig;

        GreenprintsUIData uIData = _greenprintsUIController.UIData;
        GreenprintsValueTypesData valueTypesData = _greenprintsUIController.ValueTypesData;

        AddToClassList(uIData.NodeContainerClassName);

        Label nodeTitle = new Label();
        nodeTitle.AddToClassList(uIData.NodeTitleClassName);

        nodeTitle.text = nodeConfig.NodeName;
        nodeTitle.style.backgroundColor = new StyleColor(nodeConfig.NodeAffiliation.AffiliationColor);

        VisualElement nodePinsZone = new VisualElement();
        nodePinsZone.AddToClassList(uIData.NodePinsZoneClassName);

        VisualElement inputPinsContainer = new VisualElement();
        inputPinsContainer.AddToClassList(uIData.NodePinsContainerClassName);

        VisualElement pinsSpace = new VisualElement();
        pinsSpace.AddToClassList(uIData.NodePinsSpaceClassName);

        VisualElement outputPinsContainer = new VisualElement();
        outputPinsContainer.AddToClassList(uIData.NodePinsContainerClassName);

        foreach (Pin pin in _node.Pins)
        {
            VisualElement pinContainer = new VisualElement();
            pinContainer.AddToClassList(uIData.NodePinContainerClassName);

            VisualElement pinIcon = new VisualElement();
            pinIcon.AddToClassList(uIData.NodePinClassName);

            pin.PinContainer = pinContainer;

            void SetPinIcon(GeometryChangedEvent evt)
            {
                PinUIUtility.SetPinIcon(pin, uIData, valueTypesData);

                pinContainer.UnregisterCallback<GeometryChangedEvent>(SetPinIcon);
            }

            pinContainer.RegisterCallback<GeometryChangedEvent>(SetPinIcon);

            string pinNameText = pin.PinName;
            VisualElement pinName = null;

            if (pinNameText != "")
            {
                pinName = new Label();
                pinName.AddToClassList(uIData.NodePinNameClassName);

                ((Label)pinName).text = pinNameText;
            }
            else
            {
                pinName = new VisualElement();
                pinName.style.flexGrow = 1;
            }

            void ConfigurePin(VisualElement container, params VisualElement[] pinContents)
            {
                pinContainer.Add(pinContents[0]);
                pinContainer.Add(pinContents[1]);

                container.Add(pinContainer);
            }

            if (PinUtility.IsPinInput(pin))
            {
                pinIcon.RegisterCallback<PointerUpEvent>(evt =>
                {
                    if (_greenprintsUIController.CurrentDragElement is DragPinWirePoint dragPinWirePoint &&
                    evt.button == _greenprintsUIController.DragMouseIndex)
                    {
                        Pin wireOwner = dragPinWirePoint.PinConnection.WireOwner;

                        if (wireOwner.IsCanConnect(pin) && pin.IsCanConnect(wireOwner))
                        {
                            dragPinWirePoint.PinConnection.SetInputPin(pin, pinIcon);

                            PinUtility.AddPinConnectionToInputPin(pin, wireOwner, uIData, valueTypesData);

                            evt.StopPropagation();
                        }
                        else
                        {
                            PinConnection[] pinConnections = PinUtility.GetPinConditionalConnectionsFromOutputPin(wireOwner).ToArray();
                            int pinConnectionIndex = Array.IndexOf(pinConnections, dragPinWirePoint.PinConnection);

                            PinUtility.RemovePinWire(wireOwner, pinConnectionIndex, uIData, valueTypesData);
                            dragPinWirePoint.PinConnection.PinWireElement.RemoveFromHierarchy();
                        }
                    }
                });

                ConfigurePin(inputPinsContainer, pinIcon, pinName);
            }
            else
            {
                pinIcon.RegisterCallback<PointerDownEvent>(evt =>
                {
                    if (evt.button == _greenprintsUIController.DragMouseIndex)
                    {
                        Color wireColor = pin is DataPin dataPin ? dataPin.GreenprintsValueType.ValueColor : Color.white;

                        Vector2 startPointPosition = DragAndDropUtility.GetCanvasPosition(
                              pinIcon.worldBound.center
                            , Vector2.zero
                            , _greenprintsUIController.CanvasScroller
                            , _greenprintsUIController.Canvas);

                        PinConnection newPinConnection = new PinConnection(
                              pin
                            , null
                            , _greenprintsUIController
                            , pinIcon
                            , UIPositionUtility.GetMousePosition());

                        PinUtility.AddPinConnectionToOutputPin(pin, newPinConnection, uIData, valueTypesData);

                        evt.StopPropagation();
                    }
                    else if (evt.button == _greenprintsUIController.RemovePinWireMouseIndex)
                    {
                        List<PinConnection> pinConnections = PinUtility.GetPinConditionalConnectionsFromOutputPin(pin);
                        int inputPinIndex = pinConnections.Count - 1;
                        Pin inputPin = pinConnections[inputPinIndex].InputPin;

                        PinUtility.RemovePinWire(pin, inputPinIndex, uIData, valueTypesData);
                    }
                });

                ConfigurePin(outputPinsContainer, pinName, pinIcon);
            }
        }

        nodePinsZone.Add(inputPinsContainer);
        nodePinsZone.Add(pinsSpace);
        nodePinsZone.Add(outputPinsContainer);

        Add(nodeTitle);
        Add(nodePinsZone);

        void ChangeSize(GeometryChangedEvent evt)
        {
            Vector2 nodeGridSize = _greenprintsUIController.NodeGridSize;

            Vector2 deltaSize = new Vector2(
                  worldBound.width - Mathf.CeilToInt(worldBound.width / nodeGridSize.x) * nodeGridSize.x
                , worldBound.height - Mathf.CeilToInt(worldBound.height / nodeGridSize.y) * nodeGridSize.y);

            style.width = worldBound.width - deltaSize.x;
            style.height = worldBound.height - deltaSize.y;

            UnregisterCallback<GeometryChangedEvent>(ChangeSize);
        }

        RegisterCallback<GeometryChangedEvent>(ChangeSize);

        RegisterCallback<PointerDownEvent>(evt =>
        {
            if (evt.button != _greenprintsUIController.DragMouseIndex) { return; }

            _greenprintsUIController.CurrentDragElement = new DragNode(
                  _greenprintsUIController
                , _node
                , nodeConfig
                , this
                , UIPositionUtility.GetMousePosition() - worldBound.position
                , layout.position);

            _greenprintsUIController.ConfigNodeManagerBtnOnDeleteNode();
        });

        style.left = _node.Position.x;
        style.top = _node.Position.y;

        _node.NodeTitle = this;
    }
}
