using BotVaria.Parser;
using BotVaria.Valute.Response;

namespace BotVaria.Valute
{
    internal class ValuteApiCurrentData
    {
        private Currency _valutaResponse;
        private readonly string _url;
        private readonly string _charCode;
        private readonly List<string> _emoji;

        public ValuteApiCurrentData(string url, string charCode)
        {
            _valutaResponse = new();
            _url = url;
            _charCode = charCode;
            _emoji = new List<string>();
        }

        public async Task<string> GetValutaAsync()
        {
            _valutaResponse = await ParseValuteAsync();
            GetEmoji();
            return PrintValute();
        }

        private string PrintValute()
        {
            string charCode = _valutaResponse.CharCode;
            string name = _valutaResponse.Name.ToLower();
            int nominal = _valutaResponse.Nominal;
            double value = Math.Round(_valutaResponse.Value, 2);
            double previous = Math.Round(_valutaResponse.Value - _valutaResponse.Previous, 2);

            string status = _emoji[0];
            string flag = _emoji[1];

            if (previous == 0)
                previous = 0;

            if (!string.IsNullOrEmpty(charCode))
                return $"Валюта: {charCode}{flag}\n" +
                    $"{nominal} {name}: {value} руб.\n" +
                    $"Разница с прошлого курса: {previous}{status}";
            else
                return $"Произошла непредвиденная ошибка!!!";
        }

        private void GetEmoji()
        {
            double previous = Math.Round(_valutaResponse.Value - _valutaResponse.Previous, 2);
            if (previous == 0)
                previous = 0;

            _emoji.AddRange(new[]
                                {
                        previous < 0 ? "🔻": previous == 0 ? "➖" : "🔹",
                        string.Empty
                    });
            _emoji[1] = _charCode switch
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

        private async Task<Currency> ParseValuteAsync()
        {
            BaseResponse response = new();
            Currency valuta = await response.ParseValuteAsync(_url, _charCode);
            return valuta;
        }
    }
}
