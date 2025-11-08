using Raylib_cs;
using System.Numerics;

public class DraggableObject
{
    public Vector2 Position;
    public Vector2 Size;
    public Color Color;
    public float Weight;
    public Item Item;

    public bool IsDragging { get; private set; }
    public bool ReleasedThisFrame { get; private set; } = false;
    public bool IsDroppedOnUI { get; set; } = false;

    private Vector2 dragOffset;
    private bool isReturning = false;
    private Vector2 returnTarget;
    private float returnSpeed = 5f;

    public DraggableObject(Item item, Vector2 position, Vector2 size, float weight)
    {
        Item = item;
        Position = position;
        Size = size;
        Weight = weight;
        Color = item.Color;
    }




    public void Update(Camera2D cam, World world, Program.Player player)
    {
        float delta = Raylib.GetFrameTime();
        ReleasedThisFrame = false;

        if (isReturning)
        {
            Position = Vector2.Lerp(Position, returnTarget, delta * returnSpeed);
            if (Vector2.Distance(Position, returnTarget) < 1f)
            {
                Position = returnTarget;
                isReturning = false;
            }
            return;
        }

        Vector2 mouseWorld = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), cam);
        Rectangle worldRect = new Rectangle(Position.X, Position.Y, Size.X, Size.Y);

        if (Raylib.IsMouseButtonPressed(MouseButton.Left) && Raylib.CheckCollisionPointRec(mouseWorld, worldRect))
        {
            IsDragging = true;
            dragOffset = mouseWorld - Position;
        }

        if (IsDragging)
        {
            Vector2 target = mouseWorld - dragOffset;
            Position = Vector2.Lerp(Position, target, 1f / Weight);

            Vector2 topLeft = cam.Target - cam.Offset / cam.Zoom;
            float left = topLeft.X;
            float top = topLeft.Y;
            float right = left + Raylib.GetScreenWidth() / cam.Zoom;
            float bottom = top + Raylib.GetScreenHeight() / cam.Zoom;

            Position.X = MathF.Max(left, MathF.Min(Position.X, right - Size.X));
            Position.Y = MathF.Max(top, MathF.Min(Position.Y, bottom - Size.Y));
        }

        if (IsDragging && Raylib.IsMouseButtonReleased(MouseButton.Left))
        {
            IsDragging = false;
            ReleasedThisFrame = true;
        }
    }

    public void HandlePostUIDrop(World world, Program.Player player)
    {
        if (ReleasedThisFrame && !IsDroppedOnUI)
        {
            returnTarget = FindDropSpot(world, player);
            isReturning = true;
        }

        ReleasedThisFrame = false;
        IsDroppedOnUI = false;
    }

    private Vector2 FindDropSpot(World world, Program.Player player)
    {
        int radius = 2;
        for (int tries = 0; tries < 16; tries++)
        {
            int ox = Raylib.GetRandomValue(-radius, radius);
            int oy = Raylib.GetRandomValue(-radius, radius);
            int tx = (int)player.Position.X + ox;
            int ty = (int)player.Position.Y + oy;

            if (tx < 0 || ty < 0 || tx >= world.Width || ty >= world.Height) continue;
            if (world.Map[tx, ty] == Program.TileType.Water) continue;

            return new Vector2(tx * World.TileSize, ty * World.TileSize);
        }
        return player.Position * World.TileSize;
    }

    public void OnDroppedOnUI()
    {
        IsDroppedOnUI = true;
        ForceHide(); // opcjonalnie — ukrywa obiekt, by nie był widoczny w świecie
    }

    public void ForceHide()
    {
        Position = new Vector2(-9999, -9999);
    }

    public void Draw()
    {
        Raylib.DrawRectangleV(Position, Size, Color);
        Raylib.DrawRectangleLines((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y, Color.Black);
    }
}
