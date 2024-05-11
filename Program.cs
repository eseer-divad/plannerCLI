// PlannerCLI Interactive/Animated Task Management CLI Application
// See https://aka.ms/new-console-template for more information on C# Console Apps
using System;
using plannerCLI.Repositories;
using plannerCLI.Models;

namespace plannerCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            SQLiteDatabase db = new SQLiteDatabase();
            TaskRepository taskRepository = new TaskRepository();

            // Ensure that the Tasks table is created
            db.CreateStandardTasksTable();

            // Create a sample task
            StandardTaskModel task = new StandardTaskModel
            {
                TaskName = "Sample Task",
                Due = DateTime.Now.AddDays(1),
                Priority = 80,
                Note = "This is a sample task",
                Added = DateTime.Now
            };

            // Add the task to the database
            taskRepository.AddTask(task);

            Console.WriteLine("Task added successfully!");
        }
    }
}