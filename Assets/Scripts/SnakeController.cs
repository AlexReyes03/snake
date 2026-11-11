using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Movimiento por pasos, crecimiento y colisiones de la serpiente.</summary>
public class SnakeController : MonoBehaviour
{
    [SerializeField] private float stepInterval = 0.15f;

    private readonly List<Vector2Int> segments = new List<Vector2Int>();
    private readonly List<Transform> views = new List<Transform>();

    private Vector2Int direction = Vector2Int.right;
    private Vector2Int pendingDirection = Vector2Int.right;
    private float timer;
    private bool initialized;

    private GameManager game;
    private FoodSpawner food;

    private static readonly Color HeadColor = new Color(0.4f, 1f, 0.4f);
    private static readonly Color BodyColor = new Color(0.15f, 0.7f, 0.2f);

    public void Init(GameManager gameManager, FoodSpawner foodSpawner)
    {
        game = gameManager;
        food = foodSpawner;

        int startX = BoardBuilder.Width / 2;
        int startY = BoardBuilder.Height / 2;
        segments.Clear();
        for (int i = 0; i < 3; i++) segments.Add(new Vector2Int(startX - i, startY));

        food.Spawn(segments);
        SyncViews();
        initialized = true;
    }

    private void Update()
    {
        if (!initialized || game.IsGameOver) return;

        ReadInput();

        timer += Time.deltaTime;
        if (timer < stepInterval) return;

        timer -= stepInterval;
        Step();
    }

    private void ReadInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.upArrowKey.wasPressedThisFrame || keyboard.wKey.wasPressedThisFrame) TrySetDirection(Vector2Int.up);
        else if (keyboard.downArrowKey.wasPressedThisFrame || keyboard.sKey.wasPressedThisFrame) TrySetDirection(Vector2Int.down);
        else if (keyboard.leftArrowKey.wasPressedThisFrame || keyboard.aKey.wasPressedThisFrame) TrySetDirection(Vector2Int.left);
        else if (keyboard.rightArrowKey.wasPressedThisFrame || keyboard.dKey.wasPressedThisFrame) TrySetDirection(Vector2Int.right);
    }

    private void TrySetDirection(Vector2Int requested)
    {
        // Se compara contra la direccion realmente aplicada para evitar giros de 180 grados.
        if (requested + direction == Vector2Int.zero) return;
        pendingDirection = requested;
    }

    private void Step()
    {
        direction = pendingDirection;
        var newHead = segments[0] + direction;
        bool grow = newHead == food.Cell;

        // Si no crece, la cola se libera en este paso y no cuenta como colision.
        int bodyLength = grow ? segments.Count : segments.Count - 1;
        bool hitsSelf = false;
        for (int i = 0; i < bodyLength; i++)
        {
            if (segments[i] != newHead) continue;
            hitsSelf = true;
            break;
        }

        if (!BoardBuilder.IsInside(newHead) || hitsSelf)
        {
            game.EndGame(false);
            return;
        }

        segments.Insert(0, newHead);

        if (grow)
        {
            game.AddScore();
            if (!food.Spawn(segments))
            {
                SyncViews();
                game.EndGame(true);
                return;
            }
        }
        else
        {
            segments.RemoveAt(segments.Count - 1);
        }

        SyncViews();
    }

    private void SyncViews()
    {
        while (views.Count < segments.Count)
        {
            var cell = BoardBuilder.CreateCell("Segment", BodyColor, 2, transform);
            cell.transform.localScale = Vector3.one * 0.9f;
            views.Add(cell.transform);
        }

        for (int i = 0; i < segments.Count; i++)
        {
            views[i].position = BoardBuilder.CellToWorld(segments[i]);
            views[i].GetComponent<SpriteRenderer>().color = i == 0 ? HeadColor : BodyColor;
        }
    }
}
