using Raylib_cs;

public enum ItemType
{
    Ammo,
    Gold,
    Weapon
}

public class Item
{
    public string Name;
    public ItemType Type;
    public Color Color;
    public float Weight;

    public Item(string name, ItemType type, Color color, float weight = 1f)
    {
        Name = name;
        Type = type;
        Color = color;
        Weight = weight;
    }

    public virtual void Use() { }
}
