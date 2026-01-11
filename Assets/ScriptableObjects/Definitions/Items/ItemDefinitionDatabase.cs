using UnityEngine;

[CreateAssetMenu(fileName = "ItemDefinitionDatabase", menuName = "Scriptable Objects/Items/ItemDefinitionDatabase")]
public class ItemDefinitionDatabase : DefinitionDatabase<ItemDefinition>
{
    public override void Initialize()
    {
        base.Initialize();
        foreach(var def in _definitions )
        {
            if (def is ArmorDefinition armorDef)
            {
                armorDef.InitializeAnimations(); 
            }
        }
    }
}
