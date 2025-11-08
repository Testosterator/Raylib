using Raylib_cs;

public class Ammo : Item
{
    public int Caliber { get; private set; }

    public Ammo(string name, int caliber, Color color)
        : base(name, ItemType.Ammo, color, 0.2f)
    {
        Caliber = caliber;
    }

    public override void Use()
    {
        // np. załadowanie do broni
    }
}
