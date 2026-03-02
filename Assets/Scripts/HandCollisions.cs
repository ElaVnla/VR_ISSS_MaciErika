using UnityEngine;

public class HandCollisions : MonoBehaviour
{
    public bool isTouching = false;

    void OnTriggerEnter(Collider other)
    {
        isTouching = true;
    }

    void OnTriggerExit(Collider other)
    {
        isTouching = false;
    } 
}