using UnityEngine;
using UnityEngine.UIElements;
using Combat;

public class HealthUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private Health bossHealth;

    private VisualElement playerBarFill;
    private VisualElement bossBarFill;

    private int playerMax;
    private int bossMax;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        playerBarFill = root.Q<VisualElement>("player-bar-fill");
        bossBarFill = root.Q<VisualElement>("boss-bar-fill");

        playerMax = playerHealthMax();
        bossMax = bossHealthMax();

        playerHealth.OnHealthChanged += UpdatePlayerBar;
        bossHealth.OnHealthChanged += UpdateBossBar;

        // Initialize
        UpdatePlayerBar(playerMax);
        UpdateBossBar(bossMax);
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
}