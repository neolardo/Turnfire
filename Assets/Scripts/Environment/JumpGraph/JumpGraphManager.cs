using UnityEngine;

public class JumpGraphManager : MonoBehaviour
{
    [SerializeField] private PixelUIDefinition _pixelUI;
    [SerializeField] private TerrainManager _terrain;
    [SerializeField] private float _characterHeight = 1f;
    [SerializeField] private float _characterWidth = .5f;
    private int PixelResolution => _pixelUI.PixelsPerUnit / 4;
    public JumpGraph JumpGraph { get; private set; }

    private void Start()
    {
        JumpGraph = new JumpGraph(this, PixelResolution, _pixelUI.PixelsPerUnit, Constants.MaxJumpStrength, Constants.DefaultJumpStrength, _characterWidth, _characterHeight);
        JumpGraph.InitiateGraphCreationFromTerrain(_terrain);
        _terrain.TerrainModifiedByExplosion += OnTerrainModifiedByExplosion;
    }

    private void OnTerrainModifiedByExplosion(Vector2 position, float radius)
    {
        JumpGraph.ApplyExplosion(position, radius, _terrain);
    }

}
