using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DockingProcedureManager : MonoBehaviour
{
    [Header("HUD Subtask Lines (Top-Left)")]
    [SerializeField] private TextMeshProUGUI verifySealLine;
    [SerializeField] private TextMeshProUGUI pressurizeLine;
    [SerializeField] private TextMeshProUGUI autoDockLine;

    [Header("HUD Colors")]
    [SerializeField] private Color pendingColor = Color.white;
    [SerializeField] private Color completedColor = Color.green;

    [Header("Verify Seal Panel")]
    [SerializeField] private Button verifyButton;
    [SerializeField] private Image verifyFill;
    [SerializeField] private TextMeshProUGUI sealSensorText;
    [SerializeField] private TextMeshProUGUI leakRateText;

    [Header("Pressurize Panel")]
    [SerializeField] private Button pressurizeButton;
    [SerializeField] private Image pressurizeFill;
    [SerializeField] private TextMeshProUGUI equalizationText;
    [SerializeField] private TextMeshProUGUI hatchText;
    [SerializeField] private TextMeshProUGUI pressureText;

    [Header("Auto Dock Panel")]
    [SerializeField] private Button autoDockButton;
    [SerializeField] private Image autoDockFill;
    [SerializeField] private TextMeshProUGUI autoDockStatusText;
    [SerializeField] private TextMeshProUGUI captureSystemText;

    [Header("Timing")]
    [SerializeField] private float stepDurationSeconds = 3f;

    // ADDED: sequence lock settings
    [Header("Sequence Lock")]
    [SerializeField] private bool enforceSequence = true;

    // ADDED: completion flags
    private bool autoDockDone;
    private bool verifySealDone;
    private bool pressurizeDone;

    // Default (start) text snapshots
    private string verifySealLineDefault, pressurizeLineDefault, autoDockLineDefault;
    private string sealSensorDefault, leakRateDefault;

    private string equalizationDefault, hatchDefault, pressureDefault;
    private string autoDockStatusDefault, captureSystemDefault;

    private void Awake()
    {
        // HUD defaults
        if (verifySealLine != null) verifySealLineDefault = verifySealLine.text;
        if (pressurizeLine != null) pressurizeLineDefault = pressurizeLine.text;
        if (autoDockLine != null) autoDockLineDefault = autoDockLine.text;

        // Panel defaults
        if (sealSensorText != null) sealSensorDefault = sealSensorText.text;
        if (leakRateText != null) leakRateDefault = leakRateText.text;

        if (equalizationText != null) equalizationDefault = equalizationText.text;
        if (hatchText != null) hatchDefault = hatchText.text;
        if (pressureText != null) pressureDefault = pressureText.text;

        if (autoDockStatusText != null) autoDockStatusDefault = autoDockStatusText.text;
        if (captureSystemText != null) captureSystemDefault = captureSystemText.text;

        ResetRun(); // start in clean state
    }

    // Hook these three methods to each panel button OnClick
    public void StartVerifySeal() => StartCoroutine(RunVerifySeal());
    public void StartPressurize() => StartCoroutine(RunPressurize());
    public void StartAutoDock() => StartCoroutine(RunAutoDock());

    private IEnumerator RunVerifySeal()
    {
        // ADDED: guard if sequence is enforced
        if (enforceSequence && !autoDockDone)
            yield break;

        if (verifyButton != null) verifyButton.interactable = false;
        yield return FillOverTime(verifyFill, stepDurationSeconds);

        if (sealSensorText != null) sealSensorText.text = "Seal Sensor: Check complete";
        if (leakRateText != null) leakRateText.text = "Leak Rate: No leakage";

        MarkHudComplete(verifySealLine);

        // ADDED: mark complete + unlock next
        verifySealDone = true;
        UpdateButtonLocks();
    }

    private IEnumerator RunPressurize()
    {
        // ADDED: guard if sequence is enforced
        if (enforceSequence && !verifySealDone)
            yield break;

        if (pressurizeButton != null) pressurizeButton.interactable = false;
        yield return FillOverTime(pressurizeFill, stepDurationSeconds);

        if (equalizationText != null) equalizationText.text = "Equalization: Complete";
        if (hatchText != null) hatchText.text = "Hatch: CLOSED";
        if (pressureText != null) pressureText.text = "Pressure: 101 kPa";

        MarkHudComplete(pressurizeLine);

        // ADDED: mark complete + lock all
        pressurizeDone = true;
        UpdateButtonLocks();
    }

    private IEnumerator RunAutoDock()
    {
        if (autoDockButton != null) autoDockButton.interactable = false;
        yield return FillOverTime(autoDockFill, stepDurationSeconds);

        if (autoDockStatusText != null) autoDockStatusText.text = "Auto Dock: Complete";
        if (captureSystemText != null) captureSystemText.text = "Capture System: Engaged";

        MarkHudComplete(autoDockLine);

        // ADDED: mark complete + unlock next
        autoDockDone = true;
        UpdateButtonLocks();
    }

    private IEnumerator FillOverTime(Image fillImage, float duration)
    {
        if (fillImage == null) yield break;

        fillImage.fillAmount = 0f;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            fillImage.fillAmount = Mathf.Clamp01(t / duration);
            yield return null;
        }

        fillImage.fillAmount = 1f;
    }

    private void MarkHudComplete(TextMeshProUGUI line)
    {
        if (line == null) return;
        line.color = completedColor;
    }

    private void MarkHudPending(TextMeshProUGUI line)
    {
        if (line == null) return;
        line.color = pendingColor;
    }

    // ADDED: lock/unlock buttons based on sequence
    private void UpdateButtonLocks()
    {
        if (!enforceSequence)
            return;

        if (autoDockButton != null)
            autoDockButton.interactable = !autoDockDone;

        if (verifyButton != null)
            verifyButton.interactable = autoDockDone && !verifySealDone;

        if (pressurizeButton != null)
            pressurizeButton.interactable = verifySealDone && !pressurizeDone;
    }

    // IMPORTANT: GlobalTime calls this using SendMessage("ResetRun")
    public void ResetRun()
    {
        StopAllCoroutines();

        // ADDED: reset completion flags
        autoDockDone = false;
        verifySealDone = false;
        pressurizeDone = false;

        // Reset HUD lines (text + color)
        if (verifySealLine != null) verifySealLine.text = verifySealLineDefault;
        if (pressurizeLine != null) pressurizeLine.text = pressurizeLineDefault;
        if (autoDockLine != null) autoDockLine.text = autoDockLineDefault;

        MarkHudPending(verifySealLine);
        MarkHudPending(pressurizeLine);
        MarkHudPending(autoDockLine);

        // Reset panels: fills
        if (verifyFill != null) verifyFill.fillAmount = 0f;
        if (pressurizeFill != null) pressurizeFill.fillAmount = 0f;
        if (autoDockFill != null) autoDockFill.fillAmount = 0f;

        // Reset panels: texts
        if (sealSensorText != null) sealSensorText.text = sealSensorDefault;
        if (leakRateText != null) leakRateText.text = leakRateDefault;

        if (equalizationText != null) equalizationText.text = equalizationDefault;
        if (hatchText != null) hatchText.text = hatchDefault;
        if (pressureText != null) pressureText.text = pressureDefault;

        if (autoDockStatusText != null) autoDockStatusText.text = autoDockStatusDefault;
        if (captureSystemText != null) captureSystemText.text = captureSystemDefault;

        // CHANGED: do NOT set all buttons true here; let sequence decide
        UpdateButtonLocks();
    }

    public bool AreAllSubtasksComplete()
    {
        return autoDockDone && verifySealDone && pressurizeDone;
    }
}