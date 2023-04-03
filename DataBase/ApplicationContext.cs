using BotVaria.DataBase.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotVaria.DataBase
{
    internal class ApplicationContext : DbContext
    {
        public DbSet<TelegramUser> Users => Set<TelegramUser>();
        public ApplicationContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=botvaria.db");
        }
    }
}
