using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LaserPhysics
{
    private int _maximumBounceCount;
    private float _maximumDistance;
    private RaycastHit2D[] _raycastHitArray;
    private HashSet<Character> _hitCharacters;

    private const float _beamDirectionalOffset = .08f;
    private readonly Vector2 _beamGlobalOffset = new Vector2(0f, 0.05f);

    public LaserPhysics()
    {
        _raycastHitArray = new RaycastHit2D[Constants.RaycastHitColliderNumMax];
        _hitCharacters = new HashSet<Character>();
    }

    public void Initialize(int maximumBounceCount, float maximumDistance)
    {
        _maximumBounceCount = maximumBounceCount;
        _maximumDistance = maximumDistance;
    }

    public IEnumerable<Character> GetHitCharacters()
    {
        return _hitCharacters;
    }

    public IEnumerable<Vector2> CalculateLaserPath(Vector2 origin, Vector2 direction, Character owner)
    {
        _hitCharacters.Clear();
        var points = new List<Vector2>();

        origin += direction.normalized * _beamDirectionalOffset + _beamGlobalOffset;

        points.Add(origin);

        Vector2 currentPos = origin;
        Vector2 currentDir = direction.normalized;
        var mask = LayerMaskHelper.GetCombinedLayerMask(Constants.GroundLayer, Constants.CharacterLayer);
        var filter = new ContactFilter2D();
        filter.SetLayerMask(mask);

        for (int i = 0; i < _maximumBounceCount; i++)
        {
            int numHits = Physics2D.Raycast(currentPos, currentDir, filter, _raycastHitArray, _maximumDistance);
            var hits = _raycastHitArray.Take(numHits);
            var closestGroundHit = hits.Where(hit => hit.collider != null && hit.collider.tag == Constants.GroundTag).OrderBy(hit => hit.distance).FirstOrDefault();
            var characterHits = hits.Where(hit => hit.collider != null && hit.collider.tag == Constants.CharacterTag);
            var maxDistance = closestGroundHit.collider != null ? closestGroundHit.distance : _maximumDistance;

            foreach (var cHit in characterHits)
            {
                if(cHit.distance > maxDistance)
                {
                    continue;
                }

                cHit.collider.TryGetComponent<Character>(out var c);

                if (i == 0 && c == owner)
                {
                    continue;
                }

                _hitCharacters.Add(c);
            }

            if (closestGroundHit.collider == null)
            {
                points.Add(currentPos + currentDir * _maximumDistance);
                break;
            }

            var hit = closestGroundHit;

            points.Add(hit.point);

            currentDir = Vector2.Reflect(currentDir, hit.normal);

            // Move slightly away to avoid self-hitting
            currentPos = hit.point + currentDir * 0.01f;
        }

        return points;
    }

}
