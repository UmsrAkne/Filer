using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Filer.Models
{
    public class DatabaseContext : DbContext
    {
        private static readonly string DatabaseFileName = "database.sqlite";

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private DbSet<History> Histories { get; set; }

        public void Add(string path)
        {
            var history = new History() { Path = path };
            Histories.Add(history);
            SaveChanges();
        }

        public IEnumerable<History> GetHistories()
        {
            // 間に ToList() が入っているのは、DbSet.GroupBy を実行すると例外がスローされるため
            var paths = Histories.Where(h => true).ToList().GroupBy(h => h.Path);
            return paths.Select(hs => hs.OrderByDescending(h => h.DateTime).First());
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