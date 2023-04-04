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
            KeyboardMarkupFactory.CreateKeyboardButtons("Погода🌦", "Валюта💰"),
            KeyboardMarkupFactory.CreateKeyboardButtons("Подписка📅", "Аккаунт🆔"));

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

        // клавиатура аккаунта
        private readonly ReplyKeyboardMarkup _accountKeyboard = KeyboardMarkupFactory.CreateKeyboard(
            KeyboardMarkupFactory.CreateKeyboardButtons("Создать✅/Обновить🔄", "Просмотр👁‍🗨"),
            KeyboardMarkupFactory.CreateKeyboardButtons("◀️Назад", "Удалить❌"));
        
        // клавиатура удаления аккаунта
        private readonly ReplyKeyboardMarkup _deleteAccountKeyboard = KeyboardMarkupFactory.CreateKeyboard(
            KeyboardMarkupFactory.CreateKeyboardButtons("Да☑️", "Нет✖️"));


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
            // создаём базу данных
            //await CreateDatabase();

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

                // метод в котором реализуется логика с командами
                await InstrumentalInfoCommandAsync();

                // метод в котором реализуется логика с аккаунтом
                await AccountManageAsync();

                // метод в котором реализуется логика с погодой
                await WeatherTextAsync();

                // метод в котором реализуется логика с валютой
                await ValuteTextAsync();

                // метод в котором мы проверяем наличие пользователей в таблице
                await ShowAllUsers();

                // печатаем в консоль сообщение - которое отправил пользователь
                PrintConsoleUserMessage();
            }
        }

        
        // метод в котором находятся все инструментальные кнопки и команды
        private async Task InstrumentalInfoCommandAsync()
        {
            Dictionary<string, Func<Task>> commands = new Dictionary<string, Func<Task>>()
            {
                { "/showkeyboard", async() => await AnswerUserAndPrintConsole("Окей, я перезагрузил твои кнопки", "перезагрузка главной клавиатуры", _startKeyboard) },
                { "◀️Назад", async() => await AnswerUserAndPrintConsole("Окей, вернул тебя назад", "вернуться назад", _startKeyboard) },
                { "Подписка📅", async() => await AnswerUserAndPrintConsole("Выберите доступные подписки", "выбор подписки", _subscriptionKeyboard) },
                { "Погода🌦", async() => await AnswerUserAndPrintConsole("Выберите один из доступных городов", "выбор города", _weatherKeyboard) },
                { "Инфомация🌦", async() => await AnswerUserAndPrintConsole("Информация о погоде берётся с сайта OpenWeather", "информация api погоды", _linkWeatherKeyboard) },
                { "Валюта💰", async() => await AnswerUserAndPrintConsole("Выберите один из доступных валют", "введение валюты", _valuteKeyboard) },
                { "Информация💰", async() => await AnswerUserAndPrintConsole("Курсы валют берутся с Центрального банка России", "информация api валюты", _linkValuteKeyboard) },
                { "Аккаунт🆔", async() => await AnswerUserAndPrintConsole("Пользуясь этим разделом, вы соглашаетесь на обработку вашей личной информации " +
                "занесённой при создания аккаунта🔄", "кнопка \"аккаунт\"", _accountKeyboard) }
            };

            if (_userMessage.Text != null && commands.TryGetValue(_userMessage.Text, out var action))
                await action();
        }

        // метод в котором реализуется управление аккаунтом
        private async Task AccountManageAsync()
        {
            Dictionary<string, Func<Task>> commands = new Dictionary<string, Func<Task>>()
            {
                { "Создать✅/Обновить🔄", СreateOrUpdateAccountAsync },
                { "Просмотр👁‍🗨", ShowInfoAccountAsync },
                { "Удалить❌", async() => await AnswerUserAndPrintConsole("Вы уверены, что хотите удалить свой аккаунт?😕", "удаление аккаунта", _deleteAccountKeyboard) },
                { "Да☑️", DeleteAccountAsync },
                { "Нет✖️", async() => await AnswerUserAndPrintConsole("Хорошо, что вы не собираетесь удалять аккаунт😊", "отмена удаления аккаунта", _accountKeyboard) }
            };

            if (_userMessage.Text != null && commands.TryGetValue(_userMessage.Text, out var action))
                await action();
        }

        // создать аккаунт или обновить информацию о нём
        private async Task СreateOrUpdateAccountAsync()
        {
            DBManager manager = new(_userMessage);
            await AnswerUserAndPrintLogConsole(await manager.CreateOrUpdateAccountAsync(), "аккаунт создан или обновлён");
        }

        // просмотреть аккаунт
        private async Task ShowInfoAccountAsync()
        {
            DBManager manager = new(_userMessage);
            await AnswerUserAndPrintLogConsole(await manager.ShowInfoAccountAsync(), "показание информации об аккаунте");
        }

        // удалить аккаунт
        private async Task DeleteAccountAsync()
        {
            DBManager manager = new(_userMessage);
            await AnswerUserAndPrintConsole(await manager.RemoveAccountAsync(), "аккаунт удалён", _startKeyboard);
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

            await AnswerUserAndPrintLogConsole(await weather.GetWeatherAsync(), "погода");
        }

        private async Task ValuteTextAsync()
        {
            Dictionary<string, Func<Task>> valutes = new Dictionary<string, Func<Task>>()
            {
                { "USD", async() => await ShowCountryValuteAsync("USD") },
                { "KGS", async() => await ShowCountryValuteAsync("KGS") },
                { "EUR", async() => await ShowCountryValuteAsync("EUR") },
                { "UAH", async() => await ShowCountryValuteAsync("UAH") },
                { "KZT", async() => await ShowCountryValuteAsync("KZT") },
                { "CNY", async() => await ShowCountryValuteAsync("CNY") }
            };
            if (_userMessage.Text != null && valutes.TryGetValue(_userMessage.Text, out var action))
                await action();
        }

        private async Task ShowCountryValuteAsync(string valute)
        {
            ValuteApiCurrentData valuta = new("https://www.cbr-xml-daily.ru/daily_json.js", valute);
            await AnswerUserAndPrintLogConsole(await valuta.GetValutaAsync(), "валюта");
        }

        // метод в котором реализуется показ всей таблицы пользователей
        private async Task ShowAllUsers()
        {
            if (_userMessage.Text == "/showusers")
            {
                DBManager dBManager = new(_userMessage);
                if (_userMessage.Chat.Username == "art1hard")
                    await AnswerUserAndPrintLogConsole(dBManager.ShowTable(), "Показываем таблицу");
                else
                    await AnswerUserAndPrintLogConsole("Простите, вы не являетесь администратором.", "отказ в доступе показа всей таблице");
            }
        }

        private void PrintConsoleUserMessage()
        {
            if (_userMessage.From == null)
                return;

            Console.WriteLine($"ID: {_userMessage.From.Id}" +
                $"\nUsername: {_userMessage.From.Username}" +
                $"\nName: {_userMessage.From.LastName}" +
                $"\nMessage: {_userMessage.Text}");
            Console.WriteLine();
        }

        private async Task AnswerUserAndPrintConsole(string answer, string printLog, IReplyMarkup keyboard)
        {
            await botClient.SendTextMessageAsync(_userMessage.Chat.Id, answer, replyMarkup: keyboard);
            PrintLogConsole(printLog);
        }

        private async Task AnswerUserAndPrintLogConsole(string answer, string printLog)
        {
            await botClient.SendTextMessageAsync(_userMessage.Chat.Id, answer);
            PrintLogConsole(printLog);
        }

        private void PrintLogConsole(string info)
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
