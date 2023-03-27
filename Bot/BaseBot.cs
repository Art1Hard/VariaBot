using Telegram.Bot;

namespace BotVaria.Bot
{
    internal class BaseBot
    {
        // клиент бота в котором будет токен
        protected TelegramBotClient client;

        // конструктор класса в котором ложится полученный токен в клиент.
        public BaseBot(string token)
        {
            client = new TelegramBotClient(token);
        }
    }
}
