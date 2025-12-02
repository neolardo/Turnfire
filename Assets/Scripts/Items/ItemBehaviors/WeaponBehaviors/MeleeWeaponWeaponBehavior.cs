using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponWeaponBehavior : WeaponBehavior
{
    private MeleeWeaponWeaponDefinition _definition;
    public MeleeWeaponWeaponBehavior(MeleeWeaponWeaponDefinition definition) : base(CoroutineRunner.Instance)
    {
        _definition = definition;
    }

    public override void Use(ItemUsageContext context)
    {
        //TODO
    }

    public override void InitializePreview(ItemUsageContext context, ItemPreviewRendererManager rendererManager)
    {
        //TODO
    }

    public override WeaponBehaviorSimulationResult SimulateWeaponBehavior(Vector2 start, Vector2 aimVector, DestructibleTerrainManager terrain, Character owner, IEnumerable<Character> others)
    {
        //TODO
        return WeaponBehaviorSimulationResult.Zero;
    }
}
