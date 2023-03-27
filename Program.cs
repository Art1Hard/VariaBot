using BotVaria.Bot;

namespace BotVaria
{
    internal class Program
    {
        static void Main(string[] args)
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