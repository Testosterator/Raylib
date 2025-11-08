using Raylib_cs;
using System.Numerics;

public class HotbarSlot
{
    public Rectangle Area;
    public DraggableObject? AssignedObject;
    public int Index;

    public HotbarSlot(Rectangle area, int index)
    {
        Area = area;
        Index = index;
    }

    public void Draw(bool highlight = false)
    {
        Color baseColor = highlight ? Color.DarkGray : Color.LightGray;
        Raylib.DrawRectangleRec(Area, Raylib.ColorAlpha(baseColor, 0.4f));
        Raylib.DrawRectangleLinesEx(Area, 2, Color.DarkGray);
        Raylib.DrawText((Index + 1).ToString(), (int)Area.X + 8, (int)Area.Y + 8, 16, Color.DarkGray);

        if (AssignedObject != null)
        {
            Vector2 center = new Vector2(Area.X + Area.Width / 2, Area.Y + Area.Height / 2);
            Vector2 iconSize = AssignedObject.Size * 0.4f;
            Vector2 drawPos = center - iconSize / 2;
            Raylib.DrawRectangleV(drawPos, iconSize, AssignedObject.Color);
            Raylib.DrawRectangleLines((int)drawPos.X, (int)drawPos.Y, (int)iconSize.X, (int)iconSize.Y, Color.Black);
        }
    }

    public bool CheckDrop(Vector2 mousePos) => Raylib.CheckCollisionPointRec(mousePos, Area);
}
