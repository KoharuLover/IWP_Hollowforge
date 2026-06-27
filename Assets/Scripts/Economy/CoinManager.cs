using System;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    public event Action<int> CoinsChanged;

    [Header("Coin Settings")]
    [SerializeField] private int startingCoins = 45;

    public int CurrentCoins { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CurrentCoins = startingCoins;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public bool HasEnoughCoins(int amount)
    {
        return CurrentCoins >= amount;
    }

    public bool TrySpendCoins(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Cannot spend a negative number of coins.");
            return false;
        }

        if (HasEnoughCoins(amount) == false)
        {
            Debug.Log("Not enough coins.");
            return false;
        }

        CurrentCoins -= amount;

        NotifyCoinsChanged();

        Debug.Log("Spent " + amount + " coins. Remaining coins: " + CurrentCoins);
        return true;
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("Coin amount must be greater than zero.");
            return;
        }

        CurrentCoins += amount;

        NotifyCoinsChanged();

        Debug.Log("Added " + amount + " coins. Current coins: " + CurrentCoins);
    }

    public void ResetCoins()
    {
        CurrentCoins = startingCoins;
        NotifyCoinsChanged();
    }

    private void NotifyCoinsChanged()
    {
        CoinsChanged?.Invoke(CurrentCoins);
    }
}