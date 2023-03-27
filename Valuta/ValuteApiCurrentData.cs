using BotVaria.Parser;
using BotVaria.Valuta.Response;

namespace BotVaria.Valuta
{
    internal class ValuteApiCurrentData
    {
        private ValuteResponse _valutaResponse;
        private readonly string _url;
        private readonly string _valute;
        private readonly List<string> _emoji;

        public ValuteApiCurrentData(string url, string valute)
        {
            _valutaResponse = new();
            _url = url;
            _valute = valute;
            _emoji = new List<string>();
        }

        public async Task<string> GetValutaAsync()
        {
            _valutaResponse = await ParseValuteAsync();
            return PrintValute();
        }

        private string PrintValute()
        {
            string charCode;
            string name;
            double value;
            int nominal;
            double previous;

            switch (_valute) 
            {
                case "USD":
                    charCode = _valutaResponse.Valute.Usd.CharCode;
                    name = _valutaResponse.Valute.Usd.Name;
                    value = _valutaResponse.Valute.Usd.Value;
                    nominal = _valutaResponse.Valute.Usd.Nominal;
                    previous = _valutaResponse.Valute.Usd.Previous;
                    GetEmoji(value, previous);
                    break;
                case "EUR":
                    charCode = _valutaResponse.Valute.Eur.CharCode;
                    name = _valutaResponse.Valute.Eur.Name;
                    value = _valutaResponse.Valute.Eur.Value;
                    nominal = _valutaResponse.Valute.Eur.Nominal;
                    previous = _valutaResponse.Valute.Eur.Previous;
                    GetEmoji(value, previous);
                    break;
                case "KZT":
                    charCode = _valutaResponse.Valute.Kzt.CharCode;
                    name = _valutaResponse.Valute.Kzt.Name;
                    value = _valutaResponse.Valute.Kzt.Value;
                    nominal = _valutaResponse.Valute.Kzt.Nominal;
                    previous = _valutaResponse.Valute.Kzt.Previous;
                    GetEmoji(value, previous);
                    break;
                case "KGS":
                    charCode = _valutaResponse.Valute.Kgs.CharCode;
                    name = _valutaResponse.Valute.Kgs.Name;
                    value = _valutaResponse.Valute.Kgs.Value;
                    nominal = _valutaResponse.Valute.Kgs.Nominal;
                    previous = _valutaResponse.Valute.Kgs.Previous;
                    GetEmoji(value, previous);
                    break;
                case "UAH":
                    charCode = _valutaResponse.Valute.Uah.CharCode;
                    name = _valutaResponse.Valute.Uah.Name;
                    value = _valutaResponse.Valute.Uah.Value;
                    nominal = _valutaResponse.Valute.Uah.Nominal;
                    previous = _valutaResponse.Valute.Uah.Previous;
                    GetEmoji(value, previous);
                    break;
                case "CNY":
                    charCode = _valutaResponse.Valute.Cny.CharCode;
                    name = _valutaResponse.Valute.Cny.Name;
                    value = _valutaResponse.Valute.Cny.Value;
                    nominal = _valutaResponse.Valute.Cny.Nominal;
                    previous = _valutaResponse.Valute.Cny.Previous;
                    GetEmoji(value, previous);
                    break;
                default:
                    charCode = string.Empty;
                    name = string.Empty;
                    value = 0;
                    nominal = 0;
                    previous = 0;
                    break;
            }

            if (!string.IsNullOrEmpty(charCode))
                return $"Валюта: {charCode}{_emoji[1]}\n" +
                    $"Курс {nominal} {name.ToLower()} к рублю: {Math.Round(value, 2)}💸\n" +
                    $"Разница с прошлого курса: {Math.Round(value - previous, 2)}{_emoji[0]}";
            else
                return $"Произошла непредвиденная ошибка!!!";
        }

        private void GetEmoji(double value, double previous)
        {
            _emoji.AddRange(new[]
                                {
                        value - previous < 0 ? "🔻" : "🔹",
                        string.Empty
                    });
            _emoji[1] = _valute switch
            {
                "USD" => "🇺🇸",
                "EUR" => "🇪🇺",
                "KZT" => "🇰🇿",
                "KGS" => "🇰🇬",
                "CNY" => "🇨🇳",
                "UAH" => "🇺🇦",
                _ => string.Empty
            };
        }

        private async Task<ValuteResponse> ParseValuteAsync()
        {
            BaseResponse response = new();
            ValuteResponse valuta = await response.ParseAsync<ValuteResponse>(_url);
            return valuta;
        }
    }
}
