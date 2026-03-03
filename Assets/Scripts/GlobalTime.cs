using UnityEngine;
using TMPro;
using Unity.XR.CoreUtils;

// REFERENCE
// https://www.youtube.com/watch?v=NdRneaQUkA4
// https://www.youtube.com/watch?v=8qji-GdBwag
public class GlobalTime : MonoBehaviour
{
    [Header("XR Simulator Lock (Editor Testing)")]
    [SerializeField] private Behaviour[] simulatorComponentsToDisable;

    [Header("Timer")]
    [SerializeField] private float startTime = 120f;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("UI")]
    [SerializeField] private GameObject resultsPanel;
    [SerializeField] private GameObject winMsg;
    [SerializeField] private GameObject loseMsg;
    [SerializeField] private GameObject playAgainBtn;

    [Header("Player Reset")]
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Transform cameraOffset;
    [SerializeField] private XROrigin xrOrigin;
    [SerializeField] private CharacterController characterController;

    // ADDED: some Quest rigs break if we disable CharacterController
    [Header("Reset Options")]
    [SerializeField] private bool disableCharacterControllerDuringReset = true;

    [Header("Locomotion Root")]
    [SerializeField] private GameObject locomotionRoot;

    [Header("Run Reset Dependencies")]
    [SerializeField] private DebrisRespawner debrisRespawner;
    [SerializeField] private DebrisCollection debrisCollection;

    [Header("Extra Resettable Scripts")]
    [SerializeField] private MonoBehaviour[] runResettables;

    private float timeRemaining;
    private bool ended;

    private Vector3 startCameraWorldPos;
    private Quaternion startRigRot;
    private bool startCaptured;

    private Vector3 startPos;
    private Quaternion startRot;
    private Vector3 startOffsetPos;
    private Quaternion startOffsetRot;

    private void Start()
    {
        StartCoroutine(CaptureStartPoseNextFrame());

        if (debrisRespawner != null)
            debrisRespawner.Capture();

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

    private void Update()
    {
        if (ended)
            return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining < 0f) timeRemaining = 0f;

        UpdateTimerDisplay();

        if (timeRemaining <= 0f)
            EndGame(isWin: false);
    }

    public void ReachFinish()
    {
        if (ended)
            return;

        EndGame(isWin: true);
    }

    private void EndGame(bool isWin)
    {
        ended = true;

        if (resultsPanel != null) resultsPanel.SetActive(true);
        if (winMsg != null) winMsg.SetActive(isWin);
        if (loseMsg != null) loseMsg.SetActive(!isWin);
        if (playAgainBtn != null) playAgainBtn.SetActive(true);

        SetMovementEnabled(false);
        SetSimulatorEnabled(false);
    }

    public void ResetRun()
    {
        ended = false;

        timeRemaining = startTime;
        UpdateTimerDisplay();

        if (resultsPanel != null)
            resultsPanel.SetActive(false);

        if (debrisRespawner != null)
            debrisRespawner.RespawnAll();

        if (debrisCollection != null)
            debrisCollection.ResetRun();

        ResetExtraRunDependencies();

        SetSimulatorEnabled(true);

        // Temporarily disable locomotion so it does not fight the reset.
        SetMovementEnabled(false);

        StartCoroutine(ResetXRNextFrame());
    }

    private void ResetExtraRunDependencies()
    {
        if (runResettables == null)
            return;

        foreach (var mb in runResettables)
        {
            if (mb == null)
                continue;

            mb.SendMessage("ResetRun", SendMessageOptions.DontRequireReceiver);
        }
    }

    private System.Collections.IEnumerator ResetXRNextFrame()
    {
        // CHANGED: only disable CC if option is enabled
        if (disableCharacterControllerDuringReset && characterController != null)
            characterController.enabled = false;

        yield return null;

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
        ResetExtraRunDependencies();

        // CHANGED: only re-enable CC if we disabled it
        if (disableCharacterControllerDuringReset && characterController != null)
            characterController.enabled = true;

        SetMovementEnabled(true);
        SetSimulatorEnabled(true);
    }

    private void SetMovementEnabled(bool enabled)
    {
        if (locomotionRoot != null)
            locomotionRoot.SetActive(enabled);
    }

    private void SetSimulatorEnabled(bool enabled)
    {
        if (simulatorComponentsToDisable == null)
            return;

        foreach (var c in simulatorComponentsToDisable)
        {
            if (c != null)
                c.enabled = enabled;
        }
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