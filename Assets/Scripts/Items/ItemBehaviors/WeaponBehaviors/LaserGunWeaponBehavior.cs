using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LaserGunWeaponBehavior : WeaponBehavior
{
    private LaserGunWeaponDefinition _definition;
    public LaserGunWeaponBehavior(LaserGunWeaponDefinition definition) : base(CoroutineRunner.Instance)
    {
        _definition = definition;
    }

    public override void Use(ItemUsageContext context)
    {
        //TODO
        _isFiring = true;
        var points = ComputeLaserPath(context.AimOrigin, context.AimVector);
        context.LaserRenderer.DrawLaser(points.ToArray());
        StartCoroutine(FinishItemUsageWhenLaserAnimationFinishes());
    }

    private IEnumerator FinishItemUsageWhenLaserAnimationFinishes()
    {
        yield return new WaitForSeconds(2);
        _isFiring = false;
        InvokeItemUsageFinished();
    }

    private IEnumerable<Vector2> ComputeLaserPath(Vector2 origin, Vector2 direction)
    {
        int maxBounces = _definition.MaximumBounceCount.CalculateValue();
        float maxDistance = _definition.MaximumDistance.CalculateValue();
        
        List<Vector2> points = new List<Vector2>();
        points.Add(origin);

        Vector2 currentPos = origin;
        Vector2 currentDir = direction.normalized;

        for (int i = 0; i < maxBounces; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(currentPos, currentDir, maxDistance, LayerMaskHelper.GetLayerMask(Constants.GroundLayer));

            if (hit.collider == null)
            {
                points.Add(currentPos + currentDir * maxDistance);
                break;
            }

            points.Add(hit.point);

            currentDir = Vector2.Reflect(currentDir, hit.normal);

            // Move slightly away to avoid self-hitting
            currentPos = hit.point + currentDir * 0.01f;
        }

        return points;
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
