using System.Collections;
using UnityEngine;
using UnityEngine.XR;
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
    [SerializeField] private CharacterController characterController;  // drag your CC here
    [SerializeField] private float pushDistance = 5.0f;
    [SerializeField] private float pushDuration = 1.5f;

    // temporarily disable your move provider while pushing
    [SerializeField] private Behaviour[] movementProvidersToDisable;

    private int debrisCount;
    private GameObject debrisInRange;

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
        if (debrisInRange == null || gripAction == null)
            return;

        if (gripAction.action.WasPressedThisFrame())
        {
            debrisCount++;

            // Disable instead of Destroy so DebrisRespawner can re-enable it later.
            debrisInRange.SetActive(false);
            debrisInRange = null;

            UpdateUI();

            StartCoroutine(PushPlayerBack());
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

        // Use the XR camera forward (most reliable direction in XR)
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("PushPlayerBack: Camera.main not found. Tag your XR Camera as MainCamera.");
            yield break;
        }

        Vector3 backward = -cam.transform.forward;
        backward.y = 0f;

        if (backward.sqrMagnitude < 0.0001f)
        {
            Debug.LogWarning("PushPlayerBack: backward direction is zero.");
            yield break;
        }

        backward.Normalize();

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
                // Fallback if CC isn't found
                playerRoot.position += delta;
            }

            yield return null;
        }

        Debug.Log($"PushPlayerBack done. Intended distance: {pushDistance}, moved: {moved}");
    }
}