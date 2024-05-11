using plannerCLI.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plannerCLI.Repositories
{
    public class TaskRepository
    {
        private SQLiteDatabase db = new SQLiteDatabase();
        public void AddTask(StandardTaskModel task)
        {
            db.CreateStandardTasksTable();
            using (var connection = new SQLiteConnection(db.connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO Tasks (TaskName, Due, Priority, Note, Added) VALUES (@TaskName, @Due, @Priority, @Note, @Added)";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@TaskName", task.TaskName);
                    command.Parameters.AddWithValue("@Due", task.Due);
                    command.Parameters.AddWithValue("@Priority", task.Priority);
                    command.Parameters.AddWithValue("@Note", task.Note);
                    command.Parameters.AddWithValue("@Added", task.Added);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
