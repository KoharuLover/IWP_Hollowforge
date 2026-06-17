using UnityEngine;

[System.Serializable]
public class EquipmentInstance
{
    public EquipmentData equipmentData;
    public int starLevel;

    public EquipmentInstance(EquipmentData equipmentData, int starLevel = 1)
    {
        this.equipmentData = equipmentData;
        this.starLevel = starLevel;
    }

    public string GetDisplayName()
    {
        if (equipmentData == null)
        {
            return "Empty";
        }

        return equipmentData.equipmentName + " " + GetStarText();
    }

    public string GetStarText()
    {
        if (starLevel == 1)
        {
            return "*";
        }
        else if (starLevel == 2)
        {
            return "**";
        }
        else if (starLevel == 3)
        {
            return "***";
        }

        return "";
    }

    public float GetStarMultiplier()
    {
        if (starLevel == 1)
        {
            return 1f;
        }
        else if (starLevel == 2)
        {
            return 2f;
        }
        else if (starLevel == 3)
        {
            return 3f;
        }

        return 1f;
    }

    public float GetMaxHealthBonus()
    {
        return equipmentData.maxHealthBonus * GetStarMultiplier();
    }

    public float GetMaxManaBonus()
    {
        return equipmentData.maxManaBonus * GetStarMultiplier();
    }

    public float GetAttackBonus()
    {
        return equipmentData.attackBonus * GetStarMultiplier();
    }

    public float GetAttackSpeedBonus()
    {
        return equipmentData.attackSpeedBonus * GetStarMultiplier();
    }

    public float GetMagicPowerBonus()
    {
        return equipmentData.magicPowerBonus * GetStarMultiplier();
    }

    public float GetMovementSpeedBonus()
    {
        return equipmentData.movementSpeedBonus * GetStarMultiplier();
    }

    public float GetDashRangeBonus()
    {
        return equipmentData.dashRangeBonus * GetStarMultiplier();
    }

    public bool IsSameEquipmentAndStar(EquipmentInstance other)
    {
        if (other == null)
        {
            return false;
        }

        return equipmentData == other.equipmentData && starLevel == other.starLevel;
    }
}
