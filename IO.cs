using System.Text.RegularExpressions;
using Restaurant.Models;

namespace Restaurant
{
    internal enum IOMessages
    {
        InvalidOperation,
        InvalidArguments,
        AvailableCommands,
        NoMenuItems,
        InvalidName,
        InvalidPrice,
        InvalidGramsMills,
        ItemNotFound,
        ItemAlreadyExists,
        TableNotFound,
        ExitWithItems,
        ExitWithoutItems
    }
    internal static class IO
    {
        public static void Start()
        {
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Console.WriteLine("Ресторант мениджър [Версия 1.0.0-alpha]");
            Console.WriteLine("(c) Петър Николов, КН, 241кнр\n");

            IO.NotifyUser(IOMessages.AvailableCommands);
        }

        public static void NotifyUser(IOMessages ioMessageType, string? message = null)
        {
            switch (ioMessageType)
            {
                case IOMessages.InvalidOperation:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Невалидна операция.\n");
                    Console.ResetColor();
                    break;
                case IOMessages.AvailableCommands:
                    Console.WriteLine("Възможни операции:\n");
                    Console.WriteLine("     * (за нов продукт) <категория>, <уникално име>, <грамаж/милилитри>, <цена (с точка за разделител)>");
                    Console.WriteLine("     * (за нова поръчка) <номер на маса>, <име на продукт>, <име на продукт>, ...");
                    Console.WriteLine("     * Инфо <име на продукт>");
                    Console.WriteLine("     * Продажби");
                    Console.WriteLine("     * Помощ");
                    Console.WriteLine("     * Изход\n");
                    break;
                case IOMessages.InvalidArguments:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Невалидни аргументи.\n");
                    Console.ResetColor();
                    break;
                case IOMessages.InvalidName:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Невалидно име на продукт. Името трябва да бъде изписано само с букви, празни места и тирета.\n");
                    Console.ResetColor();
                    break;
                case IOMessages.InvalidPrice:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Невалидна цена на продукт. Цената трябва да бъде число с плаваща запетая.\n");
                    Console.ResetColor();
                    break;
                case IOMessages.InvalidGramsMills:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Невалиден грамаж/милилитри на продукт. Грамажът/милилитрите трябва да бъдат цяло число.\n");
                    Console.ResetColor();
                    break;
                case IOMessages.NoMenuItems:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Няма налични продукти в менюто. Моля добавете нов.\n");
                    Console.ResetColor();
                    break;
                case IOMessages.ItemNotFound:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Продуктът \"{message}\" не е намерен в менюто.\n");
                    Console.ResetColor();
                    break;
                case IOMessages.ItemAlreadyExists:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Продуктът вече съществува в менюто.\n");
                    Console.ResetColor();
                    break;
                case IOMessages.TableNotFound:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Масата не е намерена.\n");
                    Console.ResetColor();
                    break;
                case IOMessages.ExitWithItems:
                    Console.WriteLine("Приключване на деня...\n");
                    break;
                case IOMessages.ExitWithoutItems:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Приключване на деня без налични продукти в менюто...\n");
                    Console.ResetColor();
                    break;
            }
        }

