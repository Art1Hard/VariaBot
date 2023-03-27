using BotVaria.Parser;

namespace BotVaria.Weather.Response
{
    internal class WeatherResponse : BaseResponse
    {
        public Temperature Main { get; set; }
        public Weather[] Weather { get; set; }
        public Wind Wind { get; set; }
        public string Name { get; set; }
    }
}
