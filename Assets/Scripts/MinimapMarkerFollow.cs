using UnityEngine;
using Unity.XR.CoreUtils;

public class MinimapMarkerFollow : MonoBehaviour
{
    [SerializeField] private XROrigin xrOrigin;
    [SerializeField] private float floorY = 0.05f;



    private void LateUpdate()
    {
        if (xrOrigin == null || xrOrigin.Camera == null) return;

        Vector3 p = xrOrigin.Camera.transform.position;
        transform.position = new Vector3(p.x, floorY, p.z);
    }
}