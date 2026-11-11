using UnityEngine;

/// <summary>Muestra puntaje y mensaje de Game Over mediante IMGUI.</summary>
public class ScoreUI : MonoBehaviour
{
    private GameManager game;
    private GUIStyle scoreStyle;
    private GUIStyle messageStyle;

    public void Init(GameManager gameManager) => game = gameManager;

    private void OnGUI()
    {
        if (game == null) return;

        scoreStyle ??= new GUIStyle(GUI.skin.label) { fontSize = 28, fontStyle = FontStyle.Bold };
        messageStyle ??= new GUIStyle(GUI.skin.label)
        {
            fontSize = 40,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };

        GUI.Label(new Rect(20f, 15f, 400f, 50f), $"Puntos: {game.Score}", scoreStyle);

        if (!game.IsGameOver) return;

        string title = game.HasWon ? "Ganaste" : "Game Over";
        GUI.Label(new Rect(0f, 0f, Screen.width, Screen.height), $"{title}\nPresiona R para reiniciar", messageStyle);
    }
}
