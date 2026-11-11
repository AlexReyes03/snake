using System.Collections.Generic;
using UnityEngine;

/// <summary>Gestiona la comida y su aparicion en celdas libres.</summary>
public class FoodSpawner : MonoBehaviour
{
    private GameObject foodObject;

    public Vector2Int Cell { get; private set; } = new Vector2Int(-1, -1);

    /// <summary>Coloca la comida en una celda libre. Devuelve false si no quedan celdas.</summary>
    public bool Spawn(IReadOnlyList<Vector2Int> occupied)
    {
        var free = new List<Vector2Int>();
        for (int x = 0; x < BoardBuilder.Width; x++)
        {
            for (int y = 0; y < BoardBuilder.Height; y++)
            {
                var cell = new Vector2Int(x, y);
                if (!Contains(occupied, cell)) free.Add(cell);
            }
        }

        if (free.Count == 0) return false;

        if (foodObject == null)
        {
            foodObject = BoardBuilder.CreateCell("Food", new Color(0.9f, 0.2f, 0.2f), 1, transform);
            foodObject.transform.localScale = Vector3.one * 0.8f;
        }

        Cell = free[Random.Range(0, free.Count)];
        foodObject.transform.position = BoardBuilder.CellToWorld(Cell);
        return true;
    }

    private static bool Contains(IReadOnlyList<Vector2Int> list, Vector2Int cell)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == cell) return true;
        }

        return false;
    }
}
