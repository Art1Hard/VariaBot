namespace BotVaria.Weather
{
    internal static class DegConverter
    {
        public static string ConvertWindDirection(int degrees)
        {
            // Создаем массив направлений в зависимости от количества направлений
            string[] directions = new string[] 
            {
                "Северный",
                "Северо-Восточный",
                "Восточный",
                "Юго-Восточный",
                "Южный",
                "Юго-Западный",
                "Западный",
                "Северо-Западный"
            };

            // Рассчитываем шаг между направлениями в градусах
            int step = 360 / directions.Length;

            // Определяем индекс направления в массиве
            int index = (int)Math.Round((degrees % 360) / (double)step) % directions.Length;

            // Возвращаем соответствующее направление
            return directions[index];
        }
    }
}
