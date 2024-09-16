using Restaurant.Models;
using Restaurant.Models.Categories;

namespace Restaurant
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int _maxTables = 30;
            bool _programRunning = true;

            List<MenuCategory> categories = new()
            {
                new Salads(),
                new Soups(),
                new MainCourses(),
                new Desserts(),
                new Beverages()
            };

            Menu menu = new(categories);

            List<Table> tables = new();
            for (int i = 1; i <= _maxTables; i++)
            {
                tables.Add(new Table(i));
            }

            IO.Start();

            while (_programRunning)
            {
                if (menu.Items.Count < 1)
                {
                    IO.NotifyUser(IOMessages.NoMenuItems);
                }

                Command command = IO.GetInput(menu, tables);

                switch (command.Target)
                {
                    case CommandTargets.NewMenuItem:
                        menu.Items.Add(command.MenuItem!);
                        break;
                    case CommandTargets.NewOrder:
                        Table table = tables.FirstOrDefault(t => t.Number == command.TableNumber)!;
                        table.Orders.AddRange(command.Orders!);
                        break;
                    case CommandTargets.ItemInfo:
                        IO.PrintItemInfo(command.MenuItem!);
                        break;
                    case CommandTargets.PrintSales:
                        IO.PrintSales(menu, tables);
                        break;
                    case CommandTargets.Exit:
                        if (menu.Items.Count > 0)
                        {
                            IO.NotifyUser(IOMessages.ExitWithItems);
                            IO.PrintSales(menu, tables);
                        }
                        else
                        {
                            IO.NotifyUser(IOMessages.ExitWithoutItems);
                        }
                        _programRunning = false;
                        break;
                }
            }
        }
    }
}
