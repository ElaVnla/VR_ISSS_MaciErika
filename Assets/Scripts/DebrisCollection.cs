using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class DebrisCollection : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference gripAction;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI taskText;

    [Header("Progress")]
    [SerializeField] private int targetCount = 9;

    [Header("Colors")]
    [SerializeField] private Color notStartedColor = Color.white;
    [SerializeField] private Color inProgressColor = new Color(0.75f, 0.6f, 0.25f);
    [SerializeField] private Color completeColor = Color.green;

    [Header("Push Back Effect")]
    [SerializeField] private Transform playerRoot;
    [SerializeField] private CharacterController characterController;  // drag your CC here if you use one
    [SerializeField] private Transform cameraTransform;               // drag XR Origin -> Camera Offset -> Main Camera (recommended)
    [SerializeField] private float pushDistance = 5.0f;
    [SerializeField] private float pushDuration = 1.5f;

    // temporarily disable your move provider while pushing
    [SerializeField] private Behaviour[] movementProvidersToDisable;

    private int debrisCount;
    private GameObject debrisInRange;
    private Coroutine pushRoutine;

    private void OnEnable()
    {
        if (gripAction != null)
            gripAction.action.Enable();
    }

    private void OnDisable()
    {
        if (gripAction != null)
            gripAction.action.Disable();
    }

    private void Start()
    {
        UpdateUI();
    }

    // Works if THIS script is on the same object that has the trigger collider.
    private void OnTriggerEnter(Collider other) => OnDebrisTriggerEnter(other);
    private void OnTriggerExit(Collider other) => OnDebrisTriggerExit(other);

    // NEW: Works if you put the trigger collider somewhere else (e.g., Right Controller)
    // and forward the trigger events via DebrisTriggerProxy.
    public void OnDebrisTriggerEnter(Collider other)
    {
        if (other != null && other.CompareTag("Debris"))
            debrisInRange = other.gameObject;
    }

    public void OnDebrisTriggerExit(Collider other)
    {
        if (other != null && debrisInRange == other.gameObject)
            debrisInRange = null;
    }

    private void Update()
    {
        if (debrisInRange == null || gripAction == null)
            return;

        if (gripAction.action.WasPressedThisFrame())
        {
            debrisCount++;

            // Disable instead of Destroy so DebrisRespawner can re-enable it later.
            debrisInRange.SetActive(false);
            debrisInRange = null;

            UpdateUI();

            // Restart push if player spam-collects quickly
            if (pushRoutine != null)
                StopCoroutine(pushRoutine);

            pushRoutine = StartCoroutine(PushPlayerBack());
        }
    }

    private void UpdateUI()
    {
        if (taskText == null)
            return;

        taskText.text = $"1. Collect {debrisCount} / {targetCount} debris";

        if (debrisCount <= 0)
            taskText.color = notStartedColor;
        else if (debrisCount < targetCount)
            taskText.color = inProgressColor;
        else
            taskText.color = completeColor;
    }

    // Called by GlobalTime when "Play Again" is pressed.
    public void ResetRun()
    {
        debrisCount = 0;
        debrisInRange = null;

        if (pushRoutine != null)
        {
            StopCoroutine(pushRoutine);
            pushRoutine = null;
        }

        // Ensure locomotion is not left disabled
        SetMovementProvidersEnabled(true);

        UpdateUI();
    }

    private IEnumerator PushPlayerBack()
    {
        if (playerRoot == null)
        {
            Debug.LogWarning("PushPlayerBack: playerRoot not assigned.");
            yield break;
        }

        // Try to find CC automatically if not assigned
        if (characterController == null)
            characterController = playerRoot.GetComponent<CharacterController>();

        // Choose camera transform
        Transform camT = cameraTransform;
        if (camT == null)
        {
            Camera cam = Camera.main;
            if (cam != null) camT = cam.transform;
        }

        if (camT == null)
        {
            Debug.LogWarning("PushPlayerBack: No cameraTransform and Camera.main not found. Assign cameraTransform in Inspector.");
            yield break;
        }

        Vector3 backward = -camT.forward;
        backward.y = 0f;

        if (backward.sqrMagnitude < 0.0001f)
        {
            Debug.LogWarning("PushPlayerBack: backward direction is zero.");
            yield break;
        }

        backward.Normalize();

        // IMPORTANT: disable movement providers so push actually takes effect smoothly
        SetMovementProvidersEnabled(false);

        float moved = 0f;
        float t = 0f;

        while (t < pushDuration)
        {
            t += Time.deltaTime;

            float progress = Mathf.Clamp01(t / pushDuration);
            float smooth = 1f - Mathf.Pow(1f - progress, 3f); // ease-out

            float targetMoved = pushDistance * smooth;
            float step = targetMoved - moved;
            moved = targetMoved;

            Vector3 delta = backward * step;

            if (characterController != null)
            {
                characterController.Move(delta);
            }
            else
            {
                playerRoot.position += delta;
            }

            yield return null;
        }

        SetMovementProvidersEnabled(true);
        pushRoutine = null;

        Debug.Log($"PushPlayerBack done. Intended distance: {pushDistance}, moved: {moved}");
    }

    private void SetMovementProvidersEnabled(bool enabled)
    {
        if (movementProvidersToDisable == null)
            return;

        foreach (var p in movementProvidersToDisable)
        {
            if (p != null)
                p.enabled = enabled;
        }
    }
}

// OPTIONAL helper (same file): put this on the object that has the trigger collider (e.g., Right Controller)
// and drag your DebrisCollection into the field.
public class DebrisTriggerProxy : MonoBehaviour
{
    [SerializeField] private DebrisCollection debrisCollection;

    private void OnTriggerEnter(Collider other)
    {
        if (debrisCollection != null)
            debrisCollection.OnDebrisTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (debrisCollection != null)
            debrisCollection.OnDebrisTriggerExit(other);
    }
}