using Raylib_cs;

partial class Program
{
    class Card
    {
        public string Name;
        public string Description;
        public Color Color;

        public Card(string name, string description, Color color)
        {
            Name = name;
            Description = description;
            Color = color;
        }
    }
}
