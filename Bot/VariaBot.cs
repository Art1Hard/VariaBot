using BotVaria.Valuta;
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
        private Message _message;

        // главная клавиатура погоды
        private static readonly KeyboardButton[] row1ButtonsMain = CreateKeyboardButtons("Погода🌦", "Валюта💰");

        private static readonly KeyboardButton[][] keyboardButtonsMain =
        {
            row1ButtonsMain
        };

        private readonly ReplyKeyboardMarkup KeyboardMain = new(keyboardButtonsMain)
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = false
        };

        private static readonly KeyboardButton[] row1ButtonsCity = CreateKeyboardButtons("Киев", "Янино-1", "Бишкек");
        private static readonly KeyboardButton[] row2ButtonsCity = CreateKeyboardButtons("Аляска", "Санкт-Петербург");
        private static readonly KeyboardButton[] row3ButtonsCity = CreateKeyboardButtons("◀️Назад", "Инфомация🌦");

        private static readonly KeyboardButton[][] keyboardButtonsCity =
        {
            row1ButtonsCity,
            row2ButtonsCity,
            row3ButtonsCity
        };

        private readonly ReplyKeyboardMarkup keyboardCity = new(keyboardButtonsCity)
        {
            ResizeKeyboard = true, // автоматически изменять размер клавиатуры
            OneTimeKeyboard = false // скрыть клавиатуру после использования
        };

        private static readonly KeyboardButton[] row1ButtonsValute = CreateKeyboardButtons("USD", "EUR", "KZT");
        private static readonly KeyboardButton[] row2ButtonsValute = CreateKeyboardButtons("KGS", "UAH", "CNY");
        private static readonly KeyboardButton[] row3ButtonsValute = CreateKeyboardButtons("◀️Назад", "Информация💰");

        private static readonly KeyboardButton[][] keyboardButtonsValute =
        {
            row1ButtonsValute,
            row2ButtonsValute,
            row3ButtonsValute
        };

        private readonly ReplyKeyboardMarkup keyboardValute = new(keyboardButtonsValute)
        {
            ResizeKeyboard = true, // автоматически изменять размер клавиатуры
            OneTimeKeyboard = false // скрыть клавиатуру после использования
        };

        private static KeyboardButton[] CreateKeyboardButtons(params string[] buttonNames)
        {
            var buttons = new KeyboardButton[buttonNames.Length];
            for (int i = 0; i < buttonNames.Length; i++)
            {
                buttons[i] = new KeyboardButton(buttonNames[i]);
            }
            return buttons;
        }


        // конструктор класса
        public VariaBot(string token) : base(token)
        {
            // объявляем новое пустое сообщение
            _message = new();
        }

        // вызываем когда бот запускается
        public void Start()
        {
            client.StartReceiving(Update, Error);
        }

        // сюда попадает сообщение, которое мы обрабатываем в дальнейшем
        private async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            try
            {
                // проверяем сообщение на null, во избежании зависаний
                if (update.Message == null)
                    return;

                // сохраняем сообщение в отдельную переменную
                _message = update.Message;

                // метод в котором реализуется логика с погодой
                await Weather();

            }
            catch (ApiRequestException ex)
            {
                // Обработка ошибок Telegram API
                Console.WriteLine($"Telegram API error occurred: {ex.ErrorCode} - {ex.Message}");
            }
            catch (Exception ex)
            {
                // Обработка других ошибок
                Console.WriteLine($"Error occurred: {ex.Message}");
            }
        }

        private async Task Weather()
        {
            switch (_message.Text)
            {
                case "/showkeyboard":
                    await SendMessageAsync("Окей, я перезагрузил твои кнопки", KeyboardMain);
                    ConsoleLogInfo("перезагрузка главной клавиатуры");
                    break;

                case "Погода🌦":
                    await SendMessageAsync("Выберите один из доступных городов", keyboardCity);
                    ConsoleLogInfo("выбор города");
                    break;
                case "Валюта💰":
                    await SendMessageAsync("Выберите один из доступных валют", keyboardValute);
                    ConsoleLogInfo("введение валюты");
                    break;

                case "USD":
                    await ShowValuteAsync("USD");
                    break;
                case "KGS":
                    await ShowValuteAsync("KGS");
                    break;
                case "EUR":
                    await ShowValuteAsync("EUR");
                    break;
                case "UAH":
                    await ShowValuteAsync("UAH");
                    break;
                case "KZT":
                    await ShowValuteAsync("KZT");
                    break;
                case "CNY":
                    await ShowValuteAsync("CNY");
                    break;

                case "Киев":
                    await ShowCityWeatherAsync(50.4333, 30.5167);
                    break;
                case "Янино-1":
                    await ShowCityWeatherAsync(59.947, 30.556);
                    break;
                case "Бишкек":
                    await ShowCityWeatherAsync(42.87, 74.59);
                    break;
                case "Аляска":
                    await ShowCityWeatherAsync(64.0003, -150.0003);
                    break;
                case "Санкт-Петербург":
                    await ShowCityWeatherAsync(59.8944, 30.2642);
                    break;


                case "Информация💰":
                    await SendMessageAsync("Курсы валют, API - cbr-xml-daily.ru");
                    break;
                case "Инфомация🌦":
                    await SendMessageAsync("Информация о погоде берётся с сайта - openweathermap.org");
                    ConsoleLogInfo("информация api погоды");
                    break;
                case "◀️Назад":
                    await SendMessageAsync("Окей, вернул тебя назад", KeyboardMain);
                    break;

                default:
                    Console.WriteLine($"ID: {_message.Chat.Id}\n{_message.Chat.FirstName} {_message.Chat.LastName}: {_message.Text}");
                    break;
            }
        }

        private async Task ShowValuteAsync(string valute)
        {
            ValuteApiCurrentData valuta = new("https://www.cbr-xml-daily.ru/daily_json.js", valute);
            await SendMessageAsync(await valuta.GetValutaAsync());
            ConsoleLogInfo("валюта");
        }

        private async Task SendMessageAsync(string answer)
        {
            await client.SendTextMessageAsync(_message.Chat.Id, answer);
        }

        private async Task SendMessageAsync(string answer, ReplyKeyboardMarkup keyboard)
        {
            await client.SendTextMessageAsync(_message.Chat.Id, answer, replyMarkup: keyboard);
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

            await SendMessageAsync(await weather.GetWeatherAsync());
            ConsoleLogInfo("погода");
        }

        private void ConsoleLogInfo(string info)
        {
            Console.WriteLine($"Бот выслал пользователю - {_message.Chat.Id}, {info}");
        }

        // сюда попадают все необработанные ошибки
        private Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            throw new Exception();
        }
    }
}
