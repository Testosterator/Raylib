using Raylib_cs;

public class Cylinder
{
    public Item?[] Chambers = new Item?[6];
    private int currentChamber = 0;

    public void Spin()
    {
        currentChamber = Raylib.GetRandomValue(0, 5);
    }

    public bool Fire()
    {
        if (Chambers[currentChamber] != null)
        {
            Chambers[currentChamber] = null;
            currentChamber = (currentChamber + 1) % 6;
            return true;
        }
        else
        {
            currentChamber = (currentChamber + 1) % 6;
            return false;
        }
    }
}
