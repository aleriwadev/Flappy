using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Individual powerups display a slot
public class PowerUpSlot : MonoBehaviour
{
    [Header("UI Components")]
    public Image iconImage;
    public Image fillImage;
    public TextMeshProUGUI timerText;

    private PowerUpType type;
    private float duration;
    private float timeRemaining;

    public void Initialize(PowerUpType powerUpType, float powerUpDuration, Sprite icon)
    {
        type = powerUpType;
        duration = powerUpDuration;
        timeRemaining = powerUpDuration;

        if (iconImage != null && icon != null)
        {
            iconImage.sprite = icon;
        }

        if (fillImage != null)
        {
            fillImage.fillAmount = 1f;
        }
    }

    public void RestartTimer(float newDuration)
    {
        duration = newDuration;
        timeRemaining = newDuration;
    }

    private void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.unscaledDeltaTime; // Use unscaled for slow-motion power-up

            // Update fill amount
            if (fillImage != null)
            {
                fillImage.fillAmount = timeRemaining / duration;
            }

            // Update timer text
            if (timerText != null)
            {
                timerText.text = Mathf.Ceil(timeRemaining).ToString();
            }
        }
    }
}