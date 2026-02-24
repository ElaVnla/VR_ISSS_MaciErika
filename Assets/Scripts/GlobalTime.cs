using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

// REFERENCE
// https://www.youtube.com/watch?v=NdRneaQUkA4
// https://www.youtube.com/watch?v=8qji-GdBwag
public class GlobalTime : MonoBehaviour
{
    [Header("Timer")]
    public float startTime = 120f;              // 2 minutes
    public TextMeshProUGUI timerText;

    [Header("End Screen UI")]
    public GameObject resultsPanel;             // your Results object
    public GameObject winMsg;                   // WinMsg object
    public GameObject loseMsg;                  // LoseMsg object
    public GameObject playAgainBtn;             // PlayAgainBtn object (optional)

    [Header("TEMP Placeholder (replace later)")]
    public bool tasksCompleted = false;         // toggle in Inspector for now

    private float timeRemaining;
    private bool ended = false;

    void Start()
    {
        timeRemaining = startTime;
        UpdateTimerDisplay();
        if (resultsPanel != null) resultsPanel.SetActive(false);
    }

    void Update()
    {
        if (ended) return;

        if (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining < 0f) timeRemaining = 0f;
            UpdateTimerDisplay();

            if (timeRemaining <= 0f)
            {
                OnTimeUp();
            }
        }
    }

    void OnTimeUp()
    {
        ended = true;

        // Show results UI
        if (resultsPanel != null) resultsPanel.SetActive(true);

        // If tasks completed -> WIN, else -> LOSE
        bool isWin = tasksCompleted;

        if (winMsg != null) winMsg.SetActive(isWin);
        if (loseMsg != null) loseMsg.SetActive(!isWin);

        if (playAgainBtn != null) playAgainBtn.SetActive(true);

        // Optional: freeze gameplay (simple version)
        Time.timeScale = 0f;
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    // Optional helper for testing without tasks system:
    public void SetTasksCompletedTrue()
    {
        tasksCompleted = true;
    }

    public void PlayAgain()
    {
        Debug.Log("PlayAgain clicked");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}