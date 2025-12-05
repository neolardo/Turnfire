using System;
using System.Collections.Generic;
using System.Linq;

public class CharacterArmorManager
{
    private readonly List<ArmorDefinition> _protectiveArmors = new List<ArmorDefinition>();
    private readonly List<ArmorDefinition> _otherArmors = new List<ArmorDefinition>();
    public bool IsProtected => _protectiveArmors.Any();

    public event Action<ArmorDefinition> BlockedWithArmor;
    public event Action<ArmorDefinition> ArmorEquipped;
    public event Action<ArmorDefinition> ArmorUnequipped;

    public bool CanEquip(ArmorDefinition armorDefinition)
    {
        var armorList = armorDefinition.IsProtective ? _protectiveArmors : _otherArmors;
        return !armorList.Contains(armorDefinition);
    }

    public bool TryEquipArmor(ArmorDefinition armorDefinition, ArmorBehavior armorBehavior)
    {
        var armorList = armorDefinition.IsProtective ?_protectiveArmors : _otherArmors;

        if (armorList.Contains(armorDefinition))
        {
            return false;
        }

        armorBehavior.ArmorWornOut += OnArmorWornOut;
        armorList.Add(armorDefinition);
        ArmorEquipped?.Invoke(armorDefinition);
        return true;
    }

    private void OnArmorWornOut(ArmorDefinition armorDefinition)
    {
        var armorList = armorDefinition.IsProtective ? _protectiveArmors : _otherArmors;
        var armor = armorList.First(item => item == armorDefinition);
        armorList.Remove(armor);
        ArmorUnequipped?.Invoke(armorDefinition);
    }


    public ArmorDefinition BlockAttack()
    {
        var firstProtectiveArmor = _protectiveArmors.First();
        BlockedWithArmor?.Invoke(firstProtectiveArmor);
        return firstProtectiveArmor;
    }
}
