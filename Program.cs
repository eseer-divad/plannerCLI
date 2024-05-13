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
                if (args[0].Equals("add", StringComparison.OrdinalIgnoreCase))
                {
                    HandleAddTask(args.Skip(1).ToArray(), taskService);
                }
                else if (args[0].Equals("new", StringComparison.OrdinalIgnoreCase))
                {
                    HandleNewTask(args.Skip(1).ToArray(), taskService);
                }
                else if (args[0].Equals("update", StringComparison.OrdinalIgnoreCase))
                {
                    HandleUpdateTask(args.Skip(1).ToArray(), taskService);
                }
                else if (args[0].Equals("delete", StringComparison.OrdinalIgnoreCase) ||
                         args[0].Equals("remove", StringComparison.OrdinalIgnoreCase) ||
                         args[0].Equals("complete", StringComparison.OrdinalIgnoreCase) ||
                         args[0].Equals("finish", StringComparison.OrdinalIgnoreCase))
                {
                    if (args.Length > 1 && int.TryParse(args[1], out int taskId))
                    {
                        taskService.DeleteTask(taskId);
                    }
                    else
                    {
                        Console.WriteLine("Error occurred, possible cause: Please provide a valid task ID.");
                    }
                }
                else if (args[0] == "-h" || args[0] == "-H" || args[0] == "--help" || args[0] == "--Help")
                {
                    OutputHandler.PrintHelp();
                }
                else
                {
                    DisplayTasksOrDefaultToHelp(taskService);
                }
            }
            else
            {
                DisplayTasksOrDefaultToHelp(taskService);
            }
        }

        static void HandleAddTask(string[] args, TaskService taskService)
        {
            string taskName = null, due = null, note = null;
            int priority = -1;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-t":
                    case "--task":
                        taskName = args[++i];
                        break;
                    case "-p":
                    case "--priority":
                        if (int.TryParse(args[++i], out int parsedPriority))
                        {
                            priority = parsedPriority;
                        }
                        else
                        {
                            Console.WriteLine("Invalid priority. Please enter a valid integer.");
                            return;
                        }
                        break;
                    case "-d":
                    case "--due":
                        due = args[++i];
                        break;
                    case "-n":
                    case "--note":
                        note = args[++i];
                        break;
                    default:
                        Console.WriteLine($"Unknown argument: {args[i]}");
                        break;
                }
            }

            if (string.IsNullOrWhiteSpace(taskName))
            {
                Console.WriteLine("Task name is required.");
                return;
            }

            var task = new StandardTaskModel
            {
                TaskName = taskName,
                Due = due,
                Priority = priority,
                Note = note,
                Added = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            taskService.AddTask(task);
            Console.WriteLine("Task added successfully!");
            Console.WriteLine();
            DisplayTasksOrDefaultToHelp(taskService);
        }

        static void HandleNewTask(string[] args, TaskService taskService)
        {
            // Query to ask for task name (REQUIRED)
            string taskName = InputHandler.GetInput("Enter the new task name: ");
            if (string.IsNullOrWhiteSpace(taskName))
            {
                Console.WriteLine("A task name is required.");
                return;
            }

            // Due Date (Interactive MM:DD - HH:MM:SS, console key up/down, side to side + enter)
            string due = PromptHandler.DisplayDateInputPrompt("Set the due date (MM:DD - HH:MM:SS):");

            // Priority (0-100 in 10 choices, updown keys + enter)
            string priorityPrompt = "Select a value 0-100 in ascending priority. (up/down arrows + enter)";
            List<string> opts = new List<string> { "00", "10", "20", "30", "40", "50", "60", "70", "80", "90", "100" };
            int priorityIndex = PromptHandler.DisplayMultipleChoicePrompt(priorityPrompt, opts);
            int priority = int.Parse(opts[priorityIndex]);

            // Add a note
            string note = InputHandler.GetInput("Add a note (optional): ");

            var task = new StandardTaskModel
            {
                TaskName = taskName,
                Due = due,
                Priority = priority,
                Note = note,
                Added = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            taskService.AddTask(task);
            Console.WriteLine("Task added successfully!");
            Console.WriteLine();
            DisplayTasksOrDefaultToHelp(taskService);
        }

        static void HandleUpdateTask(string[] args, TaskService taskService)
        {
            string taskName = null, due = null, note = null, dateAdded = null;
            int taskID = -1;
            int priority = -1;

            // try to convert the next argument into a number
            try
            {
                taskID = Convert.ToInt32(args[0]);
            }
            catch
            {
                Console.WriteLine("Could not convert update command's ID parameter from `plannercli update [ID#]` to an integer.");
                Console.WriteLine("Only specify task ID numbers.");
                return;
            }

            // try to find a task with the specified ID
            StandardTaskModel TaskToUpdate = taskService.GetTask(taskID);
            if (TaskToUpdate == null)
            {
                Console.WriteLine("Specified ID for task not found in database.");
                return;
            }
            else
            {
                // store all old values in local variables
                taskName = TaskToUpdate.TaskName;
                due = TaskToUpdate.Due;
                note = TaskToUpdate.Note;
                priority = TaskToUpdate.Priority ?? -1;
                dateAdded = TaskToUpdate.Added;

                // check the next command line args for updated info
                for (int i = 1; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "-t":
                        case "--task":
                            taskName = args[++i];
                            break;
                        case "-p":
                        case "--priority":
                            if (int.TryParse(args[++i], out int parsedPriority))
                            {
                                priority = parsedPriority;
                            }
                            else
                            {
                                Console.WriteLine("Invalid priority. Please enter a valid integer.");
                                return;
                            }
                            break;
                        case "-d":
                        case "--due":
                            due = args[++i];
                            break;
                        case "-n":
                        case "--note":
                            note = args[++i];
                            break;
                        case "-a":
                        case "--added":
                            dateAdded = args[++i];
                            break;
                        default:
                            Console.WriteLine($"Unknown argument: {args[i]}");
                            break;
                    }
                }

                // construct a new task with the old and updated info
                var task = new StandardTaskModel
                {
                    Id = taskID, // Ensure the task ID is set correctly
                    TaskName = taskName,
                    Due = due,
                    Priority = priority,
                    Note = note,
                    Added = dateAdded
                };

                // update task repository with new structure
                taskService.UpdateTask(task);
                Console.WriteLine("Task updated successfully!");
                Console.WriteLine();
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
