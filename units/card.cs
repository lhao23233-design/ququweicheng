namespace CoopDefense.Units
{
    public class Card
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }

        public Card(string name, int price, string description)
        {
            Name = name;
            Price = price;
            Description = description;
        }
    }
}