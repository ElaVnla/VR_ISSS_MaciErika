using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // XR Origin / VR Player
    [SerializeField] private float height = 15f;
    [SerializeField] private bool rotateWithTarget = true;

    void LateUpdate()
    {
        if (!target) return;

        Vector3 p = target.position;

        // Follow X/Z only, keep camera at fixed height
        transform.position = new Vector3(p.x, height, p.z);

        if (rotateWithTarget)
        {
            transform.rotation = Quaternion.Euler(90f, target.eulerAngles.y, 0f);
        }
        else
        {
            // Always look straight down
            transform.rotation = Quaternion.Euler(90f, -90f, 0f);
        }
    }
}