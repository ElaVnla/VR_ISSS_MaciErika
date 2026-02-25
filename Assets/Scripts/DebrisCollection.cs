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
}