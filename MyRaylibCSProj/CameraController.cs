using Raylib_cs;
using System.Numerics;
using System;

public class CameraController
{
    public Camera2D Cam;

    private bool isDragging = false;
    private Vector2 dragStart;
    private Vector2 initialTarget;

    public float ZoomSpeed = 0.1f;
    public float MinZoom = 0.3f;
    public float MaxZoom = 2.5f;
    public float SmoothSpeed = 0.1f; // 🔹 im mniejsza, tym wolniej dogania (0.05 - 0.2 fajne zakresy)

    private Vector2 targetPos;

    private int worldWidth;
    private int worldHeight;

    public CameraController(Vector2 startPos, int worldWidth, int worldHeight)
    {
        this.worldWidth = worldWidth;
        this.worldHeight = worldHeight;

        Cam = new Camera2D
        {
            Target = startPos,
            Offset = new Vector2(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2),
            Rotation = 0f,
            Zoom = 1f
        };

        targetPos = startPos;
    }

    /// <summary>
    /// Ustawia pozycję, którą kamera ma śledzić (np. gracza)
    /// </summary>
    public void Follow(Vector2 playerPos)
    {
        if (!isDragging)
            targetPos = playerPos;
    }

    /// <summary>
    /// Płynna aktualizacja kamery (z opóźnieniem)
    /// </summary>
    public void Update(float deltaTime)
    {
        // 🔹 Zoom
        float wheel = Raylib.GetMouseWheelMove();
        if (wheel != 0)
        {
            Cam.Zoom = MathF.Exp(MathF.Log(Cam.Zoom) + (wheel * ZoomSpeed));
            Cam.Zoom = Math.Clamp(Cam.Zoom, MinZoom, MaxZoom);
        }

        // 🔹 Drag
        Vector2 mouse = Raylib.GetMousePosition();
        if (Raylib.IsMouseButtonPressed(MouseButton.Middle))
        {
            isDragging = true;
            dragStart = mouse;
            initialTarget = Cam.Target;
        }
        if (Raylib.IsMouseButtonReleased(MouseButton.Middle))
            isDragging = false;

        if (isDragging)
        {
            Vector2 delta = (dragStart - mouse) * (1 / Cam.Zoom);
            targetPos = initialTarget + delta;
        }

        // 🔹 Smooth follow (interpolacja)
        Cam.Target = Vector2.Lerp(Cam.Target, targetPos, SmoothSpeed * 60f * deltaTime);

        // 🔹 Ograniczenia (kamera nie wychodzi poza mapę)
        ClampToWorldBounds();
    }

    private void ClampToWorldBounds()
    {
        float halfWidth = (Raylib.GetScreenWidth() / 2f) / Cam.Zoom;
        float halfHeight = (Raylib.GetScreenHeight() / 2f) / Cam.Zoom;

        float minX = halfWidth;
        float maxX = worldWidth * World.TileSize - halfWidth;
        float minY = halfHeight;
        float maxY = worldHeight * World.TileSize - halfHeight;

        Cam.Target = new Vector2(
            Math.Clamp(Cam.Target.X, minX, maxX),
            Math.Clamp(Cam.Target.Y, minY, maxY)
        );
    }

    public void Begin() => Raylib.BeginMode2D(Cam);
    public void End() => Raylib.EndMode2D();
}
