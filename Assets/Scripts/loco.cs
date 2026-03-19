using UnityEngine;
using UnityEngine.XR;

public class loco : MonoBehaviour
{
    private Vector3 currPosition;
    private Vector3 prevPosition;
    private Vector3 velocity;
    

    private InputDevice controllerR;
    private InputDevice controllerL;
    private bool previousGripStateR = false;
    private bool previousGripStateL = false;
    private bool isMovingForward = false;

    

    public GameObject grabMove;
    public HandCollisions RhandScript;
    public HandCollisions LhandScript;


    void Start()
    {
        grabMove.SetActive(false);
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
        
        //store prev as current for next frame
        prevPosition = currPosition;
        

        if (isMovingForward)
        {
            //rigidbody velocity application
            Rigidbody rb = GetComponent<Rigidbody>();

            rb.linearVelocity = velocity;
        }

        
        //RIGHT controller
        if (controllerR.TryGetFeatureValue(CommonUsages.gripButton, out bool currentGripStateR))
        {
            //grip is pressed
            if (currentGripStateR && !previousGripStateR && RhandScript.isTouching)
            {
                //activates grab-move locomotion
                grabMove.SetActive(true);

            }

            // grip is released
            if (!currentGripStateR && previousGripStateR)
            {
                grabMove.SetActive(false);
                isMovingForward = true;
            }

            previousGripStateR = currentGripStateR;
        }
        else
        {
            // Re-fetch the device if disconnected
            controllerR = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        }


        // LEFT controller
        if (controllerL.TryGetFeatureValue(CommonUsages.gripButton, out bool currentGripStateL))
        {
            //grip is pressed
            if (currentGripStateL && !previousGripStateL && LhandScript.isTouching)
            {
                //activates grab-move locomotion
                grabMove.SetActive(true);

            }
            // grip is released
            if (!currentGripStateL && previousGripStateL)
            {
                grabMove.SetActive(false);
                isMovingForward = true;
   
            }
            
            previousGripStateL = currentGripStateL;
        }
        else
        {
            // Reconnect the device if disconnected
            controllerL = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        }
    }
}
