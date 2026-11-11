using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>Punto de entrada: arma el juego, lleva el puntaje y gestiona Game Over y reinicio.</summary>
public class GameManager : MonoBehaviour
{
    public int Score { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool HasWon { get; private set; }

    private void Awake()
    {
        var board = gameObject.AddComponent<BoardBuilder>();
        board.Build();

        var food = gameObject.AddComponent<FoodSpawner>();
        var snake = gameObject.AddComponent<SnakeController>();
        snake.Init(this, food);

        gameObject.AddComponent<ScoreUI>().Init(this);
    }

    private void Update()
    {
        if (!IsGameOver) return;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.rKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void AddScore() => Score++;

    public void EndGame(bool won)
    {
        IsGameOver = true;
        HasWon = won;
    }
}
