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
    private readonly float _globalJumpStrengthHighestMaximum;
    private readonly float _globalJumpStrengthLowestMaximum;
    private readonly float _characterWidth;
    private readonly float _characterHeight;
    private float _currentJumpStrengthMaximum;

    private readonly Vector2[] CharacterColliderCornerPoints = new Vector2[6];
    private float GridPointHalfDistance =>  (_pixelResolution / 2f) / _pixelsPerUnit;

    private const float CollisionCheckDelaySeconds = .1f;

    // explosion and new points
    private const float ExplosionRadiusOffsetForNewJumpLinks = 3;
    private const float TwoPointsExplosionRadiusThreshold = 1f;
    private const float ThreePointsExplosionRadiusThreshold = 1.5f;
    private const float NewSearchPointAngularDistanceDegrees = 25f;

    private bool _isReady;
    public bool IsReady => _isReady;


    // path calculation
    private readonly Queue<int> _pointQueue = new Queue<int>();
    private readonly HashSet<int> _visitedHashSet = new HashSet<int>();
    private readonly Dictionary<int, int> _parentDict = new Dictionary<int, int>(); // childId -> parentId
    private readonly Dictionary<int, JumpLink> _parentLinkDict = new Dictionary<int, JumpLink>();

    public JumpGraph(MonoBehaviour coroutineManager, int pixelResolution, int pixelsPerUnit, float globalJumpStrengthHighestMaximum, float globalJumpStrengthLowestMaximum, float characterWidth, float characterHeight) : base(coroutineManager)
    {
        _pixelResolution = pixelResolution;
        _pixelsPerUnit = pixelsPerUnit;
        _globalJumpStrengthHighestMaximum = globalJumpStrengthHighestMaximum;
        _globalJumpStrengthLowestMaximum = globalJumpStrengthLowestMaximum;
        _currentJumpStrengthMaximum = globalJumpStrengthHighestMaximum;
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
                //if (iteration % iterationThreshold == 0)
                //{
                //    yield return null;
                //}
            }
        }
        yield return null;
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

                for (float strength = Constants.AimStrengthSimulationStep; strength <= 1f; strength += Constants.AimStrengthSimulationStep)
                {
                    Vector2 jumpVector = direction * strength * _globalJumpStrengthHighestMaximum;
                    var destination = SimulateJumpAndCalculateDestination(startP.WorldPos, jumpVector, terrain);
                    foreach (var endP in endPoints) 
                    {
                        if (endP.Id == startP.Id || !endP.IsValid || endP.IsCornerPoint)
                            continue;

                        var delta = endP.WorldPos - destination;
                        var newJumpLink = new JumpLink(startP.Id, endP.Id, jumpVector);
                        if (Mathf.Abs(delta.x) < GridPointHalfDistance && Mathf.Abs(delta.y) < GridPointHalfDistance)
                        {
                            if (!_adjency[startP.Id].ContainsKey(endP.Id)) 
                            {
                                _adjency[startP.Id].Add(endP.Id, newJumpLink);
                                linkCount++;
                            }
                            else if (jumpVector.magnitude <= _globalJumpStrengthLowestMaximum)
                            {
                                var lastJumpVector = _adjency[startP.Id][endP.Id].JumpVector;
                                // prefer vertical and smaller jumps than the lowest maximum 
                                if (lastJumpVector.magnitude > _globalJumpStrengthLowestMaximum || lastJumpVector.normalized.y < jumpVector.normalized.y) 
                                {
                                    _adjency[startP.Id][endP.Id] = newJumpLink;
                                }
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

    #endregion

    #region Jump Simulation

    private Vector2 SimulateJumpAndCalculateDestination(Vector2 start, Vector2 jumpVector, DestructibleTerrainManager terrain)
    {
        Vector2 pos = start;
        var velocity = jumpVector;
        const float dt = Constants.ParabolicPathSimulationDeltaForMovement;
        bool collided = false;
        for (float t = 0; t < Constants.MaxParabolicPathSimulationTime && !collided; t += Constants.ParabolicPathSimulationDeltaForMovement)
        {
            pos += velocity * dt;
            velocity += Physics2D.gravity * dt;

            if (!terrain.IsPointInsideBounds(pos))
            {
                collided = true;
                break;
            };

            if (t >= CollisionCheckDelaySeconds)
            {
                foreach (var colliderPoint in CharacterColliderCornerPoints)
                {
                    if (terrain.OverlapPoint(pos + colliderPoint))
                    {
                        collided = true;
                        break;
                    }
                }
            }

        }
        return IsLandingVectorValid(velocity) ? pos : start;
    }

    private bool IsLandingVectorValid(Vector2 landingVector)
    {
        return landingVector.y < 0 && Mathf.Abs(landingVector.y) > Mathf.Abs(landingVector.x);
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
            StartCoroutine(AddNewStandingPointsAndJumpLinksAfterExplosion(explosionCenter, explosionRadius, terrain));
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

    private IEnumerator AddNewStandingPointsAndJumpLinksAfterExplosion(Vector2 explosionCenter, float explosionRadius, DestructibleTerrainManager terrain)
    {
        var searchRadius = explosionRadius + ExplosionRadiusOffsetForNewJumpLinks;
        var possibleLinkedPoints = _points.Where(p => p.IsValid && Vector2.Distance(p.WorldPos, explosionCenter) < searchRadius).ToList();

        UpdateCornerPoints(possibleLinkedPoints, terrain);

        var searchPoints = new List<Vector2>();

        if (explosionRadius >= ThreePointsExplosionRadiusThreshold)
        {
            var leftVector = (Vector2.down.ToAngleDegrees() - NewSearchPointAngularDistanceDegrees).AngleDegreesToVector() * explosionRadius;
            var rightVector = (Vector2.down.ToAngleDegrees() + NewSearchPointAngularDistanceDegrees).AngleDegreesToVector() * explosionRadius;
            searchPoints.Add(explosionCenter + Vector2.down * explosionRadius);
            searchPoints.Add(explosionCenter + leftVector);
            searchPoints.Add(explosionCenter + rightVector);
        }
        else if (explosionRadius >= TwoPointsExplosionRadiusThreshold)
        {
            var leftVector = (Vector2.down.ToAngleDegrees() - NewSearchPointAngularDistanceDegrees / 2f).AngleDegreesToVector() * explosionRadius;
            var rightVector = (Vector2.down.ToAngleDegrees() + NewSearchPointAngularDistanceDegrees / 2f).AngleDegreesToVector() * explosionRadius;
            searchPoints.Add(explosionCenter + leftVector);
            searchPoints.Add(explosionCenter + rightVector);
        }
        else
        {
            searchPoints.Add(explosionCenter + Vector2.down * explosionRadius);
        }

        foreach(var searchPoint in searchPoints)
        {
            yield return AddNewStandingPointAndJumpLinks(searchPoint, possibleLinkedPoints, terrain);
        }

        Debug.Log($"Jump graph update finished");
        _isReady = true;
    }
    

    private IEnumerator AddNewStandingPointAndJumpLinks(Vector2 searchPoint, IList<StandingPoint> possibleLinkedPoints, DestructibleTerrainManager terrain)
    {
        if (terrain.TryFindNearestStandingPoint(searchPoint, _pixelResolution / 2, _points.Count, out var newStandingPoint))
        {
            _points.Add(newStandingPoint);
            _adjency.Add(new Dictionary<int, JumpLink>());

            yield return SimulateJumpsFromGivenPointsAndCreateJumpLinks(possibleLinkedPoints, new StandingPoint[] { newStandingPoint }, terrain);
            yield return SimulateJumpsFromGivenPointsAndCreateJumpLinks(new StandingPoint[] { newStandingPoint }, _points, terrain);
        }
    }

    private void UpdateCornerPoints(IList<StandingPoint> points, DestructibleTerrainManager terrain)
    {
        for (int i = 0; i < points.Count; i++)
        {
            var p = points[i];
            if (p.IsValid && !p.IsCornerPoint && terrain.IsCornerPoint(p.PixelCoordinates))
            {
                _points[p.Id] = new StandingPoint(p.Id, p.WorldPos, p.PixelCoordinates, true);
                points[i] = _points[p.Id];
            }
        }
    }


    #endregion

    #region Path Search

    public void SetJumpStrength(float jumpStrength)
    {
        _currentJumpStrengthMaximum = jumpStrength;
    }

    private bool IsJumpPossible(JumpLink link)
    {
        return link.IsPossible(_currentJumpStrengthMaximum);
    }

    public bool TryCalculateJumpDistanceBetween(StandingPoint start, StandingPoint end, out int numJumps)
    {
        var path = FindShortestJumpPath(start, end);
        numJumps = path == null ? -1 : path.Count;
        return path != null; 
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

                if (!_points[nextId].IsValid || !IsJumpPossible(kvp.Value))
                    continue;

                visited.Add(nextId);
                queue.Enqueue(nextId);
            }
        }

        return reachable;
    }


    public StandingPoint FindClosestStandingPoint(Vector2 position)
    {
        var validPoints = _points.Where(p => p.IsValid);
        if (!validPoints.Any())
        {
            return StandingPoint.InvalidPoint;
        }
        var closestValidPointDistanceAndId = validPoints.Select(p => (Vector2.Distance(p.WorldPos, position), p.Id)).Min();
        return _points[closestValidPointDistanceAndId.Id];
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

                if (!IsJumpPossible(link))
                    continue;

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

    #region Correction

    public Vector2 CorrectJumpVector(Vector2 start, JumpLink jumpLink, DestructibleTerrainManager terrain)
    {
        var end = SimulateJumpAndCalculateDestination(start, jumpLink.JumpVector, terrain);
        var endPointPos = _points[jumpLink.ToId].WorldPos;
        var correctionOffset = endPointPos - end;
        if(TryCorrectJumpVector(start, end, jumpLink.JumpVector, correctionOffset, out var correctedJumpVector))
        {
            return correctedJumpVector;
        }
        else
        {
            return jumpLink.JumpVector;
        }
    }

    private bool TryCorrectJumpVector(Vector2 start, Vector2 end, Vector2 jumpVector, Vector2 correctionOffset, out Vector2 correctedJumpVector )
    {
        Vector2 g = Physics2D.gravity;

        correctedJumpVector = jumpVector;

        // Solve for time using vertical motion
        float a = 0.5f * g.y;
        float b = jumpVector.y;
        float c = start.y - end.y;

        float discriminant = b * b - 4f * a * c;
        if (discriminant < 0f)
            return false;

        float sqrt = Mathf.Sqrt(discriminant);

        // looking for positive time
        float t1 = (-b + sqrt) / (2f * a);
        float t2 = (-b - sqrt) / (2f * a);
        float t = Mathf.Max(t1, t2);

        if (t <= 0f)
            return false;

        // correction
        Vector2 deltaV = correctionOffset / t;
        correctedJumpVector = jumpVector + deltaV;
        if(correctedJumpVector.magnitude > _currentJumpStrengthMaximum)
        {
            correctedJumpVector = correctedJumpVector.normalized * _currentJumpStrengthMaximum;
        }
        return true;
    }


    #endregion

}
