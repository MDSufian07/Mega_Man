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

        [SerializeField] private float panelShowDelay = 1.5f;

        private VisualElement _playerBarFill;
        private VisualElement _bossBarFill;
        private VisualElement _gameOverPanel;
        private VisualElement _winPanel;

        private int _playerMax;
        private int _bossMax;

        void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            _playerBarFill = root.Q<VisualElement>("player-bar-fill");
            _bossBarFill = root.Q<VisualElement>("boss-bar-fill");
            _gameOverPanel = root.Q<VisualElement>("game-over-panel");
            _winPanel = root.Q<VisualElement>("win-panel");

            _playerMax = GetMaxHealth(playerHealth);
            _bossMax = GetMaxHealth(bossHealth);

            UpdatePlayerBar(_playerMax);
            UpdateBossBar(_bossMax);

            SetupButtons();
        }

        void OnEnable()
        {
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged += UpdatePlayerBar;
                playerHealth.OnDeath += ShowGameOver;
            }

            if (bossHealth != null)
            {
                bossHealth.OnHealthChanged += UpdateBossBar;
                bossHealth.OnDeath += ShowWin;
            }
        }

        void OnDestroy()
        {
            UnsubscribeHealthEvents();
        }

        private void UnsubscribeHealthEvents()
        {
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged -= UpdatePlayerBar;
                playerHealth.OnDeath -= ShowGameOver;
            }

            if (bossHealth != null)
            {
                bossHealth.OnHealthChanged -= UpdateBossBar;
                bossHealth.OnDeath -= ShowWin;
            }
        }

        int GetMaxHealth(Health health)
        {
            return health != null ? health.MaxHealth : 0;
        }

        void UpdateHealthBar(VisualElement barFill, int current, int max)
        {
            if (barFill == null || max <= 0)
            {
                return;
            }

            float percent = (float)current / max;
            barFill.style.width = Length.Percent(percent * 100);
        }

        void UpdatePlayerBar(int current)
        {
            UpdateHealthBar(_playerBarFill, current, _playerMax);
        }

        void UpdateBossBar(int current)
        {
            UpdateHealthBar(_bossBarFill, current, _bossMax);
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
            {
                retryButton.clicked += Retry;
            }

            if (exitButton != null)
            {
                exitButton.clicked += Retry;
            }
        }

        void Retry()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}