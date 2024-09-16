namespace Restaurant.Models.Categories
{
    internal class Salads : MenuCategory
    {
        public override string Name => "салата";
        public override double CalorieIndexMultiplier => 1;
        public override double CalculateCalories(int grams)
        {
            return grams;
        }
    }
    internal class Soups : MenuCategory
    {
        public override string Name => "супа";
        public override double CalorieIndexMultiplier => 1;
        public override double CalculateCalories(int grams)
        {
            return grams * CalorieIndexMultiplier;
        }
    }
    internal class MainCourses : MenuCategory
    {
        public override string Name => "основно ястие";
        public override double CalorieIndexMultiplier => 1.7;
        public override double CalculateCalories(int grams)
        {
            return grams * CalorieIndexMultiplier;
        }
    }
    internal class Desserts : MenuCategory
    {
        public override string Name => "десерт";
        public override double CalorieIndexMultiplier => 3;
        public override double CalculateCalories(int grams)
        {
            return grams * CalorieIndexMultiplier;
        }
    }
    internal class Beverages : MenuCategory
    {
        public override string Name => "напитка";
        public override double CalorieIndexMultiplier => 1.2;
        public override double CalculateCalories(int mills)
        {
            return mills * CalorieIndexMultiplier;
        }
    }
}
