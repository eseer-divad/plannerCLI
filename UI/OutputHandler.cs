using System;
using plannerCLI.Models;
using System.Collections.Generic;

namespace plannerCLI.UI
{
    public static class OutputHandler
    {
        public static void PrintTasks(List<StandardTaskModel> tasks)
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

        public static void PrintHelp()
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
    }
}
