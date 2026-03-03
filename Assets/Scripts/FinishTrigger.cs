using System.Collections;
using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    [Header("References")]
    public GlobalTime game;
    public DockingProcedureManager dockingManager;

    [Header("Blocked UI Popup")]
    [SerializeField] private GameObject incompleteTaskPopup;
    [SerializeField] private float popupSeconds = 5f;

    private Coroutine popupRoutine;

    private void Start()
    {
        if (incompleteTaskPopup != null)
            incompleteTaskPopup.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered by: " + other.name);

        // works even if collider is on a child object
        if (!(other.CompareTag("Player") || other.transform.root.CompareTag("Player")))
            return;

        // BLOCK main task if subtasks not complete
        if (dockingManager != null && !dockingManager.AreAllSubtasksComplete())
        {
            ShowPopup();
            return;
        }

        // allow finish
        if (game != null)
            game.ReachFinish();
    }

    private void ShowPopup()
    {
        if (incompleteTaskPopup == null)
            return;

        if (popupRoutine != null)
            StopCoroutine(popupRoutine);

        popupRoutine = StartCoroutine(PopupRoutine());
    }

    private IEnumerator PopupRoutine()
    {
        incompleteTaskPopup.SetActive(true);
        yield return new WaitForSeconds(popupSeconds);
        incompleteTaskPopup.SetActive(false);
        popupRoutine = null;
    }

    // OPTIONAL: lets GlobalTime reset this too (via SendMessage)
    public void ResetRun()
    {
        if (popupRoutine != null)
            StopCoroutine(popupRoutine);

        popupRoutine = null;

        if (incompleteTaskPopup != null)
            incompleteTaskPopup.SetActive(false);
    }
}