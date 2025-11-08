using Raylib_cs;

public class Weapon : Item
{
    public int Damage { get; private set; }
    public int Capacity { get; private set; }

    public Weapon(string name, int damage, int capacity, Color color)
        : base(name, ItemType.Weapon, color, 2f)
    {
        Damage = damage;
        Capacity = capacity;
    }

    public override void Use() { }
}
