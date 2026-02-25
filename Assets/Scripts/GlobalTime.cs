using UnityEngine;
using TMPro;
using Unity.XR.CoreUtils;

// REFERENCE
// https://www.youtube.com/watch?v=NdRneaQUkA4
// https://www.youtube.com/watch?v=8qji-GdBwag
public class GlobalTime : MonoBehaviour
{
    [Header("XR Simulator Lock (Editor Testing)")]
    public GameObject xrInteractionSimulatorObject; // keep assigned if you want, but we won't SetActive() it
    public Behaviour[] simulatorComponentsToDisable; // drag XR Device Simulator component(s) here

    [Header("Timer")]
    public float startTime = 120f;
    public TextMeshProUGUI timerText;

    [Header("UI")]
    public GameObject resultsPanel;   // Results parent
    public GameObject winMsg;         // WinMsg object
    public GameObject loseMsg;        // LoseMsg object
    public GameObject playAgainBtn;   // PlayAgainBtn object

    [Header("Player Reset")]
    public Transform playerRoot;      // VR Player / XR Origin transform
    public Transform cameraOffset;
    public Behaviour[] movementToDisable; // movement scripts to disable (move/teleport) (optional)
    public XROrigin xrOrigin;          // drag VR Player here (the object with XROrigin)
    public CharacterController characterController; // optional: drag if VR Player has one

    private Vector3 startCameraWorldPos;
    private Quaternion startRigRot;
    private bool startCaptured = false;

    [Header("TEMP Placeholder")]
    public bool tasksCompleted = false; // later replace with real tasks system

    [Header("Locomotion Root (RECOMMENDED)")]
    public GameObject locomotionRoot;   // drag VR Player > Locomotion here

    private float timeRemaining;
    private bool ended = false;

    private Vector3 startPos;
    private Quaternion startRot;
    private Vector3 startOffsetPos;
    private Quaternion startOffsetRot;

    void Start()
    {
        StartCoroutine(CaptureStartPoseNextFrame());

        timeRemaining = startTime;
        UpdateTimerDisplay();

        if (resultsPanel != null)
            resultsPanel.SetActive(false);

        if (playerRoot != null)
        {
            startPos = playerRoot.position;
            startRot = playerRoot.rotation;
        }

        if (cameraOffset != null)
        {
            startOffsetPos = cameraOffset.localPosition;
            startOffsetRot = cameraOffset.localRotation;
        }
    }

    void Update()
    {
        if (ended) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining < 0f) timeRemaining = 0f;

        UpdateTimerDisplay();

        if (timeRemaining <= 0f)
        {
            EndGame(false);
        }
    }

    // Called by FinishTrigger when player touches cylinder
    public void ReachFinish()
    {
        if (ended) return;

        tasksCompleted = true;
        EndGame(true);
    }

    private void EndGame(bool isWin)
    {
        ended = true;

        if (resultsPanel != null) resultsPanel.SetActive(true);
        if (winMsg != null) winMsg.SetActive(isWin);
        if (loseMsg != null) loseMsg.SetActive(!isWin);
        if (playAgainBtn != null) playAgainBtn.SetActive(true);

        SetMovementEnabled(false);

        // IMPORTANT: Don't disable the entire simulator object,
        // only disable the simulator movement component(s) you dragged in.
        SetSimulatorEnabled(false);
    }

    public void ResetRun()
    {
        ended = false;
        tasksCompleted = false;

        timeRemaining = startTime;
        UpdateTimerDisplay();

        if (resultsPanel != null)
            resultsPanel.SetActive(false);

        // Re-enable simulator immediately so editor controls come back
        SetSimulatorEnabled(true);

        // stop locomotion fighting the reset
        SetMovementEnabled(false);

        StartCoroutine(ResetXRNextFrame());
    }

    private System.Collections.IEnumerator ResetXRNextFrame()
    {
        if (characterController != null) characterController.enabled = false;

        yield return null; // wait 1 frame

        if (cameraOffset != null)
        {
            cameraOffset.localPosition = startOffsetPos;
            cameraOffset.localRotation = startOffsetRot;
        }

        if (xrOrigin != null && startCaptured)
        {
            xrOrigin.transform.rotation = startRigRot;
            xrOrigin.MoveCameraToWorldLocation(startCameraWorldPos);
        }
        else if (playerRoot != null)
        {
            playerRoot.SetPositionAndRotation(startPos, startRot);
        }

        yield return null;

        if (characterController != null) characterController.enabled = true;

        // Make sure both locomotion + simulator are on after resetting
        SetMovementEnabled(true);
        SetSimulatorEnabled(true);
    }

    private void SetMovementEnabled(bool enabled)
    {
        if (locomotionRoot != null)
            locomotionRoot.SetActive(enabled);

        // optional per-script disabling (you can leave list empty)
        if (movementToDisable == null) return;
        foreach (var b in movementToDisable)
            if (b != null) b.enabled = enabled;
    }

    private void SetSimulatorEnabled(bool enabled)
    {
        // DO NOT SetActive() the entire XR Interaction Simulator object here
        // because it can kill input completely and not recover properly.
        // Instead, toggle only the simulator components you drag in.
        if (simulatorComponentsToDisable == null) return;

        foreach (var c in simulatorComponentsToDisable)
            if (c != null) c.enabled = enabled;
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);

        if (timerText != null)
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private System.Collections.IEnumerator CaptureStartPoseNextFrame()
    {
        yield return null;

        if (xrOrigin != null && xrOrigin.Camera != null)
        {
            startCameraWorldPos = xrOrigin.Camera.transform.position;
            startRigRot = xrOrigin.transform.rotation;
            startCaptured = true;
        }
    }
}