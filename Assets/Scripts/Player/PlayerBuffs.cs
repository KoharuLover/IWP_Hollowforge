using System.Collections;
using UnityEngine;


public class PlayerBuffs : MonoBehaviour
{
    private PlayerStats _playerStats;

    private void Awake()
    {
        _playerStats = GetComponent<PlayerStats>();
    }

    public void ApplyTemporaryBuff(StatType statType, float amount, float duration)
    {
        StartCoroutine(TemporaryBuffRoutine(statType, amount, duration));
    }

    private IEnumerator TemporaryBuffRoutine(StatType statType, float amount, float duration)
    {
        _playerStats.AddModifier(statType, amount);

        yield return new WaitForSeconds(duration);

        _playerStats.AddModifier(statType, -amount);
    }
}
