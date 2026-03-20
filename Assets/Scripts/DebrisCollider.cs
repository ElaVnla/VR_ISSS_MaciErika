using UnityEngine;

public class DebrisCollider : MonoBehaviour
{
    public GameObject loseUI;

    private bool hasTriggered = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (hasTriggered) return;

        if (collision.gameObject.CompareTag("GrabWalls"))
        {
            hasTriggered = true;

            Debug.Log("CUBE HIT WALL!");

            TriggerLose();
        }
    }

    private void TriggerLose()
    {
        if (loseUI != null)
        {
            loseUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
