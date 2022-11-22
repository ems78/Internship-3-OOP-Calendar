using MailCalendar.classes;
using System;
using System.Collections.Generic;

namespace MailCalendar
{
    public class Program
    {
        static void MainMenu(Dictionary<string, List<string>> MenuOptions, List<Event> activeEvents)
        {
            Console.Clear();
            int i = 0;
            foreach (var element in MenuOptions["main menu"])
            {
                Console.WriteLine($"{++i} - {element}");
            }

            while (true)
            {
                switch (UserChoice("a number to navigate the menu"))
                {
                    case 1:  // print active events
                        Console.Clear();
                        ActiveEvents(MenuOptions, activeEvents);
                        break;

                    case 2:  // print upcoming evetns
                        Console.Clear();

                        break;

                    case 3:  // print past events
                        Console.Clear();

                        break;

                    case 4:  // create new event
                        Console.Clear();

                        break;

                    case 5:  // exit application
                        Environment.Exit(0);
                        break;
                        
                    default:
                        break;
                }
            }
        }


        static void ActiveEvents(Dictionary<string, List<string>> MenuOptions, List<Event> activeEvents)
        {
            Console.WriteLine("Ative events");
            PrintLine();
            foreach (Event activeEvent in activeEvents)
            {
                Console.WriteLine($"\nID: {activeEvent.Id}\nName: {activeEvent.Name}\tLocation: {activeEvent.Location} Ends in: {activeEvent.EndsIn()}\nParticipans:");
                if (activeEvent.EmailList.Count is 0)
                {
                    Console.WriteLine("No paticipants");
                    continue;
                }
                foreach (var participant in activeEvent.EmailList.Keys)
                {
                    Console.WriteLine($"{participant}\n");
                }
            }
            PrintLine();
            ReturnToMainMenu(MenuOptions, activeEvents);
        }

        
        static int UserChoice(string typeOfChoice)
        {
            int choice;
            do
            {
                Console.Write($"\nEnter {typeOfChoice}: ");
            } while (!int.TryParse(Console.ReadLine(), out choice));
            return choice;
        }


        static void ReturnToMainMenu(Dictionary<string, List<string>> MenuOptions, List<Event> activeEvents)
        { 
            int back;
            while ((back = UserChoice("0 to return to main menu")) != 0)
            {
                Console.WriteLine("There is no such option.");
            }
            MainMenu(MenuOptions, activeEvents);
        }


        static void Main(string[] args)
        {
            
            List<Event> activeEvents = new List<Event>
            {
                new Event("Skate Contest", "Koteks", new DateTime(2022, 12, 11, 14, 30, 00), new DateTime(2022, 12, 11, 21, 15, 00)),
                new Event("Graffiti jam", "Koteks", new DateTime(2022, 12, 11, 11, 30, 00), new DateTime(2022, 12, 11, 14, 00, 00)),
                new Event("Blood Incantation concert", "--", new DateTime(2023, 2, 11, 21, 45, 00), new DateTime(2023, 2, 11, 1, 00, 00)),
                new Event("--", "--", new DateTime(2022, 12, 11, 14, 30, 00), new DateTime(2022, 12, 11, 21, 15, 00)),
                new Event("--", "--", new DateTime(2022, 12, 11, 14, 30, 00), new DateTime(2022, 12, 11, 21, 15, 00)) 
            };
            List<Person> people = new List<Person>();

            Dictionary<string, List<string>> menuOptions = new Dictionary<string, List<string>>()
            {
                {"main menu", new List<string> { "Active events", "Upcoming events", "Past events", "Create a new event", "Exit"} },
                {"active events submenu", new List<string> { "Ckeck attendance", "Return to main menu"} },
                {"upcoming events submenu", new List<string> { "Delete event", "Remove people from an event", "Return to main menu"} },
            };

            MainMenu(menuOptions, activeEvents);

            Console.ReadKey();

        }

        static void PrintLine()
        {
            Console.WriteLine("------------------------------------------\n");
        }
    }
}
