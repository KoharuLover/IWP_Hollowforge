using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private Slider _healthSlider;

    private void Awake()
    {
        if (_healthSlider == null)
        {
            _healthSlider = GetComponent<Slider>();
        }
    }

    private void Start()
    {
        _healthSlider.maxValue = _playerHealth.MaxHealth;
    }

    private void Update()
    {
        _healthSlider.maxValue = _playerHealth.MaxHealth;
        _healthSlider.value = _playerHealth.CurrentHealth;
    }
}
