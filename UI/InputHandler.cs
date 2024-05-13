using System;

namespace plannerCLI.UI
{
    public static class InputHandler
    {
        public static string GetInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
    }
}
