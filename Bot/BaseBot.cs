using Telegram.Bot;

namespace BotVaria.Bot
{
    internal class BaseBot
    {
        // клиент бота в котором будет токен
        protected TelegramBotClient botClient;

        // конструктор класса в котором ложится полученный токен в клиент.
        public BaseBot(string token)
        {
            botClient = new TelegramBotClient(token);
        }
    }
}
