using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Combat;

public class HealthUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private Health bossHealth;

    private VisualElement playerBarFill;
    private VisualElement bossBarFill;
    private VisualElement gameOverPanel;
    private VisualElement winPanel;

    private int playerMax;
    private int bossMax;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        playerBarFill = root.Q<VisualElement>("player-bar-fill");
        bossBarFill = root.Q<VisualElement>("boss-bar-fill");
        gameOverPanel = root.Q<VisualElement>("game-over-panel");
        winPanel = root.Q<VisualElement>("win-panel");

        playerMax = playerHealthMax();
        bossMax = bossHealthMax();

        playerHealth.OnHealthChanged += UpdatePlayerBar;
        playerHealth.OnDeath += ShowGameOver;
        
        bossHealth.OnHealthChanged += UpdateBossBar;
        bossHealth.OnDeath += ShowWin;

        UpdatePlayerBar(playerMax);
        UpdateBossBar(bossMax);

        SetupButtons();
    }

    int playerHealthMax() => GetMaxHealth(playerHealth);
    int bossHealthMax() => GetMaxHealth(bossHealth);

    int GetMaxHealth(Health h)
    {
        return typeof(Health)
            .GetField("maxHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(h) as int? ?? 100;
    }

    void UpdatePlayerBar(int current)
    {
        float percent = (float)current / playerMax;
        playerBarFill.style.width = Length.Percent(percent * 100);
    }

    void UpdateBossBar(int current)
    {
        float percent = (float)current / bossMax;
        bossBarFill.style.width = Length.Percent(percent * 100);
    }

    void ShowGameOver()
    {
        Invoke(nameof(DisplayGameOverPanel), 1f);
    }

    void ShowWin()
    {
        Invoke(nameof(DisplayWinPanel), 1f);
    }

    void DisplayGameOverPanel()
    {
        gameOverPanel.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f;
    }

    void DisplayWinPanel()
    {
        winPanel.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f;
    }

    void SetupButtons()
    {
        Button retryButton = gameOverPanel.Q<Button>("retry-button");
        Button exitButton = winPanel.Q<Button>("exit-button");

        if (retryButton != null)
            retryButton.clicked += Retry;

        if (exitButton != null)
            exitButton.clicked += Retry;
    }

    void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}