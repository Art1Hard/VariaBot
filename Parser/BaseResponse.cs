﻿using BotVaria.Weather.Parser;
using Newtonsoft.Json;
using System.Net;

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
                {
                    return responseObject;
                }
                else
                {
                    throw new Exception("Failed to deserialize response");
                }

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
    }
}
