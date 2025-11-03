using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerUpUI : MonoBehaviour
{
    public static PowerUpUI Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject powerUpSlotPrefab; // Prefab for individual power-up display
    public Transform powerUpContainer;    // Parent container for power-up slots

    private Dictionary<PowerUpType, PowerUpSlot> activeSlots = new Dictionary<PowerUpType, PowerUpSlot>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowPowerUp(PowerUpType type, float duration, Sprite icon)
    {
        // If already showing, just restart timer
        if (activeSlots.ContainsKey(type))
        {
            activeSlots[type].RestartTimer(duration);
            return;
        }

        // Create new slot
        GameObject slotObj = Instantiate(powerUpSlotPrefab, powerUpContainer);
        PowerUpSlot slot = slotObj.GetComponent<PowerUpSlot>();

        if (slot != null)
        {
            slot.Initialize(type, duration, icon);
            activeSlots[type] = slot;
        }
    }

    public void HidePowerUp(PowerUpType type)
    {
        if (activeSlots.ContainsKey(type))
        {
            PowerUpSlot slot = activeSlots[type];
            activeSlots.Remove(type);

            if (slot != null)
            {
                Destroy(slot.gameObject);
            }
        }
    }

    private void OnEnable()
    {
        GameManager.OnGameOver += ClearAllPowerUps;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= ClearAllPowerUps;
    }

    void ClearAllPowerUps()
    {
        foreach (var slot in activeSlots.Values)
        {
            if (slot != null)
            {
                Destroy(slot.gameObject);
            }
        }
        activeSlots.Clear();
    }
}