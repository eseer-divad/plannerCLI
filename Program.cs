using System;
using System.Collections.Generic;
using System.Linq;
using plannerCLI.Repositories;
using plannerCLI.Models;
using System.Threading.Tasks;

namespace plannerCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            SQLiteDatabase db = new SQLiteDatabase();
            TaskRepository taskRepository = new TaskRepository();

            // Ensure the database and table are properly set up.
            db.CreateStandardTasksTable();

            // Handle specific commands
            if (args.Length > 0)
            {
                if (args[0].Equals("add", StringComparison.OrdinalIgnoreCase))
                {
                    HandleAddTask(args.Skip(1).ToArray(), taskRepository);
                }
                else if (args[0].Equals("new", StringComparison.OrdinalIgnoreCase))
                {
                    HandleNewTask(args.Skip(1).ToArray(), taskRepository);
                }
                else if (args[0].Equals("update", StringComparison.OrdinalIgnoreCase))
                {
                    HandleUpdateTask(args.Skip(1).ToArray(), taskRepository);
                }
                else if (args[0].Equals("delete", StringComparison.OrdinalIgnoreCase) ||
                         args[0].Equals("remove", StringComparison.OrdinalIgnoreCase) ||
                         args[0].Equals("complete", StringComparison.OrdinalIgnoreCase) ||
                         args[0].Equals("finish", StringComparison.OrdinalIgnoreCase))
                {
                    if (args.Length > 1 && int.TryParse(args[1], out int taskId))
                    {
                        taskRepository.DeleteTask(taskId);
                    }
                    else
                    {
                        Console.WriteLine("Error occurred, possible cause: Please provide a valid task ID.");
                    }
                }
                else if (args[0] == "-h" || args[0] == "-H" || args[0] == "--help" || args[0] == "--Help")
                {
                    PrintHelp();
                }
                else
                {
                    DisplayTasksOrDefaultToHelp(taskRepository);
                }
            }
            else
            {
                DisplayTasksOrDefaultToHelp(taskRepository);
            }
        }

        static void PrintHelp()
        {
            Console.WriteLine("---------------------------------------------------------------------------");
            Console.WriteLine("plannerCLI: by David Reese (eseer-divad)");
            Console.WriteLine("Description: A command line task management / day planner application.");
            Console.WriteLine("---------------------------------------------------------------------------");
            Console.WriteLine("Commands:");
            Console.WriteLine("`plannercli`            | View Task List");
            Console.WriteLine("`plannercli -h`         | View This Help Page");
            Console.WriteLine();
            Console.WriteLine("`plannercli add [options]` | Add Task to List");
            Console.WriteLine("Options for 'add':");
            Console.WriteLine("  -t, --task [taskname]   | Specify the task name.");
            Console.WriteLine("  -p, --priority [level]  | Specify the task priority.");
            Console.WriteLine("  -d, --due [duedate]     | Specify the due date (format YYYY-MM-DD).");
            Console.WriteLine("  -n, --note [note]       | Specify additional notes.");
            Console.WriteLine("---------------------------------------------------------------------------");
        }

        static void HandleAddTask(string[] args, TaskRepository taskRepository)
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

            taskRepository.AddTask(task);
            Console.WriteLine("Task added successfully!");
            Console.WriteLine();
            DisplayTasksOrDefaultToHelp(taskRepository);
        }

        static void HandleNewTask(string[] args, TaskRepository taskRepository)
        {
            // Query to ask for task name (REQUIRED)
            string taskName = GetInput("Enter the new task name: ");
            if (string.IsNullOrWhiteSpace(taskName))
            {
                Console.WriteLine("A task name is required.");
                return;
            }

            // Due Date (Interactive MM:DD - HH:MM:SS, console key up/down, side to side + enter)
            string due = DisplayDateInputPrompt("Set the due date (MM:DD - HH:MM:SS):");

            // Priority (0-100 in 10 choices, updown keys + enter)
            string priorityPrompt = "Select a value 0-100 in ascending priority. (up/down arrows + enter)";
            List<string> opts = new List<string> { "00", "10", "20", "30", "40", "50", "60", "70", "80", "90", "100" };
            int priorityIndex = DisplayMultipleChoicePrompt(priorityPrompt, opts);
            int priority = int.Parse(opts[priorityIndex]);

            // Add a note
            string note = GetInput("Add a note (optional): ");

            var task = new StandardTaskModel
            {
                TaskName = taskName,
                Due = due,
                Priority = priority,
                Note = note,
                Added = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            taskRepository.AddTask(task);
            Console.WriteLine("Task added successfully!");
            Console.WriteLine();
            DisplayTasksOrDefaultToHelp(taskRepository);
        }

        static string GetInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        static void HandleUpdateTask(string[] args, TaskRepository taskRepository)
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
            StandardTaskModel TaskToUpdate = taskRepository.GetTask(taskID);
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
                taskRepository.UpdateTask(task);
                Console.WriteLine("Task updated successfully!");
                Console.WriteLine();
                DisplayTasksOrDefaultToHelp(taskRepository);
            }
        }

        static void DisplayTasksOrDefaultToHelp(TaskRepository taskRepository)
        {
            var tasks = taskRepository.GetAllTasks();
            if (tasks.Count > 0)
            {
                Console.WriteLine("Tasks:");
                Console.WriteLine("------------------------------------------------------------------------------------------------------");
                Console.WriteLine($"| ID  | Task Name            | Due Date          | Priority | Note");
                Console.WriteLine("------------------------------------------------------------------------------------------------------");

                ConsoleColor[] rowColors = { ConsoleColor.Black, ConsoleColor.Black };

                for (int i = 0; i < tasks.Count; i++)
                {
                    var task = tasks[i];
                    Console.BackgroundColor = rowColors[i % 2]; // Alternate background colors

                    // Format and pad each column properly to avoid overlapping colors
                    Console.WriteLine($"| {task.Id.ToString().PadRight(3)} | {task.TaskName.PadRight(21)} | {task.Due.PadRight(17)} | {task.Priority.ToString().PadRight(8)} | {task.Note.PadRight(38)} ");
                }

                Console.ResetColor(); // Reset console color after printing
                Console.WriteLine("------------------------------------------------------------------------------------------------------");
            }
            else
            {
                PrintHelp(); // Print help if no tasks are found
            }
        }

        static int DisplayMultipleChoicePrompt(string question, List<string> options)
        {
            int selectedIndex = 0; // Default to the first option

            ConsoleKey key;
            do
            {
                Console.Clear();
                Console.WriteLine(question);
                Console.WriteLine();

                for (int i = 0; i < options.Count; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    Console.WriteLine($"{options[i]}");
                    Console.ResetColor();
                }

                Console.WriteLine();
                Console.WriteLine("Use the arrow keys to navigate and press Enter to select.");

                key = Console.ReadKey().Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex > 0) ? selectedIndex - 1 : options.Count - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex < options.Count - 1) ? selectedIndex + 1 : 0;
                        break;
                }
            } while (key != ConsoleKey.Enter);
            return selectedIndex;
        }

        static string DisplayDateInputPrompt(string prompt)
        {
            // Initialize date components as strings
            string[] dateComponents = new string[]
            {
                "01", // Month
                "01", // Day
                "00", // Hour
                "00", // Minute
                "00"  // Second
            };

            int position = 0;
            ConsoleKey key;
            do
            {
                Console.Clear();
                Console.WriteLine(prompt);
                Console.WriteLine();

                // Display date components
                for (int i = 0; i < dateComponents.Length; i++)
                {
                    if (i == position)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.ResetColor();
                    }

                    Console.Write(dateComponents[i]);

                    if (i == 0 || i == 1)
                    {
                        Console.Write(":");
                    }
                    else if (i == 2)
                    {
                        Console.Write(" - ");
                    }
                    else if (i < dateComponents.Length - 1)
                    {
                        Console.Write(":");
                    }
                }
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("Use the arrow keys to navigate and modify the date, press Enter to confirm.");

                key = Console.ReadKey().Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        IncrementComponent(ref dateComponents[position], position);
                        break;
                    case ConsoleKey.DownArrow:
                        DecrementComponent(ref dateComponents[position], position);
                        break;
                    case ConsoleKey.LeftArrow:
                        position = (position > 0) ? position - 1 : dateComponents.Length - 1;
                        break;
                    case ConsoleKey.RightArrow:
                        position = (position < dateComponents.Length - 1) ? position + 1 : 0;
                        break;
                }

            } while (key != ConsoleKey.Enter);

            // Combine the date components into the final string format
            return $"{dateComponents[0]}:{dateComponents[1]} - {dateComponents[2]}:{dateComponents[3]}:{dateComponents[4]}";
        }

        static void IncrementComponent(ref string component, int position)
        {
            int value = int.Parse(component);
            switch (position)
            {
                case 0: // Month
                    value = (value < 12) ? value + 1 : 1;
                    break;
                case 1: // Day
                    value = (value < 31) ? value + 1 : 1;
                    break;
                case 2: // Hour
                    value = (value < 23) ? value + 1 : 0;
                    break;
                case 3: // Minute
                    value = (value < 59) ? value + 1 : 0;
                    break;
                case 4: // Second
                    value = (value < 59) ? value + 1 : 0;
                    break;
            }
            component = value.ToString("00");
        }

        static void DecrementComponent(ref string component, int position)
        {
            int value = int.Parse(component);
            switch (position)
            {
                case 0: // Month
                    value = (value > 1) ? value - 1 : 12;
                    break;
                case 1: // Day
                    value = (value > 1) ? value - 1 : 31;
                    break;
                case 2: // Hour
                    value = (value > 0) ? value - 1 : 23;
                    break;
                case 3: // Minute
                    value = (value > 0) ? value - 1 : 59;
                    break;
                case 4: // Second
                    value = (value > 0) ? value - 1 : 59;
                    break;
            }
            component = value.ToString("00");
        }
    }
}
