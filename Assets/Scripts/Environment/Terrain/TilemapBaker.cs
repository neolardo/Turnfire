using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapBaker
{
    public static TerrainTexture Bake(Tilemap tilemap, int pixelsPerTile, int pixelsPerUnit)
    {
        var bounds = tilemap.cellBounds;
        int width = bounds.size.x * pixelsPerTile;
        int height = bounds.size.y * pixelsPerTile;

        var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        ClearTexture(texture);

        foreach (var pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile == null) continue;

            Sprite sprite = tilemap.GetSprite(pos);
            if (sprite == null) continue;

            var rect = sprite.textureRect;
            var readableTex = GetReadableTexture(sprite);
            var spritePixels = readableTex.GetPixels(
                (int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height
            );

            int px = (pos.x - bounds.xMin) * pixelsPerTile;
            int py = (pos.y - bounds.yMin) * pixelsPerTile;
            texture.SetPixels(px, py, (int)rect.width, (int)rect.height, spritePixels);
        }

        texture.Apply();

        Vector2 pivotOffset = (Vector2)((tilemap.CellToWorld(bounds.min) + tilemap.CellToWorld(bounds.max)) / 2f);
        return new TerrainTexture(texture, pivotOffset, pixelsPerUnit);
    }

    private static void ClearTexture(Texture2D texture)
    {
        Color[] clear = new Color[texture.width * texture.height];
        texture.SetPixels(clear);
    }

    private static Texture2D GetReadableTexture(Sprite sprite)
    {
        var src = sprite.texture;
        RenderTexture tmp = RenderTexture.GetTemporary(src.width, src.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.sRGB);
        Graphics.Blit(src, tmp);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = tmp;

        Texture2D readableTex = new Texture2D(src.width, src.height, TextureFormat.RGBA32, false, false);
        readableTex.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
        readableTex.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(tmp);

        return readableTex;
    }
}
