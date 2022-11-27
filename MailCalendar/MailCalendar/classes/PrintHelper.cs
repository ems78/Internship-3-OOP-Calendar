using System;

namespace MailCalendar.classes
{
    public static class PrintHelper
    {
        public static void PrintLine()
        {
            Console.WriteLine("------------------------------------------\n");
        }

        public static void MessageAboutAction(string typeOfAction)
        {
            Console.Write($"\n{typeOfAction}\nPress any key to continue.");
            Console.ReadKey();
            Console.Clear();
        }
    }
}
