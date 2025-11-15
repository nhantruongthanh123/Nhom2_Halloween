using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI streakText;
    public GameObject gameOverPanel;
    public GameObject pauseButton;
    
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
        if (streakText != null) streakText.gameObject.SetActive(false);
        StartCoroutine(WaitAndStopTime());
        
    }
    private System.Collections.IEnumerator WaitAndStopTime()
    {
        yield return new WaitForSecondsRealtime(1.2f);
        Time.timeScale = 0;
        pauseButton.SetActive(false);
        Ghost[] allGhosts = FindObjectsByType<Ghost>(FindObjectsSortMode.None);
        foreach (Ghost ghost in allGhosts)
        {
            ghost.gameObject.SetActive(false); 
        }
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("menuScene");
    }


    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "playScene") 
        {
            FindUIElements();
            ResetGame();
        }
    }

    void FindUIElements()
    {
        GameObject scoreTextGO = GameObject.Find("Score");
        if (scoreTextGO != null)
        {
            scoreText = scoreTextGO.GetComponent<TextMeshProUGUI>();
        }

        GameObject streakTextGO = GameObject.Find("Streak");
        if (streakTextGO != null)
        {
            streakText = streakTextGO.GetComponent<TextMeshProUGUI>();
        }

        if (scoreText == null) Debug.LogError("GameManager: Không tìm thấy 'Score' Text!");
        if (streakText == null) Debug.LogError("GameManager: Không tìm thấy 'Streak' Text!");
    }

    void ResetGame()
    {
        score = 0;
        currentStreak = 0;
        currentStreakTimer = 0;
        UpdateScoreDisplay();
        UpdateStreakDisplay(false);
        Time.timeScale = 1f;
    }

}
