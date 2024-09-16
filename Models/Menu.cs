namespace Restaurant.Models
{
    internal class Menu
    {
        public List<MenuItem> Items { get; private set; } = new List<MenuItem>();
        public List<MenuCategory> Categories { get; private set; } = new List<MenuCategory>();
        public Menu(List<MenuCategory> categories)
        {
            Categories.AddRange(categories);
        }
    }
}
