using MailCalendar.classes;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MailCalendar
{
    public class Program
    {
        static void MainMenu(Dictionary<string, List<string>> MenuOptions, Hashtable tableOfEvents, Dictionary<string, Person> people)
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
                        Console.WriteLine("\tActive events");
                        PrintLine();
                        ActiveEvents(MenuOptions, tableOfEvents, people);
                        break;

                    case 2:  // print upcoming evetns
                        Console.Clear();
                        Console.WriteLine("\tUpcoming events");
                        PrintLine();
                        UpcomingEvents(MenuOptions, tableOfEvents, people);
                        break;

                    case 3:  // print past events
                        Console.Clear();
                        Console.WriteLine("\tPast events");
                        PrintLine();
                        PastEvents(MenuOptions, tableOfEvents, people);
                        break;

                    case 4:  // create new event
                        Console.Clear();
                        Console.WriteLine("\tCreate a new event");
                        PrintLine();
                        CreateEvent(MenuOptions, tableOfEvents, people);
                        break;

                    case 5:  
                        Environment.Exit(0);
                        break;
                        
                    default:
                        break;
                }
            }
        }


        static void ActiveEvents(Dictionary<string, List<string>> MenuOptions, Hashtable tableOfEvents, Dictionary<string, Person> people)
        {
            foreach (Event element in tableOfEvents.Values)
            {
                if (element.GetStatus() == "active")
                {
                    Console.WriteLine($"ID: {element.Id}\nName: {element.Name}\nLocation: {element.Location} Ends in: {element.GetDuration("active")}\n\nParticipans:");
                    if (element.EmailList.Count is 0) 
                    {
                        Console.WriteLine("No paticipants\n");
                        PrintLine();
                        continue;
                    }
                    string[] attendeeEmails = element.PrintAttendances(true).Split(' ');
                    foreach (var email in attendeeEmails)
                    {
                        Console.WriteLine($"{email}\n");
                    }
                    PrintLine();
                }
            }

            while (true)
            {
                int i = 0;
                foreach (var element in MenuOptions["active events submenu"])
                {
                    Console.WriteLine($"{++i} - {element}");
                }
                switch (UserChoice("a number to navigate the menu"))
                {
                    case 1:  // check attendance
                        CheckUnattendace(MenuOptions, tableOfEvents, people);
                        Console.Clear();
                        break;

                    case 2:  
                        MainMenu(MenuOptions, tableOfEvents, people);
                        break;

                    default:
                        break;
                }
            }
        }


        static void CheckUnattendace(Dictionary<string, List<string>> MenuOptions, Hashtable tableOfEvents, Dictionary<string, Person> people)  // unosenje ID-a??
        {
            Console.Write("Enter ID of event you want to check attendences for: ");
            string eventIDtoCheck = Console.ReadLine();
            if (!tableOfEvents.ContainsKey(eventIDtoCheck))
            {
                return;
            }
            Event currentEvent = (Event)tableOfEvents[eventIDtoCheck];

            //unos emailova osoba koje nisu prisutne
            Console.Write("Enter emails of everyone that isn't attending the event (separated by a whitespace): ");
            string emailsOfUnattendance = Console.ReadLine(); // ime prominit

            string isSuccessful = currentEvent.NotAttending(emailsOfUnattendance);
            if (isSuccessful.Length > 0)    
            {
                string[] wrongEmailsString = isSuccessful.Split(' ');
                foreach (var item in wrongEmailsString)
                {
                    Console.WriteLine($"{item} ");
                }
                Console.WriteLine("are not in the attendance list of this event.");   
            }
            //Console.Write("Press any key to continue.");
            //Console.ReadKey();
        }


        static void UpcomingEvents(Dictionary<string, List<string>> MenuOptions, Hashtable tableOfEvents, Dictionary<string, Person> people)
        {
            foreach (Event element in tableOfEvents.Values)
            {
                if (element.GetStatus() == "upcoming")
                {
                    Console.WriteLine($"\nID: {element.Id}\nName: {element.Name}\nLocation: {element.Location} \nEnds in: {element.GetDuration("upcoming")}\n\nParticipans:");
                    if (element.EmailList.Count is 0)
                    {
                        Console.WriteLine("No paticipants");
                        PrintLine();
                        continue;
                    }
                    string[] attendeeEmails = element.PrintAttendances(true).Split(' ');
                    foreach (var email in attendeeEmails)
                    {
                        Console.WriteLine($"{email}\n");
                    }
                    PrintLine();
                }
            }
            int i = 0;
            foreach (var element in MenuOptions["upcoming events submenu"])
            {
                Console.WriteLine($"{++i} - {element}");
            }

            while (true)
            {
                switch (UserChoice("a number to navigate the menu"))
                {
                    case 1:  
                        DeleteEvent(tableOfEvents,people);
                        break;

                    case 2:  
                        RemovePeopleFromEvent(tableOfEvents, people);
                        break;

                    case 3: 
                        MainMenu(MenuOptions, tableOfEvents, people);
                        break;

                    default:
                        break;
                }
            }
        }


        static void DeleteEvent(Hashtable tableOfEvents, Dictionary<string, Person> people)
        {
            Console.Write("Enter ID of event you want to delete: ");
            string eventIDtoDelete = Console.ReadLine();
            if (tableOfEvents.ContainsKey(eventIDtoDelete))
            {
                Event eventToDelete = (Event)tableOfEvents[eventIDtoDelete];
                if (eventToDelete.GetStatus() != "upcoming")
                {
                    Console.Write("You can only delete an upcoming event.\nPress any key to continue.");
                    Console.ReadKey();
                    return;
                }

                // brisati podatke o prisutnosti na tom eventu
                foreach (string email in eventToDelete.EmailList.Keys)
                {
                    people[email].Attendance.Remove(eventIDtoDelete);
                }
                // izbrisat event
                tableOfEvents.Remove(eventIDtoDelete);
                return;
            }
            Console.WriteLine("ID doesn't match any event!");
            return;
        }


        static void RemovePeopleFromEvent(Hashtable tableOfEvents, Dictionary<string, Person> people)
        {
            Console.Write("Enter ID of event you want to remove people from: ");
            string eventIDtoEdit = Console.ReadLine();
            if (tableOfEvents.ContainsKey(eventIDtoEdit))
            {
                Event eventToEdit = (Event)tableOfEvents[eventIDtoEdit];

                Console.Write("Enter emails of people you want to remove from this event (separated by a whitespace): ");
                string input = Console.ReadLine();
                string[] emailsToRemove = input.Split(' ');
                List<string> wrongInput = new List<string>();
                foreach (var email in emailsToRemove)
                {
                    if (!eventToEdit.EmailList.ContainsKey(email.Trim()))
                    {
                        wrongInput.Add(email);
                        continue;
                    }
                    eventToEdit.EmailList.Remove(email);  // izbrisat podatke o prisutnosti
                    people[email].Attendance.Remove(eventIDtoEdit);
                }
                return;
            }
        }


        static void PastEvents(Dictionary<string, List<string>> MenuOptions, Hashtable tableOfEvents, Dictionary<string, Person> people)
        {
            foreach (Event element in tableOfEvents.Values)
            {
                if (element.GetStatus() == "past")
                {
                    Console.WriteLine($"\nID: {element.Id}\nName: {element.Name}\nLocation: {element.Location}\nEnded: {element.GetDuration("past")}\n\nParticipans:");
                    if (element.EmailList.Count is 0)
                    {
                        Console.WriteLine("No paticipants");
                        continue;
                    }
                    string[] atendeeEmails = element.PrintAttendances(true).Split(' ');
                    foreach (var email in atendeeEmails)
                    {
                        Console.WriteLine($"{email}\n");
                    }
                    Console.WriteLine("\nAbsent participants:\n");
                    string[] absentAtendeeEmails = element.PrintAttendances(false).Split(' ');
                    foreach (var email in absentAtendeeEmails)
                    {
                        Console.WriteLine($"{email}\n");
                    }
                }
            }

            PrintLine();
            int i = 0;
            foreach (var element in MenuOptions["past events submenu"])
            {
                Console.WriteLine($"{++i} - {element}");
            }

            while (true)
            {
                switch (UserChoice("a number to navigate the menu"))
                {
                    case 1: // return to main menu
                        MainMenu(MenuOptions, tableOfEvents, people);
                        break;

                    default:
                        break;
                }
            }
        }


        static void CreateEvent(Dictionary<string, List<string>> MenuOptions, Hashtable tableOfEvents, Dictionary<string, Person> people)
        {
            Console.Write("Enter the name of event: ");
            string eventName = Console.ReadLine();

            Console.WriteLine("\nEnter the location of the event: ");
            string eventLocation = Console.ReadLine();

            Console.WriteLine("\nEnter the starting date of the event: ");
            // petlja dok unos nije ispravan

            Console.WriteLine("\nEnter the ending date of the event: ");
            // petlja dok unos nije ispravan

            Console.WriteLine("\nEnter emails of attendees (one string, separated by a whitespace): ");
            // unos u listu?

            // provjera jeli se kome preklapa sa drugim eventom
            // ako je ispisati poruku
            // dodati samo one koji su slobodni

            // svakoj osobi dodat prisutnost za taj event

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


        static void Main(string[] args)
        {
            
            List<Event> eventList = new List<Event>
            {
                new Event("Skate Contest", "Koteks", new DateTime(2022, 12, 11, 15, 30, 00), new DateTime(2022, 12, 11, 20, 00, 00)),
                new Event("Graffiti jam", "Koteks", new DateTime(2022, 12, 11, 11, 30, 00), new DateTime(2022, 12, 11, 14, 00, 00)),
                new Event("Blood Incantation concert", "--", new DateTime(2023, 2, 11, 21, 45, 00), new DateTime(2023, 2, 12, 3, 00, 00)),
                new Event("Discrete math and combinatorics", "--", new DateTime(2022, 11, 26, 11, 00, 00), new DateTime(2022, 11, 26, 13, 00, 00)),
                new Event("Study session", "Sveučilišna knjižnica Split", new DateTime(2022, 11, 23, 14, 30, 00), new DateTime(2022, 11, 23, 21, 25, 00)) 
            };

            Hashtable tableOfEvents = new Hashtable();
            foreach (var item in eventList)
            {
                tableOfEvents.Add(item.Id, item);
            }

            Dictionary<string, Person> people = new Dictionary<string, Person>()
            {
                {"hideyokids@gmail.com", new Person("Anakin", "Skywalker", "hideyokids@gmail.com") },
                { "reckless1111@gmail.com", new Person("Leroy", "Jenkins", "reckless1111@gmail.com") },
                { "birdboxgirl@hotmail.com", new Person("Sandra", "Bulock", "birdboxgirl@hotmail.com")}
            };

            people["hideyokids@gmail.com"].SetAttendance(eventList[0].Id.ToString(), true);
            eventList[4].EmailList.Add("hideyokids@gmail.com", "Anakin Skywalker");


            Dictionary<string, List<string>> menuOptions = new Dictionary<string, List<string>>()
            {
                {"main menu", new List<string> { "Active events", "Upcoming events", "Past events", "Create a new event", "Exit"} },
                {"active events submenu", new List<string> { "Ckeck attendance", "Return to main menu"} },
                {"upcoming events submenu", new List<string> { "Delete event", "Remove people from an event", "Return to main menu"} },
                {"past events submenu", new List<string> {"Return to main menu"} }
            };

            MainMenu(menuOptions, tableOfEvents, people);

            Console.ReadKey();
        }

        static void PrintLine()
        {
            Console.WriteLine("------------------------------------------\n");
        }
    }
}
