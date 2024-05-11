using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;

namespace plannerCLI.Repositories
{
    internal class SQLiteDatabase
    {
        public string connectionString = "Data Source=taskdb.sqlite;Version=3;";
        public void CreateStandardTasksTable()
        {
            using(var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "CREATE TABLE IF NOT EXISTS Tasks (Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                    " TaskName TEXT," +
                    " Due DATETIME," +
                    " Priority INTEGER," +
                    " Note TEXT," +
                    " Added DATETIME)";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        
        // other methods to be added as needed
    }
}
