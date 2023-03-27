using BotVaria.Parser;
using BotVaria.Weather.Response;

namespace BotVaria.Weather
{
    internal class WeatherApiCurrentData
    {
        // переменная в которой мы записываем полную ссылку api погоды
        private readonly string _fullUrl;

        // в данный экземпляр мы сохраняем информацию о погоде
        private WeatherResponse _weatherResponce;

        // список эмоджи
        private readonly List<string> _emoji;

        public WeatherApiCurrentData(string url, double lat, double lon, string unit, string lang, string apiKey)
        { 
            _fullUrl = $"{url}lat={lat}&lon={lon}&units={unit}&lang={lang}&appid={apiKey}";
            _emoji = new List<string>();
            _weatherResponce = new();
        }

        // публичный класс который помогает нам получать погоду
        public async Task<string> GetWeatherAsync()
        {
            _weatherResponce = await ParseWeather();

            ChoosingEmoji();

            return PrintWeather();
        }

        private async Task<WeatherResponse> ParseWeather()
        {
            BaseResponse response = new();
            WeatherResponse weatherResponse = await response.ParseAsync<WeatherResponse>(_fullUrl);
            return weatherResponse;
        }

        private string PrintWeather()
        {
            return $"Основная информация🗒" +
                               $"\nГород: {_weatherResponce.Name}🏘" +
                               $"\n" +
                               $"\nТемпература🌡" +
                               $"\nТемпература: {_weatherResponce.Main.Temp} °С{_emoji[0]}" +
                               $"\nОщущение: {_weatherResponce.Main.Feels_like} °С{_emoji[1]}" +
                               $"\n" +
                               $"\nПогода🌦" +
                               $"\n{Text.FirstCharUpper(_weatherResponce.Weather[0].Description)}{_emoji[2]}" +
                               $"\nВлажность: {_weatherResponce.Main.Humidity}%💦" +
                               $"\nСкорость ветра: {_weatherResponce.Wind.speed} м/с💨" +
                               $"\nНаправление ветра: {DegConverter.ConvertWindDirection(Convert.ToInt32(_weatherResponce.Wind.deg))}🧭";
        }

        

        private void ChoosingEmoji()
        {
            _emoji.AddRange(new[] 
            { 
                _weatherResponce.Main.Temp <= 0 ? "🧊" : "🔥",
                _weatherResponce.Main.Feels_like <= 0 ? "🧊" : "🔥", 
                string.Empty 
            });
            _emoji[2] = _weatherResponce.Weather[0].Description switch 
            { 
                "снег" => "🌨", 
                "небольшой снег" => "❄️",
                "небольшой снегопад" => "🌨",
                "пасмурно" => "☁️",
                "ясно" => "☀️",
                "небольшая облачность" => "🌤",
                "небольшой дождь" => "🌧",
                _ => string.Empty 
            };
        }
    }
}
