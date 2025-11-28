using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JumpGraph : UnityDriven
{
    private readonly List<StandingPoint> _points = new List<StandingPoint>();
    private readonly List<Dictionary<int, JumpLink>> _adjency = new List<Dictionary<int, JumpLink>>();

    private readonly int _pixelResolution;
    private readonly int _pixelsPerUnit;
    private readonly float _jumpStrength;
    private readonly float _characterWidth;
    private readonly float _characterHeight;

    private readonly Vector2[] CharacterColliderCornerPoints = new Vector2[6];
    private float GridPointHalfDistance =>  (_pixelResolution / 2f) / _pixelsPerUnit;
    private float JumpValidationDistance =>  _pixelResolution  / (float)_pixelsPerUnit;

    private const float ExplosionRadiusMultiplierForNewJumpLinks = 3;
    private const float CollisionCheckDelaySeconds = .1f;

    private bool _isReady;
    public bool IsReady => _isReady;

    // path calculation
    private readonly Queue<int> _pointQueue = new Queue<int>();
    private readonly HashSet<int> _visitedHashSet = new HashSet<int>();
    private readonly Dictionary<int, int> _parentDict = new Dictionary<int, int>(); // childId -> parentId
    private readonly Dictionary<int, JumpLink> _parentLinkDict = new Dictionary<int, JumpLink>();

    public JumpGraph(MonoBehaviour coroutineManager, int pixelResolution, int pixelsPerUnit, float jumpStrength, float characterWidth, float characterHeight) : base(coroutineManager)
    {
        _pixelResolution = pixelResolution;
        _pixelsPerUnit = pixelsPerUnit;
        _jumpStrength = jumpStrength;
        _characterWidth = characterWidth;
        _characterHeight = characterHeight;
        CacheCharacterColliderCornerPoints();
    }

    private void CacheCharacterColliderCornerPoints()
    {
        CharacterColliderCornerPoints[0] = new Vector2(-_characterWidth / 2, 0);
        CharacterColliderCornerPoints[1] = new Vector2(_characterWidth / 2, 0);
        CharacterColliderCornerPoints[2] = new Vector2(-_characterWidth / 2, _characterHeight / 2);
        CharacterColliderCornerPoints[3] = new Vector2(_characterWidth / 2, _characterHeight / 2);
        CharacterColliderCornerPoints[4] = new Vector2(-_characterWidth / 2, _characterHeight);
        CharacterColliderCornerPoints[5] = new Vector2(_characterWidth / 2, _characterHeight);
    }

    #region Creation

    public void InitiateGraphCreationFromTerrain(DestructibleTerrainManager terrain)
    {
        StartCoroutine(CreateStandingPointsAndJumpLinks(terrain));
    }

    private IEnumerator CreateStandingPointsAndJumpLinks(DestructibleTerrainManager terrain)
    {
        yield return CreateStandingPoints(terrain);
        yield return SimulateJumpsFromGivenPointsAndCreateJumpLinks(_points, _points, terrain);
        _isReady = true;
        Debug.Log($"Jump graph creation finished");
    }

    private IEnumerator CreateStandingPoints(DestructibleTerrainManager terrain)
    {
        Debug.Log($"Jump graph creation started");
        const int iterationThreshold = 50000;
        int iteration = 0;
        var terrainPixelSize = terrain.PixelSize;
        for (int x = 0; x < terrainPixelSize.x; x += _pixelResolution)
        {
            for (int y = 0; y < terrainPixelSize.y; y += _pixelResolution)
            {
                if (terrain.TryFindNearestStandingPoint(new Vector2Int(x, y), _pixelResolution / 2, _points.Count, out var standingPoint))
                {
                    _points.Add(standingPoint);
                    _adjency.Add(new Dictionary<int, JumpLink>());
                }
                iteration++;
                if (iteration % iterationThreshold == 0)
                {
                    yield return null;
                }
            }
        }
        Debug.Log($"Created {_points.Count} standing points");
    }
    private IEnumerator SimulateJumpsFromGivenPointsAndCreateJumpLinks(IEnumerable<StandingPoint> startPoints, IEnumerable<StandingPoint> endPoints, DestructibleTerrainManager terrain)
    {
        int linkCount = 0;
        foreach (var startP in startPoints)
        {
            if(!startP.IsValid)
                 continue;
            
            for (float angle = 0; angle < 180f; angle += Constants.AimAngleSimulationStep) // negative jumps are not simulated
            {
                Vector2 direction = angle.AngleDegreesToVector();

                for (float strength = 0; strength <= 1f; strength += Constants.AimStrengthSimulationStep)
                {
                    Vector2 jumpVector = direction * strength;
                    var destination = SimulateJumpAndCalculateDestination(startP.WorldPos, jumpVector, terrain);
                    foreach (var endP in endPoints) 
                    {
                        if (endP.Id == startP.Id || !endP.IsValid || endP.IsCornerPoint)
                            continue;

                        var delta = endP.WorldPos - destination;
                        if (Mathf.Abs(delta.x) < GridPointHalfDistance && Mathf.Abs(delta.y) < GridPointHalfDistance)
                        {
                            if (!_adjency[startP.Id].ContainsKey(endP.Id))
                            {
                                _adjency[startP.Id].Add(endP.Id, new JumpLink(startP.Id, endP.Id, jumpVector));
                                linkCount++;
                            }
                            break;
                        }
                    }

                }
            }
        }
        yield return null;
        Debug.Log($"Jump graph updated with: {linkCount} links");
    }

    private Vector2 SimulateJumpAndCalculateDestination(Vector2 start, Vector2 jumpVector, DestructibleTerrainManager terrain)
    {
        Vector2 pos = start;
        var velocity = jumpVector * _jumpStrength;
        const float dt = Constants.ParabolicPathSimulationDeltaForMovement;
        for (float t = 0; t < Constants.MaxParabolicPathSimulationTime; t += Constants.ParabolicPathSimulationDeltaForMovement)
        {
            pos += velocity * dt;
            velocity += Physics2D.gravity * dt;

            if (!terrain.IsPointInsideBounds(pos))
                return pos;

            if (t >= CollisionCheckDelaySeconds)
            {
                foreach (var colliderPoint in CharacterColliderCornerPoints)
                {
                    if (terrain.OverlapPoint(pos + colliderPoint))
                    {
                        return pos;
                    }
                }
            }

        }
        return pos;
    }

    #endregion

    #region Update

    public void ApplyExplosion(Vector2 explosionCenter, float explosionRadius, DestructibleTerrainManager terrain)
    {
        Debug.Log($"Jump graph update started");
        _isReady = false;
        var numPointsRemoved = RemovePointsAndLinksInsideExplosion(explosionCenter, explosionRadius);
        if (numPointsRemoved>0)
        {
            StartCoroutine(AddNewStandingPointAndJumpLinksAfterExplosion(explosionCenter, explosionRadius, terrain));
        }
        else
        {
            _isReady = true;
            Debug.Log($"Jump graph update finished");
        }
    }

    private int RemovePointsAndLinksInsideExplosion(Vector2 explosionCenter, float explosionRadius)
    {
        var explodedPoints = new List<StandingPoint>();
        for (int i = 0; i < _points.Count; i++)
        {
            var p = _points[i];
            if (p.IsValid && Vector2.Distance(p.WorldPos, explosionCenter) < explosionRadius)
            {
                explodedPoints.Add(p);
                _points[i] = StandingPoint.InvalidPoint;
                _adjency[p.Id].Clear();
            }
        }

        foreach (var p in _points)
        {
            if(!p.IsValid)
                continue;
            
            foreach (var exP in explodedPoints)
            {
                if (_adjency[p.Id].ContainsKey(exP.Id))
                {
                    _adjency[p.Id].Remove(exP.Id);
                }
            }
        }

        return explodedPoints.Count;
    }

    private IEnumerator AddNewStandingPointAndJumpLinksAfterExplosion(Vector2 explosionCenter, float explosionRadius, DestructibleTerrainManager terrain)
    {
        if(terrain.TryFindNearestStandingPoint(explosionCenter + Vector2.down * explosionRadius, _pixelResolution / 2, _points.Count, out var newStandingPoint))
        {
            var possibleLinkedPoints = new List<StandingPoint>();
            foreach(var p in _points)
            {
                if (!p.IsValid)
                    continue;

                if(Vector2.Distance(p.WorldPos, explosionCenter) < explosionRadius * ExplosionRadiusMultiplierForNewJumpLinks)
                {
                    possibleLinkedPoints.Add(p);
                }
            }

            _points.Add(newStandingPoint);
            _adjency.Add(new Dictionary<int, JumpLink>());

            yield return SimulateJumpsFromGivenPointsAndCreateJumpLinks(possibleLinkedPoints, new StandingPoint[] { newStandingPoint }, terrain);
            yield return SimulateJumpsFromGivenPointsAndCreateJumpLinks(new StandingPoint[] { newStandingPoint }, _points , terrain);
        }
        Debug.Log($"Jump graph update finished");
        _isReady = true;
    }


    #endregion

    #region Path Search

    public bool TryCalculateJumpDistanceBetween(StandingPoint start, StandingPoint end, out int numJumps)
    {
        var path = FindShortestJumpPath(start, end);
        numJumps = path == null ? -1 : path.Count;
        return path != null; 
    }

    public IEnumerable<StandingPoint> GetAllLinkedStandingPointsFromPoint(StandingPoint startPoint)
    {
        return _adjency[startPoint.Id].Values.Select(link => _points[link.ToId]);
    }

    public IEnumerable<StandingPoint> GetAllReachableStandingPointsFromPoint(StandingPoint startPoint) //BFS
    {
        var reachable = new List<StandingPoint>();

        if (!startPoint.IsValid)
            return reachable;

        var visited = new HashSet<int>();
        var queue = new Queue<int>();

        visited.Add(startPoint.Id);
        queue.Enqueue(startPoint.Id);

        while (queue.Count > 0)
        {
            int currentId = queue.Dequeue();
            var currentPoint = _points[currentId];

            // Collect valid points only
            if (currentPoint.IsValid)
                reachable.Add(currentPoint);

            // For each outgoing jump from this point
            foreach (var kvp in _adjency[currentId])
            {
                int nextId = kvp.Key;

                // skip invalid or visited nodes
                if (visited.Contains(nextId))
                    continue;

                if (!_points[nextId].IsValid)
                    continue;

                visited.Add(nextId);
                queue.Enqueue(nextId);
            }
        }

        return reachable;
    }


    public StandingPoint FindClosestStandingPoint(Vector2 position)
    {
        return _points[_points.Where(p=> p.IsValid).Select(p => (Vector2.Distance(p.WorldPos, position), p.Id)).Min().Id];
    }

    public List<JumpLink> FindShortestJumpPath(StandingPoint start, StandingPoint end) // BFS
    {
        if (start.Id == end.Id)
            return new List<JumpLink>();

        _pointQueue.Clear();
        _visitedHashSet.Clear();
        _parentDict.Clear();
        _parentLinkDict.Clear();

        _pointQueue.Enqueue(start.Id);
        _visitedHashSet.Add(start.Id);

        while (_pointQueue.Count > 0)
        {
            int current = _pointQueue.Dequeue();

            foreach (var kvp in _adjency[current])
            {
                int nextId = kvp.Key;
                JumpLink link = kvp.Value;

                if (_visitedHashSet.Contains(nextId))
                    continue;

                _visitedHashSet.Add(nextId);
                _parentDict[nextId] = current;
                _parentLinkDict[nextId] = link;

                if (nextId == end.Id)
                {
                    return ReconstructPath(start.Id, end.Id, _parentDict, _parentLinkDict);
                }

                _pointQueue.Enqueue(nextId);
            }
        }

        return null;
    }

    private List<JumpLink> ReconstructPath(int startId, int endId, Dictionary<int, int> parent, Dictionary<int, JumpLink> parentLink)
    {
        var path = new List<JumpLink>();
        int current = endId;

        while (current != startId)
        {
            if (!parentLink.TryGetValue(current, out var link))
                break;

            path.Add(link);
            current = parent[current];
        }

        path.Reverse();
        return path;
    }



    #endregion

    #region Validation and Correction

    public bool IsJumpPredictionValid(Vector2 start, JumpLink jumpLink, DestructibleTerrainManager terrain)
    {
        var destination = SimulateJumpAndCalculateDestination(start, jumpLink.JumpVector, terrain);
        var startDelta = start - _points[jumpLink.FromId].WorldPos;
        var predictedEnd = _points[jumpLink.ToId].WorldPos + startDelta;
        return Vector2.Distance(predictedEnd, destination) < JumpValidationDistance;
    }

    public Vector2 CalculateCorrectedJumpVectorToStandingPoint(Vector2 start, StandingPoint standingPoint)
    {
        return CalculateJumpVector(start, standingPoint.WorldPos);
    }

    private Vector2 CalculateJumpVector( Vector2 from, Vector2 to)
    {
        Vector2 jumpVector = (to - from).normalized;
        Vector2 g = Physics2D.gravity;

        const int steps = 30;
        const float minTime = 0.5f;
        const float maxTime = 10f;
        float bestT = -1f;
        Vector2 bestV = Vector2.zero;

        for (int i = 0; i < steps; i++)
        {
            float t = Mathf.Lerp(minTime, maxTime, i / (float)(steps - 1));

            Vector2 V = (to - from - 0.5f * g * t * t) / t;

            if (V.magnitude <= _jumpStrength)
            {
                bestT = t;
                bestV = V;
                break; // shortest-arc valid jump
            }
        }

        if (bestT > 0f)
        {
            jumpVector = bestV / _jumpStrength;
        }

        return jumpVector;
    }

    #endregion

}
