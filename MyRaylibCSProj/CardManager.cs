using System.Numerics;
using Raylib_cs;

partial class Program
{
    class CardManager
    {
        public List<Card> Cards = new List<Card>();
        public int SelectedCard = -1;

        const int CardWidth = 140;
        const int CardHeight = 180;

        public CardManager()
        {
            Cards.Add(new Card("Drzewo", "Generuje drewno i owoce.", Color.DarkGreen));
        }

        public void DiscoverCard(string name, string description, Color color)
        {
            if (!Cards.Exists(c => c.Name == name))
                Cards.Add(new Card(name, description, color));
        }

        public void Update()
        {
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                Vector2 mouse = Raylib.GetMousePosition();
                int screenWidth = Raylib.GetScreenWidth();
                int startX = (screenWidth - (Cards.Count * (CardWidth + 10))) / 2;
                int y = Raylib.GetScreenHeight() - CardHeight - 20;

                for (int i = 0; i < Cards.Count; i++)
                {
                    int x = startX + i * (CardWidth + 10);
                    Rectangle rect = new Rectangle(x, y, CardWidth, CardHeight);
                    if (Raylib.CheckCollisionPointRec(mouse, rect))
                        SelectedCard = (SelectedCard == i) ? -1 : i;
                }
            }
        }

        public void Draw()
        {
            int screenWidth = Raylib.GetScreenWidth();
            int startX = (screenWidth - (Cards.Count * (CardWidth + 10))) / 2;
            int y = Raylib.GetScreenHeight() - CardHeight - 20;

            for (int i = 0; i < Cards.Count; i++)
            {
                int x = startX + i * (CardWidth + 10);
                Card card = Cards[i];
                Color border = (i == SelectedCard) ? Color.Yellow : Color.DarkGray;
                Raylib.DrawRectangleLinesEx(new Rectangle(x - 2, y - 2, CardWidth + 4, CardHeight + 4), 3, border);
                Raylib.DrawRectangle(x, y, CardWidth, CardHeight, card.Color);
                Raylib.DrawText(card.Name, x + 10, y + 10, 20, Color.Black);
                Raylib.DrawText(card.Description, x + 10, y + 40, 16, Color.Black);
            }
        }
    }
}
