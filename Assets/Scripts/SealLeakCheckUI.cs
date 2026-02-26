using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SealLeakCheckUI : MonoBehaviour
{
    [SerializeField] private Button runLeakCheckButton;
    [SerializeField] private Image progressFillImage;
    [SerializeField] private TextMeshProUGUI sealSensorText;
    [SerializeField] private TextMeshProUGUI leakRateText;

    [SerializeField] private TextMeshProUGUI subTaskTextToTurnGreen;

    private bool hasRun;

    private const string SealStart = "Seal Sensor: Awaiting Check";
    private const string LeakStart = "Leak Rate: Unknown";

    private void Start()
    {
        ResetRun();
    }

    public void ResetRun()
    {
        hasRun = false;

        if (progressFillImage != null)
            progressFillImage.fillAmount = 0f;

        if (sealSensorText != null)
            sealSensorText.text = SealStart;

        if (leakRateText != null)
            leakRateText.text = LeakStart;

        if (runLeakCheckButton != null)
            runLeakCheckButton.interactable = true;

        if (subTaskTextToTurnGreen != null)
            subTaskTextToTurnGreen.color = Color.white; // or your default color
    }

    public void RunLeakCheck()
    {
        if (hasRun) return;
        hasRun = true;

        runLeakCheckButton.interactable = false;
        StartCoroutine(LeakCheckRoutine());
    }

    private System.Collections.IEnumerator LeakCheckRoutine()
    {
        float duration = 3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            if (progressFillImage != null)
                progressFillImage.fillAmount = t;

            yield return null;
        }

        if (sealSensorText != null)
            sealSensorText.text = "Seal Sensors: Check complete";

        if (leakRateText != null)
            leakRateText.text = "Leak Rate: No leakage";

        // stays disabled
        runLeakCheckButton.interactable = false;

        if (subTaskTextToTurnGreen != null)
            subTaskTextToTurnGreen.color = Color.green;
    }
}