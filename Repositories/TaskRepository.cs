using plannerCLI.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plannerCLI.Repositories
{
    public class TaskRepository
    {
        private SQLiteDatabase db = new SQLiteDatabase();

        // insert a task to SQLite
        // plannercli add -t name -d duedate -p 10 -n note
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

        // return a single task from an ID number
        // used in the update function to search for the appropriate row
        public StandardTaskModel GetTask(int id)
        {
            using (var connection = new SQLiteConnection(db.connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM Tasks WHERE ID = @Id";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Create a new task based on the retrieved data
                            StandardTaskModel task = new StandardTaskModel
                            {
                                Id = Convert.ToInt32(reader["ID"]),
                                TaskName = Convert.ToString(reader["TaskName"]),
                                Due = Convert.ToString(reader["Due"]),
                                Priority = Convert.ToInt32(reader["Priority"]),
                                Note = Convert.ToString(reader["Note"]),
                                Added = Convert.ToString(reader["Added"])
                            };
                            return task;
                        }
                        else
                        {
                            // If no task with the given ID is found, return null
                            return null;
                        }
                    }
                }
            }
        }


        // view tasklist from SQLite Standard Tasks table
        // plannercli view
        public List<StandardTaskModel> GetAllTasks() 
        {
            List<StandardTaskModel> tasks = new List<StandardTaskModel>();
            using (var connection = new SQLiteConnection(db.connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM Tasks";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read()) 
                        {
                            // declare new task structure for each result
                            StandardTaskModel task = new StandardTaskModel
                            {
                                Id = Convert.ToInt32(reader["ID"]),
                                TaskName = Convert.ToString(reader["TaskName"]),
                                Due = Convert.ToString(reader["Due"]),
                                Priority = Convert.ToInt32(reader["Priority"]),
                                Note = Convert.ToString(reader["Note"]),
                                Added = Convert.ToString(reader["Added"])
                            };

                            // add tasks to the list to return
                            tasks.Add(task);
                        }
                    }
                }
            }
            return tasks;
        }

        // Update an existing task in SQLite
        public void UpdateTask(StandardTaskModel task)
        {
            using (var connection = new SQLiteConnection(db.connectionString))
            {
                connection.Open();
                string sql = @"UPDATE Tasks 
                       SET TaskName = @TaskName, 
                           Due = @Due, 
                           Priority = @Priority, 
                           Note = @Note, 
                           Added = @Added 
                       WHERE ID = @Id";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@TaskName", task.TaskName);
                    command.Parameters.AddWithValue("@Due", task.Due);
                    command.Parameters.AddWithValue("@Priority", task.Priority);
                    command.Parameters.AddWithValue("@Note", task.Note);
                    command.Parameters.AddWithValue("@Added", task.Added);
                    command.Parameters.AddWithValue("@Id", task.Id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteTask(int taskId)
        {
            using (var connection = new SQLiteConnection(db.connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM Tasks WHERE ID = @Id";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", taskId);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine($"Task with ID {taskId} deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Task with ID {taskId} not found");
                    }
                }
            }
        }
    }
}
