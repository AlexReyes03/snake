# Plan: Juego Snake en Unity (2D)

## Reglas del juego

- La serpiente se mueve en una cuadrícula a intervalos fijos.
- Al comer comida (1 punto) la serpiente crece 1 segmento.
- Muere al chocar contra una pared (límite del tablero) o contra su propio cuerpo.
- Al morir se muestra "Game Over" y se puede reiniciar con una tecla.

## Estado inicial del proyecto

- Unity 2D con URP, escena `Assets/Scenes/SampleScene.unity`.
- Objetos en escena: `Main Camera`, `Global Light 2D`.
- Input System instalado (`com.unity.inputsystem`). Se usará `Keyboard.current` para evitar configurar un asset de acciones.
- MCP `unityMCP` disponible para crear objetos, scripts y componentes desde el agente.

## Decisiones de diseño

| Elemento | Decisión |
|---|---|
| Tablero | 20 x 20 celdas, 1 unidad Unity por celda, centrado en el origen |
| Movimiento | Por pasos cada 0.15 s (`stepInterval`), sin física |
| Colisiones | Por comparación de coordenadas enteras (sin Rigidbody ni Collider) |
| Gráficos | Sprites cuadrados generados en runtime (sin assets externos) |
| Puntos | 1 comida = 1 punto = 1 segmento nuevo |

## Estructura de archivos

```
Assets/
  Scripts/
    GameManager.cs     // estado, puntaje, game over, reinicio
    SnakeController.cs // movimiento, crecimiento, colisiones
    FoodSpawner.cs     // aparición de comida en celda libre
    BoardBuilder.cs    // dibuja bordes del tablero
    ScoreUI.cs         // texto de puntaje y Game Over
  Sprites/
    Square.png         // sprite blanco 1x1 (o generado por código)
  Scenes/
    SampleScene.unity
```

## Pasos

### Paso 1: Preparar la escena
1. Abrir `SampleScene`.
2. Configurar `Main Camera`: Projection `Orthographic`, `Size = 11`, posición `(0, 0, -10)`, fondo gris oscuro.
3. Crear la carpeta `Assets/Scripts`.

### Paso 2: Sprite base
1. Crear un sprite cuadrado blanco (`Assets/Sprites/Square.png`, 16x16 px, Pixels Per Unit = 16) o usar el sprite integrado `Square` de Unity.
2. Se reutilizará para cabeza, cuerpo, comida y paredes, cambiando solo el color.

### Paso 3: Tablero y paredes (`BoardBuilder`)
1. Definir constantes `Width = 20`, `Height = 20`.
2. Dibujar los 4 bordes con sprites de color gris claro en las celdas fuera del rango `0..Width-1` / `0..Height-1`.
3. Exponer método `bool IsInside(Vector2Int cell)`.

### Paso 4: Serpiente (`SnakeController`)
1. Mantener `List<Vector2Int> segments` (índice 0 = cabeza) y `Vector2Int direction`.
2. Leer entrada con `Keyboard.current` (flechas y WASD). Ignorar el giro de 180 grados.
3. Usar un temporizador: cada `stepInterval` ejecutar `Step()`:
   - Calcular `newHead = segments[0] + direction`.
   - Si `!board.IsInside(newHead)` o `segments.Contains(newHead)` -> `GameManager.GameOver()`.
   - Insertar `newHead` en índice 0.
   - Si `newHead == food.Cell` -> sumar 1 punto y pedir nueva comida (no eliminar la cola; la serpiente crece 1).
   - En otro caso eliminar el último segmento.
4. Sincronizar los `GameObject` visuales con la lista (pool simple: crear los que falten, mover los existentes).

> Nota: al evaluar colisión con el cuerpo, excluir la cola actual si no se va a crecer en este paso; es una mejora opcional.

### Paso 5: Comida (`FoodSpawner`)
1. Crear un `GameObject` rojo de 1x1.
2. `Spawn(HashSet<Vector2Int> occupied)`: elegir una celda aleatoria dentro del tablero que no esté ocupada por la serpiente.
3. Si no quedan celdas libres, declarar victoria (caso borde).

### Paso 6: Estado del juego (`GameManager`)
1. Campos: `int score`, `bool isGameOver`.
2. `AddScore()` incrementa en 1 y notifica a `ScoreUI`.
3. `GameOver()` detiene el movimiento y muestra el mensaje.
4. En Game Over, `Space` o `R` recarga la escena con `SceneManager.LoadScene`.

### Paso 7: Interfaz (`ScoreUI`)
1. Crear `Canvas` (Screen Space - Overlay) con dos `TextMeshPro - Text (UI)`:
   - `ScoreText` (esquina superior izquierda): "Puntos: 0".
   - `GameOverText` (centro, desactivado): "Game Over - Presiona R".
2. `ScoreUI` actualiza ambos textos desde `GameManager`.

### Paso 8: Ensamblar en escena
1. Crear un `GameObject` vacío `Game` y añadir `GameManager`, `BoardBuilder`, `SnakeController` y `FoodSpawner`.
2. Asignar referencias en el Inspector (UI y sprite).
3. Guardar la escena.

### Paso 9: Pruebas manuales
| Caso | Resultado esperado |
|---|---|
| Comer comida | +1 punto, +1 segmento, nueva comida en celda libre |
| Salir del tablero por cualquier lado | Game Over |
| Girar sobre el propio cuerpo | Game Over |
| Presionar la dirección opuesta | Se ignora |
| Presionar R en Game Over | Juego reiniciado con puntaje 0 |

### Paso 10: Pulido opcional
- Aumentar velocidad cada N puntos.
- Guardar récord con `PlayerPrefs`.
- Sonidos de comer y de muerte.

## Orden de ejecución sugerido con el MCP

1. `manage_camera` / `manage_gameobject`: configurar cámara.
2. `create_script`: crear los 5 scripts.
3. `refresh_unity` y `read_console`: verificar compilación sin errores.
4. `manage_gameobject` / `manage_components`: crear `Game` y asignar componentes.
5. `manage_ui`: crear Canvas y textos.
6. `manage_editor` (`play`) y `read_console`: probar y revisar errores.
7. `manage_scene` (`save`): guardar la escena.
