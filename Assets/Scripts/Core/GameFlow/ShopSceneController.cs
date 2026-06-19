using UnityEngine;

public class ShopSceneController : MonoBehaviour
{
    private void Start()
    {
        UnlockEquipmentEditing();
    }

    private void UnlockEquipmentEditing()
    {
        if (EquipmentManager.Instance == null)
        {
            Debug.LogWarning("ShopSceneController could not find the EquipmentManager.", this);
            return;
        }

        EquipmentManager.Instance.SetEquipmentEditingAllowed(true);
    }
    public void ContinueToCombat()
    {
        SceneLoader.LoadCombatScene();
    }
}
