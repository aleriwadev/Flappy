using System.Collections;
using UnityEngine;
using TMPro;

public class LevelDisplayUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI levelUpNotification;
    public GameObject levelUpPanel;

    [Header("Animation Settings")]
    public float notificationDuration = 2f;
    public float fadeInTime = 0.3f;
    public float fadeOutTime = 0.5f;

    private Animator animator;
    private CanvasGroup notificationCanvasGroup;

    private void Awake()
    {
        if (levelUpPanel != null)
        {
            notificationCanvasGroup = levelUpPanel.GetComponent<CanvasGroup>();
            if (notificationCanvasGroup == null)
            {
                notificationCanvasGroup = levelUpPanel.AddComponent<CanvasGroup>();
            }
            levelUpPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        LevelManager.OnLevelUp += OnLevelUp;
        GameManager.OnGameStart += OnGameStart;
    }

    private void OnDisable()
    {
        LevelManager.OnLevelUp -= OnLevelUp;
        GameManager.OnGameStart -= OnGameStart;
    }

    private void OnGameStart()
    {
        UpdateLevelDisplay(1);
    }

    private void Update()
    {
        // Update level display continuously
        if (LevelManager.Instance != null && levelText != null)
        {
            UpdateLevelDisplay(LevelManager.Instance.CurrentLevel);
        }
    }

    void UpdateLevelDisplay(int level)
    {
        if (levelText != null)
        {
            levelText.text = $"Level {level}";
        }
    }

    void OnLevelUp(int newLevel)
    {
        UpdateLevelDisplay(newLevel);
        ShowLevelUpNotification(newLevel);
    }

    void ShowLevelUpNotification(int level)
    {
        if (levelUpPanel != null && levelUpNotification != null)
        {
            StartCoroutine(LevelUpNotificationCoroutine(level));
        }
    }

    IEnumerator LevelUpNotificationCoroutine(int level)
    {
        // Set text
        levelUpNotification.text = $"LEVEL {level}!";

        // Show panel
        levelUpPanel.SetActive(true);
        notificationCanvasGroup.alpha = 0f;

        // Fade in
        float elapsed = 0f;
        while (elapsed < fadeInTime)
        {
            elapsed += Time.unscaledDeltaTime;
            notificationCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInTime);
            yield return null;
        }
        notificationCanvasGroup.alpha = 1f;

        // Wait
        yield return new WaitForSecondsRealtime(notificationDuration);

        // Fade out
        elapsed = 0f;
        while (elapsed < fadeOutTime)
        {
            elapsed += Time.unscaledDeltaTime;
            notificationCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutTime);
            yield return null;
        }

        // Hide panel
        levelUpPanel.SetActive(false);
    }
}