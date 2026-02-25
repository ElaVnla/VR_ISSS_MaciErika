using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    public GlobalTime game;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered by: " + other.name);
        // works even if collider is on a child object
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            game.ReachFinish();
        }
    }
}