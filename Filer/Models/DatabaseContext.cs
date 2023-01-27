using System;
using System.Data.SQLite;
using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Filer.Models
{
    public class DatabaseContext : DbContext
    {
        private static readonly string DatabaseFileName = "database.sqlite";

        public DbSet<History> Histories { get; set; }

        public void Add(string path)
        {
            var history = new History() { Path = path };
            Histories.Add(history);
            SaveChanges();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!File.Exists(DatabaseFileName))
            {
                SQLiteConnection.CreateFile(DatabaseFileName); // ファイルが存在している場合は問答無用で上書き。
            }

            var connectionString = new SqliteConnectionStringBuilder { DataSource = DatabaseFileName }.ToString();
            optionsBuilder.UseSqlite(new SQLiteConnection(connectionString));
        }
    }
}