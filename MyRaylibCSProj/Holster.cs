using Raylib_cs;
using System.Numerics;

public class Holster
{
    public Weapon? EquippedWeapon;
    public Cylinder Cylinder = new Cylinder();

    private Rectangle area;
    private float rotationLogic = 0f;   // faktyczny obrót bębenka
    private float rotationVisual = 0f;  // obrót wizualny (animacja)
    private bool spinning = false;
    private float spinSpeed = 0f;
    private float spinTotal = 0f;
    private float spinTarget = 0f;

    public Holster(Rectangle hudArea)
    {
        area = hudArea;
    }

    public void Draw()
    {
        if (EquippedWeapon == null) return;

        // --- tło kabury ---
        Raylib.DrawRectangleRec(area, Raylib.ColorAlpha(Color.DarkBrown, 0.4f));
        Raylib.DrawRectangleLinesEx(area, 2, Color.Brown);
        Raylib.DrawText("Cylinder", (int)area.X + 10, (int)area.Y + 10, 20, Color.White);

        Vector2 center = new Vector2(area.X + area.Width / 2, area.Y + area.Height / 2 + 20);
        float radius = 60f;

        // --- animacja obrotu ---
        if (spinning)
        {
            float dt = Raylib.GetFrameTime();
            rotationVisual += spinSpeed * dt;
            spinTotal += spinSpeed * dt;

            // spowolnienie po przekroczeniu celu
            if (spinTotal >= spinTarget)
            {
                spinning = false;
                rotationLogic += spinTarget; // zaktualizuj logiczny kąt po zakończeniu
                rotationVisual = rotationLogic;
            }
        }

        // --- rysowanie 6 komór ---
        for (int i = 0; i < 6; i++)
        {
            float angle = rotationVisual + i * (MathF.PI * 2 / 6);
            Vector2 pos = new Vector2(center.X + MathF.Cos(angle) * radius, center.Y + MathF.Sin(angle) * radius);
            bool loaded = Cylinder.Chambers[i] != null;
            Color color = loaded ? Color.Gold : Color.Gray;

            Raylib.DrawCircleV(pos, 15, Raylib.ColorAlpha(color, 0.7f));
            Raylib.DrawCircleLines((int)pos.X, (int)pos.Y, 15, Color.Black);
        }

        // --- przycisk zakręcenia ---
        Rectangle spinButton = new Rectangle(area.X + 10, area.Y + area.Height - 40, 80, 30);
        Raylib.DrawRectangleRec(spinButton, Color.DarkGray);
        Raylib.DrawText("Zakręć", (int)spinButton.X + 5, (int)spinButton.Y + 7, 18, Color.White);

        if (Raylib.IsMouseButtonPressed(MouseButton.Left) &&
            Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), spinButton) &&
            !spinning)
        {
            Cylinder.Spin();

            spinning = true;
            spinSpeed = 12f;             // prędkość
            spinTotal = 0f;              // aktualny obrót
            spinTarget = (MathF.PI * 4); // 2 pełne obroty
        }
    }

    public void Update(InventoryBucket bucket)
    {
        if (EquippedWeapon == null || spinning) return; // ⛔ nie wkładamy podczas kręcenia

        Vector2 mouse = Raylib.GetMousePosition();

        if (Raylib.IsMouseButtonReleased(MouseButton.Left))
        {
            foreach (var bucketItem in bucket.GetAllItems())
            {
                var draggable = bucketItem.Obj;
                if (draggable.Item is not Ammo ammo)
                    continue;

                Vector2 center = new Vector2(area.X + area.Width / 2, area.Y + area.Height / 2 + 20);

                // sprawdzamy 6 miejsc (bazując na rotationLogic)
                for (int i = 0; i < 6; i++)
                {
                    float angle = rotationLogic + i * (MathF.PI * 2 / 6);
                    Vector2 pos = new Vector2(center.X + MathF.Cos(angle) * 60f, center.Y + MathF.Sin(angle) * 60f);
                    Rectangle chamber = new Rectangle(pos.X - 15, pos.Y - 15, 30, 30);

                    if (Raylib.CheckCollisionPointRec(mouse, chamber) && Cylinder.Chambers[i] == null)
                    {
                        Cylinder.Chambers[i] = ammo;
                        bucket.RemoveItem(draggable);
                        return;
                    }
                }
            }
        }
    }
}