        public static Command GetInput(Menu menu, List<Table> tables)
        {
            bool noMenuItems = menu.Items.Count < 1;

            Command command = new(CommandTargets.Invalid);

            Console.Write("> ");

            string? rawInput = Console.ReadLine();
            string[] rawInputArray;

            Console.WriteLine("");

            if (String.IsNullOrWhiteSpace(rawInput))
            {
                IO.NotifyUser(IOMessages.InvalidOperation);
                IO.NotifyUser(IOMessages.AvailableCommands);
                return command;
            }

            rawInputArray = rawInput.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            bool categoryExists = menu.Categories.Any(c => c.Name == rawInputArray[0].ToLower());

            if (noMenuItems && !categoryExists && rawInputArray[0].ToLower() != "изход")
            {
                return command;
            }
            else if (categoryExists)
            {
                command = ValidateMenuItemInput(rawInputArray, menu);
                return command;
            }

            if (rawInput.StartsWith("инфо"))
            {
                command = ValidateItemInfo(rawInput, menu);
                return command;
            }

            bool isTableNumber = int.TryParse(rawInputArray[0], out int tableNumber);

            if (isTableNumber)
            {
                command = ValidateOrderInput(rawInputArray, menu, tableNumber, tables);
                return command;
            }

            switch (rawInputArray[0].ToLower())
            {
                case "продажби":
                    command.Target = CommandTargets.PrintSales;
                    break;
                case "помощ":
                    IO.NotifyUser(IOMessages.AvailableCommands);
                    break;
                case "изход":
                    command.Target = CommandTargets.Exit;
                    break;
                default:
                    IO.NotifyUser(IOMessages.InvalidOperation);
                    IO.NotifyUser(IOMessages.AvailableCommands);
                    break;
            }

            return command;
        }

        private static Command ValidateMenuItemInput(string[] rawInputArray, Menu menu)
        {
            Command command = new(CommandTargets.Invalid);

            string pattern = @"^[a-zA-Zа-яА-ЯёЁ\s-]+$";

            if (rawInputArray.Length < 4 || rawInputArray.Length > 4)
            {
                IO.NotifyUser(IOMessages.InvalidArguments);
                IO.NotifyUser(IOMessages.AvailableCommands);
                return command;
            }

            bool nameIsValid = Regex.IsMatch(rawInputArray[1], pattern);

            if (!nameIsValid)
            {
                IO.NotifyUser(IOMessages.InvalidName);
                return command;
            }

            bool itemExists = menu.Items.Any(i => i.Name.ToLower() == rawInputArray[1].ToLower());

            if (itemExists)
            {
                IO.NotifyUser(IOMessages.ItemAlreadyExists);
                return command;
            }

            bool gramsMillsIsValid = int.TryParse(rawInputArray[2], out int gramsMills) && gramsMills > 0 && gramsMills < 1000;
            bool priceIsValid = decimal.TryParse(rawInputArray[3], out decimal price) && price > 0.00m && price < 100.00m;
            MenuCategory category = menu.Categories.First(c => c.Name == rawInputArray[0]);

            if (!gramsMillsIsValid)
            {
                IO.NotifyUser(IOMessages.InvalidGramsMills);
                return command;
            }

            if (!priceIsValid)
            {
                IO.NotifyUser(IOMessages.InvalidPrice);
                return command;
            }

            MenuItem menuItem = new(rawInputArray[1], price, gramsMills, category);

            command.Target = CommandTargets.NewMenuItem;
            command.MenuItem = menuItem;

            return command;
        }

        public static Command ValidateOrderInput(string[] rawInputArray, Menu menu, int tableNumber, List<Table> tables)
        {
            Command command = new(CommandTargets.Invalid);

            bool tableNumberIsValid = tableNumber > 0 && tableNumber < 31;

            if (rawInputArray.Length < 2)
            {
                IO.NotifyUser(IOMessages.InvalidArguments);
                IO.NotifyUser(IOMessages.AvailableCommands);
                return command;
            }

            if (!tableNumberIsValid)
            {
                IO.NotifyUser(IOMessages.TableNotFound);
                return command;
            }

            List<MenuItem> orders = new();

            foreach (string orderItem in rawInputArray)
            {
                if (orderItem == rawInputArray[0])
                {
                    continue;
                }

                MenuItem? menuItem = menu.Items.FirstOrDefault(i => i.Name.ToLower() == orderItem.ToLower());

                if (menuItem == null)
                {
                    IO.NotifyUser(IOMessages.ItemNotFound, orderItem);
                    return command;
                }

                orders.Add(menuItem);
            }

            command.Target = CommandTargets.NewOrder;
            command.TableNumber = tableNumber;
            command.Orders = orders;

            return command;
        }

