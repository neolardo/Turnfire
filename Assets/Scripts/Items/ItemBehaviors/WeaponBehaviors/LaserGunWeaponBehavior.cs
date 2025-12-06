using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngineInternal;

public class LaserGunWeaponBehavior : WeaponBehavior
{
    private LaserGunWeaponDefinition _definition;
    private RaycastHit2D[] _raycastHitArray;
    public LaserGunWeaponBehavior(LaserGunWeaponDefinition definition) : base(CoroutineRunner.Instance)
    {
        _definition = definition;
        _raycastHitArray = new RaycastHit2D[Constants.RaycastHitColliderNumMax];
    }

    public override void Use(ItemUsageContext context)
    {
        //TODO
        _isFiring = true;
        var points = CalculateLaserPath(context.Owner, context.AimOrigin, context.AimVector, out var hitCharacers);
        foreach (var c in hitCharacers)
        {
            c.Damage(_definition.Damage.CalculateValue());
        }
        context.LaserRenderer.DrawLaser(points.ToArray());
        StartCoroutine(FinishItemUsageWhenLaserAnimationFinishes());
    }

    private IEnumerator FinishItemUsageWhenLaserAnimationFinishes()
    {
        yield return new WaitForSeconds(2);
        _isFiring = false;
        InvokeItemUsageFinished();
    }

    private IEnumerable<Vector2> CalculateLaserPath(Character owner, Vector2 origin, Vector2 direction, out IEnumerable<Character> hitCharacters)
    {
        int maxBounces = _definition.MaximumBounceCount.CalculateValue();
        float maxDistance = _definition.MaximumDistance.CalculateValue();
        
        var points = new List<Vector2>();
        var characters = new HashSet<Character>();
        points.Add(origin);

        Vector2 currentPos = origin;
        Vector2 currentDir = direction.normalized;
        var mask = LayerMaskHelper.GetCombinedLayerMask(Constants.GroundLayer, Constants.CharacterLayer);
        var filter = new ContactFilter2D();
        filter.SetLayerMask(mask);

        for (int i = 0; i < maxBounces; i++)
        {
            int numHits = Physics2D.Raycast(currentPos, currentDir, filter,_raycastHitArray, maxDistance);
            var hits = _raycastHitArray.Take(numHits);
            var closestGroundHit = hits.Where(hit => hit.collider != null && hit.collider.tag == Constants.GroundTag).OrderBy(hit => hit.distance).FirstOrDefault();
            var characterHits = hits.Where(hit => hit.collider != null && hit.collider.tag == Constants.CharacterTag);

            foreach(var cHit in characterHits)
            {
                cHit.collider.TryGetComponent<Character>(out var c);

                if (i == 0 && c == owner)
                {
                    continue;
                }

                characters.Add(c);
            }

            if (closestGroundHit.collider == null)
            {
                points.Add(currentPos + currentDir * maxDistance);
                break;
            }

            var hit = closestGroundHit;

            points.Add(hit.point);

            currentDir = Vector2.Reflect(currentDir, hit.normal);

            // Move slightly away to avoid self-hitting
            currentPos = hit.point + currentDir * 0.01f;
        }
        hitCharacters = characters;

        return points;
    }

    public override void InitializePreview(ItemUsageContext context, ItemPreviewRendererManager rendererManager)
    {
        rendererManager.SelectRenderer(ItemPreviewRendererType.Trajectory);
        rendererManager.TrajectoryRenderer.ToggleGravity(false);
        rendererManager.TrajectoryRenderer.SetOrigin(context.Owner.ItemTransform);
        rendererManager.TrajectoryRenderer.SetTrajectoryMultipler(1);
    }

    public override WeaponBehaviorSimulationResult SimulateWeaponBehavior(Vector2 start, Vector2 aimVector, DestructibleTerrainManager terrain, Character owner, IEnumerable<Character> others)
    {
        //TODO
        return WeaponBehaviorSimulationResult.Zero;
    }


}
