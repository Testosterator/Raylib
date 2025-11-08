using Raylib_cs;
using System.Numerics;

public class WeaponHolster
{
    public bool IsWeaponDrawn { get; private set; } = false;
    public Weapon? EquippedWeapon;

    private Rectangle area;
    private Rectangle weaponSlot;
    private bool isDraggingWeapon = false;
    private Vector2 dragOffset;
    private Vector2 weaponPosition;

    public WeaponHolster(Rectangle area)
    {
        this.area = area;
        weaponSlot = new Rectangle(area.X + 40, area.Y + 60, 120, 80);
        weaponPosition = new Vector2(weaponSlot.X, weaponSlot.Y);
    }

    public void Update()
    {
        Vector2 mouse = Raylib.GetMousePosition();

        // jeśli nie ma broni, nic nie rób
        if (EquippedWeapon == null)
            return;

        // wykryj kliknięcie na kaburze
        if (Raylib.IsMouseButtonPressed(MouseButton.Left) && Raylib.CheckCollisionPointRec(mouse, weaponSlot))
        {
            isDraggingWeapon = true;
            dragOffset = mouse - weaponPosition;
        }

        // przeciąganie
        if (isDraggingWeapon && Raylib.IsMouseButtonDown(MouseButton.Left))
        {
            weaponPosition = mouse - dragOffset;
        }

        // puszczenie
        if (isDraggingWeapon && Raylib.IsMouseButtonReleased(MouseButton.Left))
        {
            isDraggingWeapon = false;

            // jeśli przesunięto wystarczająco w górę → wyciągnięta broń
            if (weaponPosition.Y < area.Y + 10)
            {
                IsWeaponDrawn = true;
                weaponPosition = new Vector2(area.X + 80, area.Y - 60);
            }
            else
            {
                IsWeaponDrawn = false;
                weaponPosition = new Vector2(weaponSlot.X, weaponSlot.Y);
            }
        }

        // skrót: F – schowaj broń
        if (IsWeaponDrawn && Raylib.IsKeyPressed(KeyboardKey.F))
        {
            IsWeaponDrawn = false;
            weaponPosition = new Vector2(weaponSlot.X, weaponSlot.Y);
        }
    }

    public void Draw()
    {
        // tło kabury
        Raylib.DrawRectangleRec(area, Raylib.ColorAlpha(Color.DarkBrown, 0.4f));
        Raylib.DrawRectangleLinesEx(area, 2, Color.Brown);
        Raylib.DrawText("Kabura", (int)area.X + 10, (int)area.Y + 10, 20, Color.White);

        if (EquippedWeapon == null)
        {
            Raylib.DrawText("Pusta", (int)area.X + 60, (int)area.Y + 80, 18, Color.Gray);
            return;
        }

        // broń w kaburze / w dłoni
        Color weaponColor = IsWeaponDrawn ? Color.DarkGray : Color.Gray;
        Raylib.DrawRectangleV(weaponPosition, new Vector2(100, 40), weaponColor);
        Raylib.DrawRectangleLines((int)weaponPosition.X, (int)weaponPosition.Y, 100, 40, Color.Black);

        string state = IsWeaponDrawn ? "Wyciągnięta" : "Schowana";
        Raylib.DrawText(state, (int)area.X + 10, (int)area.Y + (int)area.Height - 25, 16, Color.White);
    }
}