        private static Command ValidateItemInfo(string rawInput, Menu menu)
        {
            Command command = new(CommandTargets.Invalid);

            if (rawInput.Length < 6)
            {
                IO.NotifyUser(IOMessages.InvalidArguments);
                IO.NotifyUser(IOMessages.AvailableCommands);
                return command;
            }

            string menuItemName = rawInput.Substring(5).Trim();

            MenuItem? menuItem = menu.Items.FirstOrDefault(i => i.Name.ToLower() == menuItemName.ToLower());

            if (menuItem == null)
            {
                IO.NotifyUser(IOMessages.ItemNotFound);
                return command;
            }

            command.Target = CommandTargets.ItemInfo;
            command.MenuItem = menuItem;

            return command;
        }

        public static void PrintItemInfo(MenuItem menuItem)
        {
            Console.WriteLine($"Информация за продукт \"{menuItem.Name}\"");
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("| Категория            | {0, -27} |", string.Concat(menuItem.Category.Name[0].ToString().ToUpper(), menuItem.Category.Name.AsSpan(1)));
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("| Цена                 | {0, -27:F2} |", menuItem.Price);
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("| Грамаж/милилитри     | {0, -27} |", menuItem.GramsMills);
            Console.WriteLine("------------------------------------------------------");
            if (menuItem.Calories != null)
            {
                Console.WriteLine("| Калории              | {0, -27} |", menuItem.Calories);
                Console.WriteLine("------------------------------------------------------");
            }
            Console.WriteLine("");
        }

        public static void PrintSales(Menu menu, List<Table> tables)
        {
            Console.WriteLine("Продажби за деня:");
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("| Общо заети маси   | {0, -30} |", tables.Count(t => t.Orders.Count > 0));
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("| Общо продажби     | Брой: {0, -2} | Обща сума: {1, -8:F2} |", tables.Sum(t => t.Orders.Count), tables.Sum(t => t.Orders?.Sum(o => o.Price) ?? 0));
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("По категории за деня:");
            Console.WriteLine("------------------------------------------------------");
            foreach (MenuCategory category in menu.Categories)
            {
                Console.WriteLine("| {0, -17} | Брой: {1, -2} | Обща сума: {2, -8:F2} |", string.Concat(category.Name[0].ToString().ToUpper(), category.Name.AsSpan(1)), tables.Sum(t => t.Orders.Count(o => o.Category == category)), tables.Sum(t => t.Orders?.Where(o => o.Category == category).Sum(o => o.Price) ?? 0));
                Console.WriteLine("------------------------------------------------------");
            }
            Console.WriteLine("");
        }
    }

    internal enum CommandTargets
    {
        NewMenuItem,
        NewOrder,
        ItemInfo,
        PrintSales,
        Exit,
        Invalid
    }
    internal class Command
    {
        public CommandTargets Target { get; set; }
        public MenuItem? MenuItem { get; set; }
        public int TableNumber { get; set; }
        public List<MenuItem>? Orders { get; set; } = new List<MenuItem>();

        public Command(CommandTargets target, MenuItem? menuItem = null, int tableNumber = 0, List<MenuItem>? orders = null)
        {
            this.Target = target;

            switch (target)
            {
                case CommandTargets.NewMenuItem:
                    if (menuItem == null)
                    {
                        throw new ArgumentNullException(nameof(menuItem), "Menu item object is null.");
                    }
                    this.MenuItem = menuItem;
                    break;
                case CommandTargets.NewOrder:
                    if (tableNumber == 0)
                    {
                        throw new ArgumentNullException(nameof(tableNumber), "Table object is null.");
                    }
                    else if (orders == null || orders.Count < 1)
                    {
                        throw new ArgumentNullException(nameof(orders), "Menu items list is null or empty.");
                    }
                    this.TableNumber = tableNumber;
                    this.Orders = orders;
                    break;
                case CommandTargets.ItemInfo:
                    if (menuItem == null)
                    {
                        throw new ArgumentNullException(nameof(menuItem), "Menu item object is null.");
                    }
                    this.MenuItem = menuItem;
                    break;
            }
        }

    }
}
