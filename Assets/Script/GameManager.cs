using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI streakText;
    
    // Point when killing a ghost
    public int maxPoint = 50;
    public int minPoint = 10;
    public float decreasePoint = 8f;

    // Streak point
    public int streakMultiplier = 2;  
    public int streakTimeLimit = 2; 

    private int score = 0;
    private int currentStreak = 0;
    private float currentStreakTimer = 2;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        score = 0;
        currentStreak = 0;
        currentStreakTimer = 0;
        UpdateScoreDisplay();
        UpdateStreakDisplay(false);
    }

    void Update()
    {
        if (currentStreak > 0)
        {
            currentStreakTimer -= Time.deltaTime;
            if (currentStreakTimer <= 0)
            {
                ResetStreak();
            }
        }
    }

    public void ReportKill(float timeAlive)
    {
        int baseScore = Mathf.Max(minPoint, Mathf.RoundToInt(maxPoint - (decreasePoint * timeAlive)));

        currentStreak++;
        currentStreakTimer = streakTimeLimit;

        int finalScore = baseScore;
        if (currentStreak > 1)
        {
            finalScore *= streakMultiplier;
        }

        score += finalScore;
        UpdateScoreDisplay();
        if (currentStreak > 1) UpdateStreakDisplay(true);
    }
    
    private void ResetStreak()
    {
        currentStreak = 0;
        currentStreakTimer = streakTimeLimit;
        UpdateStreakDisplay(false); 
    }

    public void AddScore(int pointsToAdd)
    {
        score += pointsToAdd;
        UpdateScoreDisplay();
    }

    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    void UpdateStreakDisplay(bool isActive)
    {
        if (streakText != null)
        {
            if (isActive)
            {
                streakText.text = "Streak: " + currentStreak;
                streakText.gameObject.SetActive(true);
            }
            else
            {
                streakText.gameObject.SetActive(false);
            }
        }
    }


    public void GameOver()
    {
        StartCoroutine(WaitAndStopTime());
    }
    private System.Collections.IEnumerator WaitAndStopTime()
    {
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 0;
    }

}
