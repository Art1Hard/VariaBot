using Telegram.Bot.Types.ReplyMarkups;

namespace BotVaria.Bot
{
    internal static class KeyboardMarkupFactory
    {
        public static KeyboardButton[] CreateKeyboardButtons(params string[] buttonNames)
        {
            var buttons = new KeyboardButton[buttonNames.Length];
            for (int i = 0; i < buttonNames.Length; i++)
                buttons[i] = new KeyboardButton(buttonNames[i]);
            return buttons;
        }

        public static ReplyKeyboardMarkup CreateKeyboard(KeyboardButton[] row1)
        {
            var keyboardButtons = new KeyboardButton[][] { row1 };
            return new ReplyKeyboardMarkup(keyboardButtons)
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };
        }

        public static ReplyKeyboardMarkup CreateKeyboard(KeyboardButton[] row1, KeyboardButton[] row2)
        {
            var keyboardButtons = new KeyboardButton[][] { row1, row2 };
            return new ReplyKeyboardMarkup(keyboardButtons)
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };
        }

        public static ReplyKeyboardMarkup CreateKeyboard(KeyboardButton[] row1, KeyboardButton[] row2, KeyboardButton[] row3)
        {
            var keyboardButtons = new KeyboardButton[][] { row1, row2, row3 };
            return new ReplyKeyboardMarkup(keyboardButtons)
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };
        }

        public static ReplyKeyboardMarkup CreateKeyboard(KeyboardButton[] row1, KeyboardButton[] row2, KeyboardButton[] row3, KeyboardButton[] row4)
        {
            var keyboardButtons = new KeyboardButton[][] { row1, row2, row3, row4 };
            return new ReplyKeyboardMarkup(keyboardButtons)
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };
        }

        public static InlineKeyboardMarkup CreateInlineKeyboard(string description, string link)
        {
            InlineKeyboardMarkup inlineKeyboard = new(new[]
            {
                InlineKeyboardButton.WithUrl(description, link),
            });
            return inlineKeyboard;
        }
    }
}
