namespace Restaurant.Models
{
    internal class Table
    {
        public int Number { get; private set; }
        public List<MenuItem> Orders { get; private set; } = new List<MenuItem>();
        public Table(int number)
        {
            Number = number;
        }
    }
}
