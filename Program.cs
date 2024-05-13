using System;
using System.Linq;
using plannerCLI.Repositories;
using plannerCLI.Services;
using plannerCLI.UI;
using plannerCLI.Models;

namespace plannerCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            SQLiteDatabase db = new SQLiteDatabase();
            TaskRepository taskRepository = new TaskRepository();
            TaskService taskService = new TaskService(taskRepository);

            // Ensure the database and table are properly set up.
            db.CreateStandardTasksTable();

            // Handle specific commands
            if (args.Length > 0)
            {
                switch (args[0].ToLower())
                {
                    case "add":
                        taskService.HandleAddTask(args.Skip(1).ToArray());
                        break;
                    case "new":
                        taskService.HandleNewTask();
                        break;
                    case "update":
                        taskService.HandleUpdateTask(args.Skip(1).ToArray());
                        break;
                    case "delete":
                    case "remove":
                    case "complete":
                    case "finish":
                        if (args.Length > 1 && int.TryParse(args[1], out int taskId))
                        {
                            taskService.DeleteTask(taskId);
                        }
                        else
                        {
                            Console.WriteLine("Error occurred, possible cause: Please provide a valid task ID.");
                        }
                        break;
                    case "-h":
                    case "--help":
                        OutputHandler.PrintHelp();
                        break;
                    default:
                        DisplayTasksOrDefaultToHelp(taskService);
                        break;
                }
            }
            else
            {
                DisplayTasksOrDefaultToHelp(taskService);
            }
        }

        static void DisplayTasksOrDefaultToHelp(TaskService taskService)
        {
            var tasks = taskService.GetAllTasks();
            if (tasks.Count > 0)
            {
                OutputHandler.PrintTasks(tasks);
            }
            else
            {
                OutputHandler.PrintHelp(); // Print help if no tasks are found
            }
        }
    }
}
