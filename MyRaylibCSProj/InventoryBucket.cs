using Raylib_cs;
using System.Numerics;

public class InventoryBucket
{
    public Rectangle Area;
    private List<BucketItem> items = new();

    public InventoryBucket(Rectangle area)
    {
        Area = area;
    }

    public void AddItem(DraggableObject obj)
    {
        BucketItem slot = new BucketItem
        {
            Obj = obj,
            Pos = new Vector2(
                Area.X + Raylib.GetRandomValue(10, (int)Area.Width - 20),
                Area.Y + Raylib.GetRandomValue(10, (int)Area.Height - 20)
            ),
            Rot = Raylib.GetRandomValue(-25, 25),
            Vel = Vector2.Zero
        };
        items.Add(slot);
    }

    public void RemoveItem(DraggableObject obj)
    {
        items.RemoveAll(x => x.Obj == obj);
    }

    public bool Contains(DraggableObject obj) => items.Any(x => x.Obj == obj);

    public void Update(List<DraggableObject> worldObjects)
    {
        float dt = Raylib.GetFrameTime();

        // 🔹 Grawitacja + opadanie
        for (int i = 0; i < items.Count; i++)
        {
            var s = items[i];
            s.Vel.Y += 100f * dt; // grawitacja
            s.Pos += s.Vel * dt;

            // podłoga
            float bottom = Area.Y + Area.Height - 16;
            if (s.Pos.Y > bottom)
            {
                s.Pos.Y = bottom;
                s.Vel.Y *= -0.2f;
                s.Vel.X *= 0.8f;
            }

            items[i] = s;
        }

        // 🔹 Kolizje / repulsja
        for (int i = 0; i < items.Count; i++)
        {
            for (int j = i + 1; j < items.Count; j++)
            {
                var a = items[i];
                var b = items[j];
                Vector2 diff = a.Pos - b.Pos;
                float dist = diff.Length();
                float minDist = (a.Obj.Size.X + b.Obj.Size.X) * 0.3f;

                if (dist > 0 && dist < minDist)
                {
                    Vector2 push = Vector2.Normalize(diff) * (minDist - dist) * 0.5f;
                    a.Pos += push;
                    b.Pos -= push;

                    items[i] = a;
                    items[j] = b;
                }
            }
        }

        // 🔹 Wyciąganie z wiadra (kliknięcie)
        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            Vector2 mouse = Raylib.GetMousePosition();

            for (int i = 0; i < items.Count; i++)
            {
                var s = items[i];
                Rectangle rect = new Rectangle(s.Pos.X - 10, s.Pos.Y - 10, 20, 20);

                if (Raylib.CheckCollisionPointRec(mouse, rect))
                {
                    s.Obj.Position = mouse;
                    worldObjects.Add(s.Obj);
                    items.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public void Draw()
    {
        Raylib.DrawRectangleRec(Area, Raylib.ColorAlpha(Color.Brown, 0.25f));
        Raylib.DrawRectangleLinesEx(Area, 2, Color.DarkBrown);

        foreach (var s in items)
        {
            Rectangle rect = new Rectangle(s.Pos.X, s.Pos.Y, s.Obj.Size.X * 0.5f, s.Obj.Size.Y * 0.5f);
            Vector2 origin = new Vector2(rect.Width / 2, rect.Height / 2);

            Raylib.DrawRectanglePro(rect, origin, s.Rot, s.Obj.Color);
            Raylib.DrawRectangleLines(
                (int)(rect.X - rect.Width / 2),
                (int)(rect.Y - rect.Height / 2),
                (int)rect.Width,
                (int)rect.Height,
                Color.Black
            );
        }
    }

    public bool CheckDrop(Vector2 mousePos) => Raylib.CheckCollisionPointRec(mousePos, Area);

    public List<BucketItem> GetAllItems()
    {
        return new List<BucketItem>(items);
    }
}



public struct BucketItem
{
    public DraggableObject Obj;
    public Vector2 Pos;
    public Vector2 Vel;
    public float Rot;
}
