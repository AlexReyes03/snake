using UnityEngine;

/// <summary>Construye el tablero, las paredes y expone utilidades de celdas.</summary>
public class BoardBuilder : MonoBehaviour
{
    public const int Width = 20;
    public const int Height = 20;

    private static Sprite cellSprite;

    public static Sprite CellSprite
    {
        get
        {
            if (cellSprite != null) return cellSprite;

            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point
            };
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            cellSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
            return cellSprite;
        }
    }

    public static bool IsInside(Vector2Int cell) =>
        cell.x >= 0 && cell.x < Width && cell.y >= 0 && cell.y < Height;

    public static Vector3 CellToWorld(Vector2Int cell) =>
        new Vector3(cell.x - (Width - 1) / 2f, cell.y - (Height - 1) / 2f, 0f);

    public static GameObject CreateCell(string objectName, Color color, int sortingOrder, Transform parent)
    {
        var go = new GameObject(objectName);
        go.transform.SetParent(parent, false);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = CellSprite;
        sr.color = color;
        sr.sortingOrder = sortingOrder;
        return go;
    }

    public void Build()
    {
        ConfigureCamera();

        var background = CreateCell("Background", new Color(0.1f, 0.1f, 0.12f), -10, transform);
        background.transform.localScale = new Vector3(Width, Height, 1f);

        var wallColor = new Color(0.75f, 0.75f, 0.8f);
        for (int x = -1; x <= Width; x++)
        {
            CreateWall(new Vector2Int(x, -1), wallColor);
            CreateWall(new Vector2Int(x, Height), wallColor);
        }

        for (int y = 0; y < Height; y++)
        {
            CreateWall(new Vector2Int(-1, y), wallColor);
            CreateWall(new Vector2Int(Width, y), wallColor);
        }
    }

    private void CreateWall(Vector2Int cell, Color color)
    {
        var wall = CreateCell("Wall", color, -5, transform);
        wall.transform.position = CellToWorld(cell);
    }

    private static void ConfigureCamera()
    {
        var cam = Camera.main;
        if (cam == null) return;

        cam.orthographic = true;
        cam.orthographicSize = Height / 2f + 2f;
        cam.transform.position = new Vector3(0f, 0f, -10f);
        cam.backgroundColor = new Color(0.05f, 0.05f, 0.06f);
    }
}
