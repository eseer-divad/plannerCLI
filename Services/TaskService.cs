using plannerCLI.Models;
using plannerCLI.Repositories;
using plannerCLI.UI;
using System;
using System.Collections.Generic;

namespace plannerCLI.Services
{
    public class TaskService
    {
        private readonly TaskRepository _taskRepository;

        public TaskService(TaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public void HandleAddTask(string[] args)
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

            AddTask(task);
            Console.WriteLine("Task added successfully!");
            Console.WriteLine();
            DisplayTasksOrDefaultToHelp();
        }

        public void HandleNewTask()
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

            AddTask(task);
            Console.WriteLine("Task added successfully!");
            Console.WriteLine();
            DisplayTasksOrDefaultToHelp();
        }

        public void HandleUpdateTask(string[] args)
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
            StandardTaskModel TaskToUpdate = GetTask(taskID);
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

                UpdateTask(task);
                Console.WriteLine("Task updated successfully!");
                Console.WriteLine();
                DisplayTasksOrDefaultToHelp();
            }
        }

        public void AddTask(StandardTaskModel task)
        {
            _taskRepository.AddTask(task);
        }

        public void UpdateTask(StandardTaskModel task)
        {
            _taskRepository.UpdateTask(task);
        }

        public void DeleteTask(int taskId)
        {
            _taskRepository.DeleteTask(taskId);
        }

        public StandardTaskModel GetTask(int id)
        {
            return _taskRepository.GetTask(id);
        }

        public List<StandardTaskModel> GetAllTasks()
        {
            return _taskRepository.GetAllTasks();
        }

        private void DisplayTasksOrDefaultToHelp()
        {
            var tasks = GetAllTasks();
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
