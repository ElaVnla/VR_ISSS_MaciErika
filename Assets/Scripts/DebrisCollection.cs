using UnityEngine;
using UnityEngine.InputSystem;

public class DebrisCollection : MonoBehaviour
{
    public InputActionReference gripAction; // drag your Grip action here in Inspector

    private int debris = 0;
    private GameObject debrisInRange;

    private void OnEnable() => gripAction.action.Enable();
    private void OnDisable() => gripAction.action.Disable();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Debris"))
            debrisInRange = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (debrisInRange == other.gameObject)
            debrisInRange = null;
    }

    private void Update()
    {
        if (debrisInRange != null && gripAction.action.WasPressedThisFrame())
        {
            debris++;
            Debug.Log("Debris: " + debris);
            Destroy(debrisInRange);
            debrisInRange = null;
        }
    }
}