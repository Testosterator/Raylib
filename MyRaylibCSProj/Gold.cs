using Raylib_cs;

public class Gold : Item
{
    public int Value { get; private set; }

    public Gold(int value)
        : base("Złoto", ItemType.Gold, Color.Yellow, 0.5f)
    {
        Value = value;
    }

    public override void Use()
    {
        // np. zwiększenie stanu konta
    }
}
