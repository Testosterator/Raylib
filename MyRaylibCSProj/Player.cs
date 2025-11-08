using Raylib_cs;
using System.Numerics;

public partial class Program
{
    public class Player
    {
        public Vector2 Position;

        public void Spawn(World world)
        {
            for (int y = 0; y < world.Height; y++)
                for (int x = 0; x < world.Width; x++)
                    if (world.Map[x, y] == TileType.Grass)
                    {
                        Position = new Vector2(x, y);
                        return;
                    }
        }

        public void Update(World world)
        {
            Vector2 newPos = Position;

            if (Raylib.IsKeyPressed(KeyboardKey.Right) || Raylib.IsKeyPressed(KeyboardKey.D))
                newPos.X++;
            if (Raylib.IsKeyPressed(KeyboardKey.Left) || Raylib.IsKeyPressed(KeyboardKey.A))
                newPos.X--;
            if (Raylib.IsKeyPressed(KeyboardKey.Down) || Raylib.IsKeyPressed(KeyboardKey.S))
                newPos.Y++;
            if (Raylib.IsKeyPressed(KeyboardKey.Up) || Raylib.IsKeyPressed(KeyboardKey.W))
                newPos.Y--;

            if (newPos.X >= 0 && newPos.Y >= 0 && newPos.X < world.Width && newPos.Y < world.Height)
            {
                if (world.Map[(int)newPos.X, (int)newPos.Y] != TileType.Water)
                    Position = newPos;
            }
        }

        public void Draw()
        {
            Raylib.DrawRectangle(
                (int)Position.X * World.TileSize + 4,
                (int)Position.Y * World.TileSize + 4,
                World.TileSize - 8,
                World.TileSize - 8,
                Color.Red
            );
        }
    }
}
