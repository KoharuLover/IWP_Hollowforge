using TMPro;
using UnityEngine;

public class CoinDisplayUI : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TMP_Text _coinsText;

    private void Start()
    {
        if (CoinManager.Instance == null)
        {
            Debug.LogWarning("CoinDisplayUI could not find CoinManager.", gameObject);
            return;
        }

        CoinManager.Instance.CoinsChanged += UpdateCoinText;
        UpdateCoinText(CoinManager.Instance.CurrentCoins);
    }

    private void OnDestroy()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.CoinsChanged -= UpdateCoinText;
        }
    }

    private void UpdateCoinText(int coinAmount)
    {
        if (_coinsText != null)
        {
            _coinsText.text = "Coins: " + coinAmount;
        }
    }
}