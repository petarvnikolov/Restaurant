namespace Restaurant.Models
{
    internal class MenuItem
    {
        public string Name { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public double GramsMills { get; private set; }
        public MenuCategory Category { get; private set; }

        public double? Calories { get; private set; }

        public MenuItem(string name, decimal price, int gramsMills, MenuCategory category)
        {
            Name = name;
            Price = price;
            GramsMills = gramsMills;
            Category = category;
            if (category.Name != "салата")
            {
                Calories = category.CalculateCalories(gramsMills);
            }
        }
    }
}
