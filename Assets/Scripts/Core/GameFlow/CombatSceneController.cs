using UnityEngine;

public class CombatSceneController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private GameObject _victoryPanel;

    [Header("Combat")]
    [SerializeField] private bool _startCombatAutomatically = true;

    private PlayerHealth _playerHealth;

    private bool _combatStarted;
    private bool _combatEnded;

    private void Start()
    {
        FindPlayer();
        HideEndPanels();

        if (_startCombatAutomatically)
        {
            BeginCombat();
        }
    }

    private void Update()
    {
        if (_combatStarted == false || _combatEnded)
        {
            return;
        }

        CheckGameOver();
        CheckVictory();
    }

    public void BeginCombat()
    {
        _combatStarted = true;
        _combatEnded = false;

        HideEndPanels();

        Debug.Log("Combat started.");
    }

    private void FindPlayer()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            Debug.LogWarning(
                "CombatSceneController could not find the Player.",
                this
            );

            return;
        }

        _playerHealth =
            playerObject.GetComponent<PlayerHealth>();

        if (_playerHealth == null)
        {
            Debug.LogWarning(
                "The Player does not have a PlayerHealth component.",
                playerObject
            );
        }
    }

    private void HideEndPanels()
    {
        if (_gameOverPanel != null)
        {
            _gameOverPanel.SetActive(false);
        }

        if (_victoryPanel != null)
        {
            _victoryPanel.SetActive(false);
        }
    }

    private void CheckGameOver()
    {
        if (_playerHealth == null)
        {
            return;
        }

        if (_playerHealth.IsDead == false)
        {
            return;
        }

        EndCombatWithGameOver();
    }

    private void CheckVictory()
    {
        EnemyHealth[] enemies =
            FindObjectsByType<EnemyHealth>(
                FindObjectsSortMode.None
            );

        if (enemies.Length > 0)
        {
            return;
        }

        EndCombatWithVictory();
    }

    private void EndCombatWithGameOver()
    {
        _combatEnded = true;

        if (_gameOverPanel != null)
        {
            _gameOverPanel.SetActive(true);
        }

        Debug.Log("Game Over");
    }

    private void EndCombatWithVictory()
    {
        _combatEnded = true;

        if (_victoryPanel != null)
        {
            _victoryPanel.SetActive(true);
        }

        Debug.Log("Combat room cleared.");
    }
}
