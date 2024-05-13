using System;
using System.Collections.Generic;

namespace plannerCLI.UI
{
    public static class PromptHandler
    {
        public static int DisplayMultipleChoicePrompt(string question, List<string> options)
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

        public static string DisplayDateInputPrompt(string prompt)
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

        private static void IncrementComponent(ref string component, int position)
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

        private static void DecrementComponent(ref string component, int position)
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
