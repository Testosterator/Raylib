using Raylib_cs;
using System.Numerics;

public class Hotbar
{
    public List<HotbarSlot> Slots = new();
    public const int SlotCount = 9;
    public const int SlotSize = 64;
    public const int Padding = 8;

    private HotbarSlot? hoveredSlot = null;

    public Hotbar()
    {
        float totalWidth = SlotCount * SlotSize + (SlotCount - 1) * Padding;
        float startX = (Raylib.GetScreenWidth() - totalWidth) / 2;
        float y = Raylib.GetScreenHeight() - SlotSize - 16;

        for (int i = 0; i < SlotCount; i++)
        {
            Rectangle rect = new Rectangle(startX + i * (SlotSize + Padding), y, SlotSize, SlotSize);
            Slots.Add(new HotbarSlot(rect, i));
        }
    }

    public void Update(List<DraggableObject> objects, Camera2D camera, Program.Player player, World world)
    {
        Vector2 mouseScreen = Raylib.GetMousePosition();
        hoveredSlot = null;

        // podświetlenie slotu
        foreach (var s in Slots)
            if (s.CheckDrop(mouseScreen))
                hoveredSlot = s;

        // przeciągnięcie obiektu ze świata na slot
        if (Raylib.IsMouseButtonReleased(MouseButton.Left))
        {
            foreach (var obj in objects)
            {
                if (!obj.ReleasedThisFrame) continue;
                foreach (var slot in Slots)
                {
                    if (slot.CheckDrop(mouseScreen))
                    {
                        slot.AssignedObject = obj;
                        obj.OnDroppedOnUI();
                        break;
                    }
                }
            }
        }

        // przeciągnięcie obiektu ze slotu z powrotem do świata
        if (hoveredSlot != null && hoveredSlot.AssignedObject != null && Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            DraggableObject obj = hoveredSlot.AssignedObject;
            hoveredSlot.AssignedObject = null;

            // umieść przed graczem
            Vector2 dropPos = player.Position * World.TileSize + new Vector2(World.TileSize, 0);
            obj.Position = dropPos;
        }

        // aktywacja klawiszami 1–9
        for (int i = 0; i < Slots.Count; i++)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.One + i))
            {
                var slot = Slots[i];
                if (slot.AssignedObject != null)
                {
                    // wyrzuć obiekt na ziemię przed graczem
                    Vector2 dropPos = player.Position * World.TileSize + new Vector2(World.TileSize, 0);
                    slot.AssignedObject.Position = dropPos;
                    slot.AssignedObject = null;
                }
            }
        }
    }

    public void Draw()
    {
        foreach (var s in Slots)
        {
            bool highlight = (hoveredSlot == s);
            s.Draw(highlight);
        }
    }
}
