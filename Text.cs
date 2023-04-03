namespace BotVaria
{
    internal static class Text
    {
        public static string FirstCharUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return char.ToUpper(input[0]) + input[1..];
        }
    }
}
