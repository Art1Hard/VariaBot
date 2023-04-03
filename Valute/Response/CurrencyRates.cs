namespace BotVaria.Valute.Response
{
    internal class CurrencyRates
    {
        public string Date { get; set; }
        public Dictionary<string, Currency> Valute { get; set; }
    }
}
