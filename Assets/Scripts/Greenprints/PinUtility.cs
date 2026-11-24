using System;
using System.Collections.Generic;
using System.Linq;
using NewPowerSupply.UI.UIGreenprints;
using UnityEngine;

namespace NewPowerSupply.Geenprints.Pins
{
    public static class PinUtility
    {
        public static bool IsPinInput(Pin pin)
        {
            return pin is DataInputPin || pin is ExecutionInputPin;
        }

        public static List<PinConnection> GetPinConnectionsFromOutputPin(Pin pin)
        {
            if (pin is DataOutputPin dataOutputPin)
            {
                return dataOutputPin.InputConnections;
            }
            else if (pin is ExecutionOutputPin executionOutputPin)
            {
                List<PinConnection> returnList = new List<PinConnection>();

                PinConnection inputConnection = executionOutputPin.InputConnection;

                if (inputConnection != null)
                {
                    returnList.Add(inputConnection);
                }

                return returnList;
            }

            return null;
        }

        public static List<PinConnection> GetPinConditionalConnectionsFromOutputPin(Pin pin)
        {
            if (pin is DataOutputPin dataOutputPin)
            {
                return dataOutputPin.InputConditionalConnections;
            }
            else if (pin is ExecutionOutputPin executionOutputPin)
            {
                List<PinConnection> returnList = new List<PinConnection>();

                PinConnection inputConditionalConnection = executionOutputPin.InputConditionalConnection;

                if (inputConditionalConnection != null)
                {
                    returnList.Add(inputConditionalConnection);
                }

                return returnList;
            }

            return null;
        }

        public static List<Pin> GetPinConnectionsFromInputPin(Pin pin)
        {
            if (pin is DataInputPin dataInputPin)
            {
                List<Pin> returnList = new List<Pin>();

                Pin outputConnection = dataInputPin.OutputConnection;

                if (outputConnection != null)
                {
                    returnList.Add(outputConnection);
                }

                return returnList;
            }
            else if (pin is ExecutionInputPin executionInputPin)
            {
                return executionInputPin.OutputConnections.Select(connection => (Pin)connection).ToList();
            }

            return null;
        }

        public static void AddPinConnectionToOutputPin(
              Pin pin
            , PinConnection pinConnection
            , GreenprintsUIData uIData
            , GreenprintsValueTypesData valueTypesData)
        {
            bool isChangeIcon = !pin.IsConnected();

            if (pin is DataOutputPin dataOutputPin)
            {
                dataOutputPin.InputConditionalConnections.Add(pinConnection);
            }
            else if (pin is ExecutionOutputPin executionOutputPin)
            {
                executionOutputPin.InputConditionalConnection = pinConnection;
            }

            if (isChangeIcon)
            {
                PinUIUtility.SetPinIcon(pin, uIData, valueTypesData);
            }
        }

        public static void AddPinConnectionToInputPin(
              Pin pin
            , Pin pinConnection
            , GreenprintsUIData uIData
            , GreenprintsValueTypesData valueTypesData)
        {
            bool isChangeIcon = !pin.IsConnected();

            if (pin is DataInputPin dataInputPin && dataInputPin.OutputConnection == null)
            {
                dataInputPin.OutputConnection = (DataOutputPin)pinConnection;
            }
            else if (pin is ExecutionInputPin executionInputPin)
            {
                executionInputPin.OutputConnections.Add((ExecutionOutputPin)pinConnection);
            }

            if (isChangeIcon)
            {
                PinUIUtility.SetPinIcon(pin, uIData, valueTypesData);
            }
        }

        private static void RemovePinConnectionFromOutputPin(
              Pin pin
            , int index
            , GreenprintsUIData uIData
            , GreenprintsValueTypesData valueTypesData)
        {
            bool isChangeIcon = GetPinConditionalConnectionsFromOutputPin(pin).Count == 1;

            if (pin is DataOutputPin dataOutputPin)
            {
                dataOutputPin.InputConditionalConnections.RemoveAt(index);
            }
            else if (pin is ExecutionOutputPin executionOutputPin)
            {
                executionOutputPin.InputConditionalConnection = null;
            }

            if (isChangeIcon)
            {
                PinUIUtility.SetPinIcon(pin, uIData, valueTypesData);
            }
        }

        private static void RemovePinConnectionFromInputPin(
              Pin pin
            , int index
            , GreenprintsUIData uIData
            , GreenprintsValueTypesData valueTypesData)
        {
            bool isChangeIcon = GetPinConnectionsFromInputPin(pin).Count == 1;

            if (pin is DataInputPin dataInputPin)
            {
                dataInputPin.OutputConnection = null;
            }
            else if (pin is ExecutionInputPin executionInputPin)
            {
                executionInputPin.OutputConnections.RemoveAt(index);
            }

            if (isChangeIcon)
            {
                PinUIUtility.SetPinIcon(pin, uIData, valueTypesData);
            }
        }

        public static void RemovePinWire(
              Pin outputPin
            , int pinWireIndex
            , GreenprintsUIData uIData
            , GreenprintsValueTypesData valueTypesData)
        {
            PinConnection pinConnection = GetPinConditionalConnectionsFromOutputPin(outputPin)[pinWireIndex];

            pinConnection.PinWireElement?.RemoveFromHierarchy();

            Pin inputPin = pinConnection.InputPin;
            if (inputPin != null)
            {
                List<Pin> inputPinConnections = GetPinConnectionsFromInputPin(inputPin);
                int outputPinIndexInInputPinConnections = Array.IndexOf(inputPinConnections.ToArray(), outputPin);

                RemovePinConnectionFromInputPin(inputPin, outputPinIndexInInputPinConnections, uIData, valueTypesData);
            }

            RemovePinConnectionFromOutputPin(outputPin, pinWireIndex, uIData, valueTypesData);
        }

        public static void RemoveNodePinsConnections(Node node, GreenprintsUIData uIData, GreenprintsValueTypesData valueTypesData)
        {
            foreach (Pin inputPin in node.Pins.Where(pin => IsPinInput(pin)).ToList())
            {
                List<Pin> inputPinConnections = GetPinConnectionsFromInputPin(inputPin);

                for (int pinConnectionIndex = 0; pinConnectionIndex < inputPinConnections.Count; pinConnectionIndex++)
                {
                    Pin outputPin = inputPinConnections[pinConnectionIndex];
                    List<PinConnection> outputPinConnections = GetPinConnectionsFromOutputPin(outputPin);
                    PinConnection outputPinConnection = outputPinConnections.FirstOrDefault(pinConnection => pinConnection.InputPin == inputPin);
                    int inputPinIndexInOutputPinConnections = Array.IndexOf(outputPinConnections.ToArray(), outputPinConnection);

                    RemovePinWire(outputPin, inputPinIndexInOutputPinConnections, uIData, valueTypesData);
                }
            }

            foreach (Pin outputPin in node.Pins.Where(pin => !IsPinInput(pin)).ToList())
            {
                int pinConnectionCount = GetPinConditionalConnectionsFromOutputPin(outputPin).Count;

                for (int pinConnectionIndex = pinConnectionCount - 1; pinConnectionIndex >= 0; pinConnectionIndex--)
                {
                    Debug.Log(pinConnectionIndex);

                    RemovePinWire(outputPin, pinConnectionIndex, uIData, valueTypesData);
                }
            }
        }
    }
}
