using BotVaria.Bot;
using BotVaria.DataBase;

namespace BotVaria
{
    internal class Program
    {
        private static void Main()
        {
            // Создание экземпляра текущего бота
            VariaBot bot = new VariaBot("6060051380:AAE4mqPl4lSkBeOmo73Or89EvdAUUadg8S4");

            // Запуск бота
            bot.Start();

            // Нужно что-бы висела консоль
            Console.ReadKey();
        }
    }
}