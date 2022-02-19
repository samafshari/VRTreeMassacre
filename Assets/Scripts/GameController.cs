using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class GameController : MonoBehaviour
{
    protected SkiPole[] poles;

    protected float GetThumbstickX(XRNode node)
    {
        var hands = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(node, hands);
        foreach (var item in hands)
        {
            if (!item.isValid) continue;
            if (item.TryGetFeatureValue(CommonUsages.primary2DAxis, out var v))
                return v.x;
        }
        return 0;
    }

    protected float GetThumbstickX()
    {
        var thumbstick = GetThumbstickX(XRNode.LeftHand) + GetThumbstickX(XRNode.RightHand);

        if (poles == null)
            poles = FindObjectsOfType<SkiPole>();

        foreach (var item in poles)
        {
            thumbstick -= item.X * 100.0f;
            break;
        }

        return thumbstick;
    }

    protected void Vibrate(float amplitude = 0.5f, float duration = 1.0f)
    {
        var nodes = new[] { XRNode.RightHand, XRNode.LeftHand, XRNode.Head };
        foreach (var node in nodes)
        {
            InputDevice device = InputDevices.GetDeviceAtXRNode(node);
            HapticCapabilities capabilities;
            if (device.TryGetHapticCapabilities(out capabilities))
            {
                if (capabilities.supportsImpulse)
                {
                    uint channel = 0;
                    device.SendHapticImpulse(channel, amplitude, duration);
                }
            }
        }
    }
}
