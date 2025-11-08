using MyRaylibCSProj;
using Raylib_cs;

public class World
{
    public int Width;
    public int Height;
    public Program.TileType[,] Map;
    public const int TileSize = 32;

    public int Seed { get; private set; }

    public World(int width, int height)
    {
        Width = width;
        Height = height;
        Map = new Program.TileType[width, height];
        Generate();
    }

    public void Generate(float scale = 0.05f, int? seed = null)
    {
        Seed = seed ?? Guid.NewGuid().GetHashCode();
        Random rand = new Random(Seed);

        float offsetX = rand.Next(0, 10000);
        float offsetY = rand.Next(0, 10000);

        // Ustawienia wysp
        float centerX = Width / 2f;
        float centerY = Height / 2f;
        float maxDistance = MathF.Sqrt(centerX * centerX + centerY * centerY);

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                // Szum bazowy
                double noiseValue = Noise.Perlin((x + offsetX) * scale, (y + offsetY) * scale);

                // 🔹 Maska "wyspy" — im dalej od środka, tym więcej wody
                float dx = x - centerX;
                float dy = y - centerY;
                float distance = MathF.Sqrt(dx * dx + dy * dy);
                float distanceFactor = distance / maxDistance; // 0 (środek) → 1 (krawędź)

                // Odejmujemy wpływ odległości — brzegi to woda
                noiseValue -= distanceFactor * 0.7f;

                // 🔹 Normalizacja
                noiseValue = Math.Clamp(noiseValue, 0, 1);

                // 🔹 Wybór biomu
                if (noiseValue < 0.3)
                    Map[x, y] = Program.TileType.Water;
                else if (noiseValue < 0.4)
                    Map[x, y] = Program.TileType.Sand;
                else if (noiseValue < 0.65)
                    Map[x, y] = Program.TileType.Grass;
                else if (noiseValue < 0.8)
                    Map[x, y] = Program.TileType.ForestLeafy;
                else
                    Map[x, y] = Program.TileType.Snow;
            }
        }

        // 🔹 Pasek wody na obrzeżach mapy (margines bezpieczeństwa)
        AddWaterBorder(4);
    }

    private void AddWaterBorder(int thickness)
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (x < thickness || y < thickness || x >= Width - thickness || y >= Height - thickness)
                {
                    Map[x, y] = Program.TileType.Water;
                }
            }
        }
    }

    //public void Draw()
    //{
    //    int tileWidth = 64;   // szerokość kafelka w izometrii
    //    int tileHeight = 32;  // wysokość kafelka (połowa szerokości daje ładny efekt)

    //    for (int y = 0; y < Height; y++)
    //    {
    //        for (int x = 0; x < Width; x++)
    //        {
    //            Color color = Map[x, y] switch
    //            {
    //                Program.TileType.Water => Color.Blue,
    //                Program.TileType.Sand => Color.Beige,
    //                Program.TileType.Grass => Color.Green,
    //                Program.TileType.ForestLeafy => Color.DarkGreen,
    //                Program.TileType.Snow => Color.LightGray,
    //                Program.TileType.TreeLeafy => Color.DarkGreen,
    //                Program.TileType.TreeCactus => Color.Brown,
    //                Program.TileType.TreePine => Color.DarkGreen,
    //                Program.TileType.Bridge => Color.Brown,
    //                _ => Color.Gray
    //            };

    //            // 📐 przeliczenie współrzędnych 2D na izometryczne
    //            int isoX = (int)((x - y) * (tileWidth / 2));
    //            int isoY = (int)((x + y) * (tileHeight / 2));

    //            // przesunięcie mapy, żeby środek był w ekranie
    //            int offsetX = Width * tileWidth / 4;
    //            int offsetY = 100;

    //            // rysowanie rombu (cztery punkty)
    //            Vector2 top = new Vector2(isoX + offsetX, isoY + offsetY);
    //            Vector2 left = new Vector2(isoX - tileWidth / 2 + offsetX, isoY + tileHeight / 2 + offsetY);
    //            Vector2 right = new Vector2(isoX + tileWidth / 2 + offsetX, isoY + tileHeight / 2 + offsetY);
    //            Vector2 bottom = new Vector2(isoX + offsetX, isoY + tileHeight + offsetY);

    //            Raylib.DrawTriangle(top, left, right, color);
    //            Raylib.DrawTriangle(left, bottom, right, color);
    //            Raylib.DrawLineV(top, left, Color.Black);
    //            Raylib.DrawLineV(left, bottom, Color.Black);
    //            Raylib.DrawLineV(bottom, right, Color.Black);
    //            Raylib.DrawLineV(right, top, Color.Black);
    //        }
    //    }
    //}


    public void Draw()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Color color = Map[x, y] switch
                {
                    Program.TileType.Water => Color.Blue,
                    Program.TileType.Sand => Color.Beige,
                    Program.TileType.Grass => Color.Green,
                    Program.TileType.ForestLeafy => Color.DarkGreen,
                    Program.TileType.Snow => Color.LightGray,
                    Program.TileType.TreeLeafy => Color.DarkGreen,
                    Program.TileType.TreeCactus => Color.Brown,
                    Program.TileType.TreePine => Color.DarkGreen,
                    Program.TileType.Bridge => Color.Brown,
                    _ => Color.Gray
                };

                Raylib.DrawRectangle(x * TileSize, y * TileSize, TileSize, TileSize, color);
            }
        }
    }
}
