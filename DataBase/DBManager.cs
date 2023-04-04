using BotVaria.DataBase.Models;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace BotVaria.DataBase
{
    internal class DBManager
    {
        private Message _userMessage { get; }

        public DBManager(Message userMessage)
        {
            _userMessage = userMessage;
        }

        public async Task<string> CreateOrUpdateAccountAsync()
        {
            TelegramUser telegramUser = CreateTelegramUser();
            using (ApplicationContext db = new())
            {
                try
                {
                    var existingUser = await db.Users.FindAsync(telegramUser.Id);
                    if (existingUser == null)
                    {
                        await db.Users.AddAsync(telegramUser);
                        await db.SaveChangesAsync();
                        return "Аккаунт успешно создан!";
                    }
                    else
                    {
                        db.Entry(existingUser).State = EntityState.Detached;
                        db.Users.Update(telegramUser);
                        await db.SaveChangesAsync();
                        return "Аккаунт успешно обновлён!";
                    }
                }
                catch
                {
                    return "Что-то пошло не так при создании или обновлении вашего аккаунта.";
                }
            }
        }

        public async Task<string> RemoveAccountAsync()
        {
            TelegramUser telegramUser = CreateTelegramUser();
            using (ApplicationContext db = new())
            {
                try
                {
                    db.Users.Remove(telegramUser);
                    await db.SaveChangesAsync();
                    return "Аккаунт успешно удалён!";
                }
                catch
                {
                    return "Что-то пошло не так при удалении вашего аккаунта." +
                        "\nВозможно вы ещё не создали аккаунт!";
                }
            }
        }

        public async Task<string> ShowInfoAccountAsync()
        {
            using (ApplicationContext db = new())
            {
                try
                {
                    var users = await db.Users.ToListAsync();

                    foreach (var user in users)
                        if (_userMessage.From != null && user.Id == _userMessage.From.Id)
                        {
                            return $"Ваш ID: {user.Id}" +
                                $"\nВаш никнейм: {user.UserName}" +
                                $"\nВаше имя: {user.FirstName} {user.LastName}";
                        }
                    return "Вашего аккаунта не существует.";
                }
                catch
                {
                    return "Что-то пошло не так, попробуйте позже...";
                }
            }
        }

        public string ShowTable()
        {
            using (ApplicationContext db = new())
            {
                var users = db.Users.ToList();

                string dbUsers = "Список пользователей в БД:\n";
                int userNamber = 0;
                foreach (TelegramUser user in users)
                {
                    userNamber++;
                    if (string.IsNullOrEmpty(dbUsers))
                        dbUsers = ShowRow(userNamber, user);
                    else
                        dbUsers += ShowRow(userNamber, user);
                }
                return dbUsers;
            }
        }

        private static string ShowRow(int userNamber, TelegramUser user)
        {
            return $"{userNamber}. ID: {user.Id}, U: {user.UserName}, N: {user.FirstName} {user.LastName}\n";
        }

        private TelegramUser CreateTelegramUser()
        {
            if (_userMessage.From == null)
                throw new Exception("Не удалось найти пользователя!");
            TelegramUser telegramUser = new()
            {
                Id = Convert.ToInt32(_userMessage.From.Id),
                UserName = _userMessage.From.Username,
                FirstName = _userMessage.From.FirstName,
                LastName = _userMessage.From.LastName
            };
            return telegramUser;
        }
    }
}
