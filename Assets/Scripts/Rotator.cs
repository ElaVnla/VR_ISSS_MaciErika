using UnityEngine;

public class Rotator : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        // deltatime ensures smooth rotation regardless of frame rate
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
    }
}
