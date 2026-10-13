using System.Collections.Generic;
using UnityEngine.UIElements;

public abstract class Pin
{
    private string _pinName;

    private Greenprints _greenprints;

    private Node _owner;

    private VisualElement _pinContainer;

    public string PinName => _pinName;

    public Greenprints Greenprints => _greenprints;

    public Node Owner => _owner;

    public VisualElement PinContainer
    {
        get => _pinContainer;
        set => _pinContainer = value;
    }

    public Pin(string pinName, Node owner, Greenprints greenprints)
    {
        _pinName = pinName;

        _greenprints = greenprints;

        _owner = owner;
    }

    public virtual bool IsCanConnect(Pin connectPin)
    {
        if (connectPin != null &&
            connectPin != this &&
            _owner != connectPin.Owner)
        {
            return true;
        }

        return false;
    }

    public abstract bool IsConnected();
}
//public class PinExecutionModule : IPinSignalModule
//{
//    public void Active(Pin pin)
//    {
//        if (pin.IPinDirectionModule is PinOutputModule pinOutputModule && pinOutputModule.PinConnections.Count > 0)
//        {
//            Pin connectionPin = pinOutputModule.PinConnections[0].InputPin;

//            ((PinExecutionModule)connectionPin.IPinSignalModule).Active(connectionPin);
//        }
//        else if (pin.IPinDirectionModule is PinInputModule)
//        {
//            pin.Greenprints.NodesInWork.Add(pin.Owner);
//        }
//    }

//    public bool IsCanConnect(Pin pin)
//    {
//        if (pin.IPinDirectionModule is PinOutputModule pinOutputModule && pinOutputModule.PinConnections.Count > 0)
//        {
//            return false;
//        }

//        return true;
//    }

//    public static bool operator ==(PinExecutionModule left, PinExecutionModule right)
//    {
//        return true;
//    }

//    public static bool operator !=(PinExecutionModule left, PinExecutionModule right)
//    {
//        return false;
//    }

//    public override bool Equals(object obj) => obj is PinExecutionModule;
//    public override int GetHashCode() => 0;
//}

//public class PinDataModule : IPinSignalModule
//{
//    private GreenprintsValueType _greenprintsValueType;
//    private object _value;

//    public GreenprintsValueType GreenprintsValueType => _greenprintsValueType;

//    public PinDataModule(GreenprintsValueType greenprintsValueType)
//    {
//        _greenprintsValueType = greenprintsValueType;

//        Type type = Type.GetType(_greenprintsValueType.ValueName);
//        _value = type.IsValueType ? Activator.CreateInstance(type) : null;
//    }

//    public bool IsCanConnect(Pin pin)
//    {
//        if (pin.IPinDirectionModule is PinInputModule pinInputModule && pinInputModule.IsConnected())
//        {
//            return false;
//        }

//        return true;
//    }

//    public void Active(Pin pin, object signal)
//    {
//        if (pin.IPinDirectionModule is PinOutputModule pinOutputModule)
//        {
//            foreach (PinConnection pinConnection in pinOutputModule.PinConnections)
//            {
//                ((PinDataModule)pinConnection.InputPin.IPinSignalModule).Active(pinConnection.InputPin, signal);
//            }
//        }
//        else if (pin.IPinDirectionModule is PinInputModule)
//        {
//            _value = signal;
//        }
//    }

//    public object GetValue(Pin pin)
//    {
//        if (_value == null)
//        {
//            if (pin.IPinDirectionModule is PinInputModule pinInputModule)
//            {
//                if (pinInputModule.IsConnected())
//                {
//                    Node node = pinInputModule.PinConnections[0].Owner;

//                    if (node.Pins.FirstOrDefault(pin => pin.IPinSignalModule is PinExecutionModule) == null)
//                    {
//                        node.Greenprints.NodesInWork.Add(node);
//                    }
//                }
//            }
//        }

//        return _value;
//    }

//    public void SetValue(object value)
//    {
//        _value = value;
//    }

//    public static bool operator ==(PinDataModule left, PinDataModule right)
//    {
//        return true;
//    }

//    public static bool operator !=(PinDataModule left, PinDataModule right)
//    {
//        return false;
//    }

//    public override bool Equals(object obj) => obj is PinDataModule;
//    public override int GetHashCode() => 0;
//}

//public class PinOutputModule : IPinDirectionModule
//{
//    private List<PinConnection> _pinConditionalConnections = new List<PinConnection>();

//    public List<PinConnection> PinConditionalConnections => _pinConditionalConnections;

//    public List<PinConnection> PinConnections => _pinConditionalConnections.Where(
//        conditionalConnection => conditionalConnection.InputPin != null).ToList();

//    public bool IsConnected()
//    {
//        return _pinConditionalConnections.Count > 0;
//    }

//    public static bool operator ==(PinOutputModule left, PinOutputModule right)
//    {
//        return true;
//    }

//    public static bool operator !=(PinOutputModule left, PinOutputModule right)
//    {
//        return false;
//    }

//    public override bool Equals(object obj) => obj is PinOutputModule;
//    public override int GetHashCode() => 0;
//}

//public class PinInputModule : IPinDirectionModule
//{
//    private List<Pin> _pinConnections = new List<Pin>();

//    public List<Pin> PinConnections => _pinConnections;

//    public bool IsConnected()
//    {
//        return _pinConnections.Count > 0;
//    }

//    public static bool operator ==(PinInputModule left, PinInputModule right)
//    {
//        return true;
//    }

//    public static bool operator !=(PinInputModule left, PinInputModule right)
//    {
//        return false;
//    }

//    public override bool Equals(object obj) => obj is PinInputModule;
//    public override int GetHashCode() => 0;
//}

//public interface IPinDirectionModule
//{
//    bool IsConnected();
//}

//public interface IPinSignalModule
//{
//    bool IsCanConnect(Pin pin);
//}
