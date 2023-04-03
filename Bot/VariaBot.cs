using BotVaria.DataBase;
using BotVaria.DataBase.Models;
using BotVaria.Valute;
using BotVaria.Weather;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotVaria.Bot
{
    internal class VariaBot : BaseBot // наследуем всё от базового бота
    {
        // переменная месседж, нужна для сохранения сообщения которое мы получили
        private Message _userMessage;


        // главная клавиатура
        private readonly ReplyKeyboardMarkup _startKeyboard = KeyboardMarkupFactory.CreateKeyboard(
            KeyboardMarkupFactory.CreateKeyboardButtons("Погода🌦", "Валюта💰", "Подписка📅"));

        // клавиатура городов
        private readonly ReplyKeyboardMarkup _weatherKeyboard = KeyboardMarkupFactory.CreateKeyboard(
            KeyboardMarkupFactory.CreateKeyboardButtons("Киев", "Питер", "Бишкек"),
            KeyboardMarkupFactory.CreateKeyboardButtons("Аляска", "Янино-1", "Славутич"),
            KeyboardMarkupFactory.CreateKeyboardButtons("◀️Назад", "Инфомация🌦"));

        // клавиатура валют
        private readonly ReplyKeyboardMarkup _valuteKeyboard = KeyboardMarkupFactory.CreateKeyboard(
            KeyboardMarkupFactory.CreateKeyboardButtons("USD", "EUR", "KZT"),
            KeyboardMarkupFactory.CreateKeyboardButtons("KGS", "UAH", "CNY"),
            KeyboardMarkupFactory.CreateKeyboardButtons("◀️Назад", "Информация💰"));

        // клавиатура выбора подписки
        private readonly ReplyKeyboardMarkup _subscriptionKeyboard = KeyboardMarkupFactory.CreateKeyboard(
            KeyboardMarkupFactory.CreateKeyboardButtons("◀️Назад", "Погодная🌦", "Валютная💰"));


        // клавиатура с ссылкой на сайт погоды
        private readonly InlineKeyboardMarkup _linkWeatherKeyboard = KeyboardMarkupFactory.CreateInlineKeyboard("Ссылка на сайт", @"https://openweathermap.org");

        // клавиатура с ссылкой на сайт API-валюты
        private readonly InlineKeyboardMarkup _linkValuteKeyboard = KeyboardMarkupFactory.CreateInlineKeyboard("Ссылка на API", @"https://www.cbr-xml-daily.ru");


        // конструктор класса
        public VariaBot(string token) : base(token)
        {
            // объявляем новое пустое сообщение
            _userMessage = new();
        }


        // вызываем когда бот запускается
        public void Start()
        {
            botClient.StartReceiving(Update, Error);
        }

        // сюда попадает сообщение, которое мы обрабатываем в дальнейшем
        private async Task Update(ITelegramBotClient botClient, Update inputUpdate, CancellationToken cancellationToken)
        {
            try
            {
                // проверяем сообщение на null, во избежании зависаний
                if (inputUpdate.Message == null)
                    return;

                // сохраняем сообщение в отдельную переменную
                _userMessage = inputUpdate.Message;

                // обработка текстового сообщения
                await MessageText();
            }
            catch (ApiRequestException ex)
            {
                // Обработка ошибок Telegram API
                Console.WriteLine($"Telegram API произошла ошибка: {ex.ErrorCode} - {ex.Message}");
            }
            catch (Exception ex)
            {
                // Обработка других ошибок
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }

        // метод обработки текстового сообщения
        private async Task MessageText()
        {
            if (_userMessage.Text != null)
            {
                // временный метод и проверка для добавления в бд
                if (_userMessage.Text.Contains("Добавить"))
                    await AddTelegramUser();

                // метод в котором реализуется логика с командами
                await InstrumentalInfoCommandAsync();

                // временный метод в котором мы проверяем наличие данных в бд
                await DataBaseCheckAsync();

                // метод в котором реализуется логика с погодой
                await WeatherTextAsync();

                // метод в котором реализуется логика с валютой
                await ValuteTextAsync();

                // печатаем в консоль сообщение - которое отправил пользователь
                PrintMessage();
            }
        }

        private async Task InstrumentalInfoCommandAsync()
        {
            Dictionary<string, Func<Task>> commands = new Dictionary<string, Func<Task>>()
            {
                { "/showkeyboard", () => AnswerUserCommand("Окей, я перезагрузил твои кнопки", "перезагрузка главной клавиатуры", _startKeyboard) },
                { "◀️Назад", () => AnswerUserCommand("Окей, вернул тебя назад", "вернуться назад", _startKeyboard) },
                { "Подписка📅", () => AnswerUserCommand("Выберите доступные подписки", "выбор подписки", _subscriptionKeyboard) },
                { "Погода🌦", () => AnswerUserCommand("Выберите один из доступных городов", "выбор города", _weatherKeyboard) },
                { "Инфомация🌦", () => AnswerUserCommand("Информация о погоде берётся с сайта OpenWeather", "информация api погоды", _linkWeatherKeyboard) },
                { "Валюта💰", () => AnswerUserCommand("Выберите один из доступных валют", "введение валюты", _valuteKeyboard) },
                { "Информация💰", () => AnswerUserCommand("Курсы валют берутся с Центрального банка России", "информация api валюты", _linkValuteKeyboard) }
            };

            if (_userMessage.Text != null && commands.TryGetValue(_userMessage.Text, out var action))
                await action();
        }

        private async Task AnswerUserCommand(string answer, string printLog, IReplyMarkup keyboard)
        {
            await SendTextMessageAsync(answer, keyboard);
            ConsoleLogInfo(printLog);
        }

        private async Task AnswerUserCommand(string answer, string printLog)
        {
            await SendTextMessageAsync(answer);
            ConsoleLogInfo(printLog);
        }

        private async Task WeatherTextAsync()
        {
            Dictionary<string, Func<Task>> cities = new Dictionary<string, Func<Task>>()
            {
                { "Киев", async() => await ShowCityWeatherAsync(50.4333, 30.5167) },
                { "Янино-1", async() => await ShowCityWeatherAsync(59.947, 30.556) },
                { "Бишкек", async() => await ShowCityWeatherAsync(42.87, 74.59) },
                { "Аляска", async() => await ShowCityWeatherAsync(64.0003, -150.0003) },
                { "Питер", async() => await ShowCityWeatherAsync(59.8944, 30.2642) },
                { "Славутич", async() => await ShowCityWeatherAsync(51.5196, 30.7343) }
            };

            if (_userMessage.Text != null && cities.TryGetValue(_userMessage.Text, out var action))
                await action();
        }

        private async Task ShowCityWeatherAsync(double lat, double lon)
        {
            WeatherApiCurrentData weather = new(
                    "https://api.openweathermap.org/data/2.5/weather?",
                    lat,
                    lon,
                    "metric",
                    "ru",
                    "de362d5cd398a08ed08785357a45fefa");

            await SendTextMessageAsync(await weather.GetWeatherAsync());
            ConsoleLogInfo("погода");
        }

        private async Task ValuteTextAsync()
        {
            Dictionary<string, Func<Task>> valutes = new Dictionary<string, Func<Task>>()
            {
                { "USD", async() => await ShowValuteAsync("USD") },
                { "KGS", async() => await ShowValuteAsync("KGS") },
                { "EUR", async() => await ShowValuteAsync("EUR") },
                { "UAH", async() => await ShowValuteAsync("UAH") },
                { "KZT", async() => await ShowValuteAsync("KZT") },
                { "CNY", async() => await ShowValuteAsync("CNY") }
            };
            if (_userMessage.Text != null && valutes.TryGetValue(_userMessage.Text, out var action))
                await action();
        }

        private async Task ShowValuteAsync(string valute)
        {
            ValuteApiCurrentData valuta = new("https://www.cbr-xml-daily.ru/daily_json.js", valute);
            await SendTextMessageAsync(await valuta.GetValutaAsync());
            ConsoleLogInfo("валюта");
        }

        private async Task DataBaseCheckAsync()
        {
            if(_userMessage.Text == "/database")
            {
                if (_userMessage.Chat.Username == "art1hard")
                    await ShowDataBase();
                else
                    await SendTextMessageAsync("Простите, вы не являетесь администратором.");
            }
        }

        private async Task ShowDataBase()
        {
            using (ApplicationContext db = new())
            {
                var users = db.Users.ToList();

                string objects = "Список объектов:\n";
                foreach (TelegramUser user in users)
                {
                    if (string.IsNullOrEmpty(objects))
                        objects = $"{user.Id}: {user.UserName} {user.FirstName}\n";
                    else
                        objects += $"{user.Id}: {user.UserName} {user.FirstName}\n";
                }
                await SendTextMessageAsync(objects);
            }
        }

        private async Task AddTelegramUser()
        {
            using (ApplicationContext db = new())
            {
               
                // создаем два объекта User
                TelegramUser art = new() { Id = 123, UserName = "art", FirstName = "artem", LastName = "hardov" };
                TelegramUser vlad = new() { Id = 124, UserName = "vlad", FirstName = "vlad", LastName = "dark" };

                // добавляем их в бд
                db.Users.Remove(vlad);
                db.Users.Remove(art);
                db.SaveChanges();
                await SendTextMessageAsync("Объекты успешно сохранены");

                // получаем объекты из бд и выводим на консоль
                var users = db.Users.ToList();
                Console.WriteLine("Список объектов:");
                foreach (TelegramUser user in users)
                {
                    Console.WriteLine($"{user.Id}.{user.UserName} - {user.FirstName}");
                }
            }
        }

        private void PrintMessage()
        {
            if(_userMessage.Chat.FirstName == null)
                Console.WriteLine($"ID: {_userMessage.Chat.Id}\n{_userMessage.Chat.LastName}: {_userMessage.Text}");
            else if(_userMessage.Chat.LastName == null)
                Console.WriteLine($"ID: {_userMessage.Chat.Id}\n{_userMessage.Chat.FirstName}: {_userMessage.Text}");
            else
                Console.WriteLine($"ID: {_userMessage.Chat.Id}\n{_userMessage.Chat.FirstName} {_userMessage.Chat.LastName}: {_userMessage.Text}");
            Console.WriteLine();
        }

        private async Task SendTextMessageAsync(string answer)
        {
            await botClient.SendTextMessageAsync(_userMessage.Chat.Id, answer);
        }

        private async Task SendTextMessageAsync(string answer, IReplyMarkup keyboard)
        {
            await botClient.SendTextMessageAsync(_userMessage.Chat.Id, answer, replyMarkup: keyboard);
        }

        private void ConsoleLogInfo(string info)
        {
            Console.WriteLine($"Бот выслал пользователю \"{info}\"");
        }

        // сюда попадают все необработанные ошибки
        private Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            throw new Exception();
        }
    }
}
