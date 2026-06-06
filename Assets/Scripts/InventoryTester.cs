using UnityEngine;

public class InventoryTester : MonoBehaviour
{
    [Header("Test Equipment")]
    [SerializeField] private EquipmentData testWeapon;
    [SerializeField] private EquipmentData testArmor;
    [SerializeField] private EquipmentData testArtifact;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            AddTestItem(testWeapon);
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            AddTestItem(testArmor);
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            AddTestItem(testArtifact);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            EquipFirstInventoryItem();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            PrintInventory();
            PrintEquippedItems();
        }
    }

    private void AddTestItem(EquipmentData equipmentData)
    {
        if (equipmentData == null)
        {
            Debug.LogWarning("No test equipment assigned.");
            return;
        }

        EquipmentInstance newEquipment = new EquipmentInstance(equipmentData, 1);
        InventoryManager.Instance.AddToInventory(newEquipment);
    }

    private void EquipFirstInventoryItem()
    {
        if (InventoryManager.Instance.InventorySlots.Count <= 0)
        {
            Debug.Log("Inventory is empty. Add an item first.");
            return;
        }

        EquipmentInstance equipment = InventoryManager.Instance.InventorySlots[0];
        EquipmentManager.Instance.EquipItem(equipment);
    }

    private void PrintInventory()
    {
        Debug.Log("===== INVENTORY =====");

        for (int i = 0; i < InventoryManager.Instance.InventorySlots.Count; i++)
        {
            EquipmentInstance equipment = InventoryManager.Instance.InventorySlots[i];
            Debug.Log("Slot " + (i + 1) + ": " + equipment.GetDisplayName());
        }
    }

    private void PrintEquippedItems()
    {
        Debug.Log("===== EQUIPPED ITEMS =====");

        PrintSlot("Weapon Slot 1", EquipmentManager.Instance.WeaponSlot1);
        PrintSlot("Weapon Slot 2", EquipmentManager.Instance.WeaponSlot2);

        PrintSlot("Helmet", EquipmentManager.Instance.HelmetSlot);
        PrintSlot("Chestplate", EquipmentManager.Instance.ChestplateSlot);
        PrintSlot("Pants", EquipmentManager.Instance.PantsSlot);
        PrintSlot("Boots", EquipmentManager.Instance.BootsSlot);

        for (int i = 0; i < EquipmentManager.Instance.ArtifactSlots.Length; i++)
        {
            PrintSlot("Artifact Slot " + (i + 1), EquipmentManager.Instance.ArtifactSlots[i]);
        }
    }

    private void PrintSlot(string slotName, EquipmentInstance equipment)
    {
        if (equipment == null)
        {
            Debug.Log(slotName + ": Empty");
        }
        else
        {
            Debug.Log(slotName + ": " + equipment.GetDisplayName());
        }
    }
}
