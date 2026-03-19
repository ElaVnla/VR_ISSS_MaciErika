using UnityEngine;

public class HandCollisions : MonoBehaviour
{
    
    public LayerMask wallMask;
    public LayerMask interactableMask;

    
    public bool isTouching = false;

    bool touchingWall = false;
    bool touchingInteractable = false;

    void OnTriggerEnter(Collider other)
    {
        int bit = 1 << other.gameObject.layer;

        if ((wallMask.value & bit) != 0)
            touchingWall = true;

        if ((interactableMask.value & bit) != 0)
            touchingInteractable = true;

        UpdateIsTouching();
    }

    void OnTriggerExit(Collider other)
    {
        int bit = 1 << other.gameObject.layer;

        if ((wallMask.value & bit) != 0)
            touchingWall = false;

        if ((interactableMask.value & bit) != 0)
            touchingInteractable = false;

        UpdateIsTouching();
    }

    void UpdateIsTouching()
    {
       
        if (touchingWall && !touchingInteractable)
            isTouching = true;
        else
            isTouching = false;
    }
}
