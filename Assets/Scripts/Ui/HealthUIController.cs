using Combat;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Ui
{
    public class HealthUIController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Health playerHealth;
        [SerializeField] private Health bossHealth;

        private VisualElement _playerBarFill;
        private VisualElement _bossBarFill;
        private VisualElement _gameOverPanel;
        private VisualElement _winPanel;
    
        [SerializeField] private float panelShowDelay = 1.5f;

        private int _playerMax;
        private int _bossMax;

        void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            _playerBarFill = root.Q<VisualElement>("player-bar-fill");
            _bossBarFill = root.Q<VisualElement>("boss-bar-fill");
            _gameOverPanel = root.Q<VisualElement>("game-over-panel");
            _winPanel = root.Q<VisualElement>("win-panel");

            _playerMax = PlayerHealthMax();
            _bossMax = BossHealthMax();

            playerHealth.OnHealthChanged += UpdatePlayerBar;
            playerHealth.OnDeath += ShowGameOver;

            bossHealth.OnHealthChanged += UpdateBossBar;
            bossHealth.OnDeath += ShowWin;

            UpdatePlayerBar(_playerMax);
            UpdateBossBar(_bossMax);

            SetupButtons();
        }

        private int PlayerHealthMax() => GetMaxHealth(playerHealth);
        private int BossHealthMax() => GetMaxHealth(bossHealth);

        int GetMaxHealth(Health h)
        {
            return typeof(Health)
                .GetField("maxHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(h) as int? ?? 100;
        }

        void UpdatePlayerBar(int current)
        {
            float percent = (float)current / _playerMax;
            _playerBarFill.style.width = Length.Percent(percent * 100);
        }

        void UpdateBossBar(int current)
        {
            float percent = (float)current / _bossMax;
            _bossBarFill.style.width = Length.Percent(percent * 100);
        }

        void ShowGameOver()
        {
            Invoke(nameof(DisplayGameOverPanel), panelShowDelay);
        }

        void ShowWin()
        {
            Invoke(nameof(DisplayWinPanel), panelShowDelay);
        }

        void DisplayGameOverPanel()
        {
            _gameOverPanel.style.display = DisplayStyle.Flex;
            Time.timeScale = 0f;
        }

        void DisplayWinPanel()
        {
            _winPanel.style.display = DisplayStyle.Flex;
            Time.timeScale = 0f;
        }

        void SetupButtons()
        {
            Button retryButton = _gameOverPanel.Q<Button>("retry-button");
            Button exitButton = _winPanel.Q<Button>("exit-button");

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
}

