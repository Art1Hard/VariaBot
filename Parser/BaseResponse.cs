using BotVaria.Valute.Response;
using BotVaria.Weather.Parser;
using Newtonsoft.Json;
using System.Net;
using System.Text.Json;

namespace BotVaria.Parser
{
    internal class BaseResponse : IResponse
    {
        public async Task<T> ParseAsync<T>(string url) where T : BaseResponse
        {
            try
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetStringAsync(url);

                var responseObject = JsonConvert.DeserializeObject<T>(response);

                if (responseObject != null)
                    return responseObject;
                else
                    throw new Exception("Failed to deserialize response");
            }
            catch (WebException ex)
            {
                // Обработка ошибок
                Console.WriteLine($"Ошибка HTTP: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Обработка других ошибок
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            return null;
        }

        public async Task<Currency> ParseValuteAsync(string url, string currencyCode)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();

                var currencyRates = System.Text.Json.JsonSerializer.Deserialize<CurrencyRates>(json);
                var currency = currencyRates.Valute[currencyCode];
                return currency;
            }
        }
    }
}
