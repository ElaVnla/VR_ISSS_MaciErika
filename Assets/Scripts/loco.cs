using UnityEngine;
using UnityEngine.XR;

public class loco : MonoBehaviour
{
    private Vector3 currPosition;
    private Vector3 prevPosition;
    private Vector3 velocity;
    private Vector3 driftVelocity;

    private InputDevice controllerR;
    private InputDevice controllerL;
    private bool previousGripStateR = false;
    private bool previousGripStateL = false;

    private bool isMovingForward = false;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //start the prevPosition at start
        prevPosition = transform.position;

        //controllers
        controllerR = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        controllerL = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
    }

    // Update is called once per frame
    void Update()
    {
        
        //update current position
        currPosition = transform.position;

        //velocity calc
        velocity = (currPosition - prevPosition) / Time.deltaTime;

        Debug.Log(velocity);
        //store prev as current for next frame
        prevPosition = currPosition;
        

        if (isMovingForward)
        {
            transform.position +=  velocity * Time.deltaTime;
        }

        // controllerR grip and release

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
                driftVelocity = velocity;
                isMovingForward = true;
            }

            previousGripStateR = currentGripStateR;
        }
        else
        {
            // Re-fetch the device if disconnected
            controllerR = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        }


        // controllerL grip and release
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
                driftVelocity = velocity;
                isMovingForward = true;



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
