using System;
using System.Collections.Generic;
using System.Linq;

public class CharacterArmorManager
{
    private readonly Queue<Item> _protectiveArmors = new Queue<Item>();
    private readonly List<Item> _otherArmors = new List<Item>();
    public bool IsProtected => _protectiveArmors.Any();

    public event Action<Item> BlockedWithArmor;

    public bool TryEquipArmor(Item armor)
    {
        if ((armor.Definition as ArmorDefinition).IsProtective)
        {
            if(_protectiveArmors.Contains(armor))
            {
                return false;
            }
            _protectiveArmors.Enqueue(armor);
        }
        else 
        {
            if (_otherArmors.Contains(armor))
            {
                return false;
            }    
            _otherArmors.Add(armor);
        }
        return true;
    }

    public void OnBlockedAttack()
    {
        var lastProtectiveArmor = _protectiveArmors.Peek();
        BlockedWithArmor?.Invoke(lastProtectiveArmor);
    }
}
