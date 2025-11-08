using Raylib_cs;
using System.Numerics;


public partial class Program
{
    public enum TileType
    {
        Water,
        Grass,
        Sand,
        Snow,
        TreeLeafy,
        TreeCactus,
        TreePine,
        ForestLeafy,
        Bridge
    }

    static void Main()
    {
        Raylib.InitWindow(1280, 800, "Alchemia: Odkrywanie Kart");
        Raylib.SetTargetFPS(60);

        World world = new World(200, 200);
        Player player = new Player();
        player.Spawn(world);
        Hotbar hotbar = new Hotbar();

        CardManager cards = new CardManager();

        CameraController camera = new CameraController(player.Position * World.TileSize, world.Width, world.Height);


        List<DraggableObject> objects = new List<DraggableObject>()
        {
            new DraggableObject(new Ammo("Nabój .38", 38, Color.Gold), new Vector2(400, 300), new Vector2(30, 30), 2f),
            new DraggableObject(new Ammo("Nabój .38", 38, Color.Gold), new Vector2(400, 300), new Vector2(30, 30), 2f),
            new DraggableObject(new Ammo("Nabój .38", 38, Color.Gold), new Vector2(400, 300), new Vector2(30, 30), 2f),
            new DraggableObject(new Ammo("Nabój .38", 38, Color.Gold), new Vector2(400, 300), new Vector2(30, 30), 2f),
            new DraggableObject(new Ammo("Nabój .38", 38, Color.Gold), new Vector2(400, 300), new Vector2(30, 30), 2f),
            new DraggableObject(new Gold(10), new Vector2(500, 300), new Vector2(40, 40), 3f),
            new DraggableObject(new Weapon("Rewolwer", 20, 6, Color.DarkGray), new Vector2(600, 300), new Vector2(50, 50), 4f)
        };






        InventoryBucket backpack = new InventoryBucket(
        new Rectangle(20, Raylib.GetScreenHeight() - 200, 200, 180));

        Holster holster = new Holster(
        new Rectangle(Raylib.GetScreenWidth() - 220, Raylib.GetScreenHeight() - 220, 200, 200));

        holster.EquippedWeapon = new Weapon("Rewolwer", 20, 6, Color.DarkGray);

        WeaponHolster weaponHolster = new WeaponHolster(
        new Rectangle(Raylib.GetScreenWidth() - 260, Raylib.GetScreenHeight() - 220, 240, 200));

        weaponHolster.EquippedWeapon = new Weapon("Rewolwer", 20, 6, Color.DarkGray);






        // 🔹 Stwórz kamień obok gracza
        Vector2 stonePos = player.Position * World.TileSize + new Vector2(50, 0);
        DraggableObject stone = new DraggableObject(new Gold(1), stonePos, new Vector2(48, 48), 6f);

        objects.Add(stone);


        while (!Raylib.WindowShouldClose())
        {
            float delta = Raylib.GetFrameTime();

            player.Update(world);

            foreach (var obj in objects)
                obj.Update(camera.Cam, world, player);


            cards.Update();

            camera.Follow(player.Position * World.TileSize);
            camera.Update(delta);

            // 🔹 Update hotbara (drop detection)
            hotbar.Update(objects, camera.Cam, player, world);

            foreach (var obj in objects)
                obj.HandlePostUIDrop(world, player);

            if (Raylib.IsMouseButtonReleased(MouseButton.Left))
            {
                Vector2 mouse = Raylib.GetMousePosition();
                foreach (var obj in objects.ToList())
                {
                    if (backpack.CheckDrop(mouse))
                    {
                        backpack.AddItem(obj);
                        objects.Remove(obj);
                        break;
                    }
                }
            }

            // --- Aktualizacja plecaka ---
            backpack.Update(objects);

            weaponHolster.Update();

            // Otwórz cylinder tylko jeśli broń wyciągnięta i kliknięto R
            if (weaponHolster.IsWeaponDrawn && Raylib.IsKeyPressed(KeyboardKey.R))
            {
                holster.Update(backpack); // logika przeładowania
            }



            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.SkyBlue);

            camera.Begin();
            world.Draw();
            player.Draw();
            foreach (var obj in objects)
                obj.Draw();
            camera.End();

            // 🔹 Rysujemy hotbar (UI)
            hotbar.Draw();

            //Rysuje plecak
            backpack.Draw();

            //Rysuje kabure
            weaponHolster.Draw();

            //Rysuje cylinder
            holster.Draw();



            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}
