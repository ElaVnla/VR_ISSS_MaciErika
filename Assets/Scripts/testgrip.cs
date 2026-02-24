using UnityEngine;
using UnityEngine.XR;

public class GripEdgeExample : MonoBehaviour
{
    
    private InputDevice controllerR;
    private InputDevice controllerL;
    private bool previousGripStateR = false;
    private bool previousGripStateL = false;

    void Start()
    {
        controllerR = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        controllerL = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
    }

    void Update()
    {
        if (controllerR.TryGetFeatureValue(CommonUsages.gripButton, out bool currentGripStateR))
        {
            // Trigger once when pressed
            if (currentGripStateR && !previousGripStateR)
            {
                Debug.Log("Grip pressed!");
            }

            // Trigger once when released
            if (!currentGripStateR && previousGripStateR)
            {
                Debug.Log("Grip released!");
            }

            previousGripStateR = currentGripStateR;
        }
        else
        {
            // Re-fetch the device if disconnected
            controllerR = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        }

        if (controllerL.TryGetFeatureValue(CommonUsages.gripButton, out bool currentGripStateL))
        {
            // Trigger once when pressed
            if (currentGripStateL && !previousGripStateL)
            {
                Debug.Log("Grip pressed!");
            }

            // Trigger once when released
            if (!currentGripStateL && previousGripStateL)
            {
                Debug.Log("Grip released!");
            }

            previousGripStateL = currentGripStateL;
        }
        else
        {
            // Re-fetch the device if disconnected
            controllerL = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        }
    }
}