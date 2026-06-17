using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private GameObject _victoryPanel;

    private PlayerHealth _playerHealth;
    private bool _gameEnded;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            _playerHealth = playerObject.GetComponent<PlayerHealth>();
        }

        if (_gameOverPanel != null)
        {
            _gameOverPanel.SetActive(false);
        }

        if (_victoryPanel != null)
        {
            _victoryPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (_gameEnded)
        {
            return;
        }

        CheckGameOver();
        CheckVictory();
    }

    private void CheckGameOver()
    {
        if (_playerHealth == null)
        {
            return;
        }

        if (_playerHealth.IsDead)
        {
            _gameEnded = true;

            if (_gameOverPanel != null)
            {
                _gameOverPanel.SetActive(true);
            }

            Debug.Log("Game Over");
        }
    }

    private void CheckVictory()
    {
        EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);

        if (enemies.Length <= 0)
        {
            _gameEnded = true;

            if (_victoryPanel != null)
            {
                _victoryPanel.SetActive(true);
            }

            Debug.Log("Victory");
        }
    }
}
