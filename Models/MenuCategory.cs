namespace Restaurant.Models
{
    internal abstract class MenuCategory
    {
        public abstract string Name { get; }
        public abstract double CalorieIndexMultiplier { get; }
        public abstract double CalculateCalories(int grams);
    }
}
