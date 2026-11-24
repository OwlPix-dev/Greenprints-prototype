using UnityEngine;
using UnityEngine.UIElements;
using NewPowerSupply.Geenprints.Pins;

namespace NewPowerSupply.UI.UIGreenprints
{
    public static class PinUIUtility
    {
        public static PinUIData GetPinUIData(
             GreenprintsValueType valueType
            , GreenprintsUIData uIData
            , GreenprintsValueTypesData valueTypesData)
        {
            if (valueType == valueTypesData.IntValue)
            {
                return uIData.IntPinUIData;
            }
            else if (valueType == valueTypesData.FloatValue)
            {
                return uIData.FloatPinUIData;
            }
            else if (valueType == valueTypesData.BoolValue)
            {
                return uIData.BoolPinUIData;
            }
            else if (valueType == valueTypesData.StringValue)
            {
                return uIData.StringPinUIData;
            }
            else if (valueType == null)
            {
                return uIData.ExecutionPinUIData;
            }

            return null;
        }

        public static void SetPinIcon(Pin pin, GreenprintsUIData uIData, GreenprintsValueTypesData valueTypesData)
        {
            GreenprintsValueType valueType = pin is DataPin dataPin ? dataPin.GreenprintsValueType : null;

            PinUIData pinUIData = GetPinUIData(valueType, uIData, valueTypesData);

            Sprite icon = (PinUtility.IsPinInput(pin), pin.IsConnected()) switch
            {
                (true, false) => pinUIData.UnconnectInputIcon,
                (true, true) => pinUIData.ConnectInputIcon,
                (false, false) => pinUIData.UnconnectOutputIcon,
                (false, true) => pinUIData.ConnectOutputIcon
            };

            VisualElement pinIcon = pin.PinContainer.Q<VisualElement>(className: uIData.NodePinClassName);
            pinIcon.style.backgroundImage = new StyleBackground(icon);
        }

        public static Vector2 GetDragElementPosition(
              Vector2 offset
            , Vector2 gridSize
            , VisualElement dragElement
            , VisualElement canvas
            , ScrollView scrollView
            , Vector2? position = null)
        {


            //position = UIPositionUtility.GetPositionFromScroller(scrollView, position.Value, gridSize);



            return position.Value;
        }
    }
}