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

            // Ensure that the Standard Tasks table is created
            db.CreateStandardTasksTable();

            // If there are no command-line arguments
            if (args.Length == 0)
            {
                Console.WriteLine("No command-line arguments provided, printing manual page from `plannercli -h");
                PrintHelp();
                return;
            }

            // Parse command-line arguments

            // Check if the first argument is the "add" command
            if (args[0].Equals("add", StringComparison.OrdinalIgnoreCase))
            {
                // Remove the "add" command from the arguments
                args = args.Skip(1).ToArray();

                // Parse command-line arguments
                string taskName = string.Empty;
                string note = string.Empty;
                DateTime due = DateTime.MaxValue;
                int priority = -1;


                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "-t":
                        case "-T":
                        case "--task":
                        case "--Task":
                            taskName = args[++i];
                            break;
                        case "-p":
                        case "-P":
                        case "--priority":
                        case "--Priority":
                            if (int.TryParse(args[++i], out int parsedPriority))
                            {
                                priority = parsedPriority;
                            }
                            else
                            {
                                Console.WriteLine("Invalid priority. Please enter a valid integer.");
                                return; // Exit if priority is invalid
                            }
                            break;
                        case "-d":
                        case "-D":
                        case "--due":
                        case "--Due":
                            DateTime.TryParse(args[++i], out due);
                            break;
                        case "-n":
                        case "-N":
                        case "--note":
                        case "--Note":
                            note = args[++i];
                            break;
                        default:
                            Console.WriteLine($"Unknown argument: {args[i]}");
                            break;
                    }
                }

                // Validate input
                if (string.IsNullOrWhiteSpace(taskName))
                {
                    Console.WriteLine("Task name is required.");
                    Console.WriteLine("Enter task name:");
                    taskName = Console.ReadLine();
                    if (string.IsNullOrEmpty(taskName))
                    {
                        Console.WriteLine("Task name could not be registered, string null or empty.");
                        return;
                    }
                }
                // Create a Standard Task
                StandardTaskModel task = new StandardTaskModel
                {
                    TaskName = taskName,
                    Due = due,
                    Priority = priority,
                    Note = note,
                    Added = DateTime.Now,
                };

                // Add the task to the database
                taskRepository.AddTask(task);

                Console.WriteLine("Task added successfully!");
            }

            /*
             * Use for Testing later on:
             * 
             * Create a sample task
            StandardTaskModel task = new StandardTaskModel
            {
                TaskName = "Sample Task",
                Due = DateTime.Now.AddDays(1),
                Priority = 80,
                Note = "This is a sample task",
                Added = DateTime.Now
            };
            */
        }
        public static void PrintHelp()
        {
            Console.WriteLine("TODO: Write man page");
            return;
        }
    }
}