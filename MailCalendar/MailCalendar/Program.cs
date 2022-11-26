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
            while (true)
            {
                Console.Clear();
                int i = 0;
                foreach (var element in MenuOptions["main menu"])
                {
                    Console.WriteLine($"{++i} - {element}");
                }

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
                        CreateEvent(tableOfEvents, people);
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
                    if (element.PrintAttendances(true) == "") 
                    {
                        Console.WriteLine("-- No paticipants\n");
                        PrintLine();
                        continue;
                    }
                    string attendeeEmails = element.PrintAttendances(true); 
                    Console.WriteLine($"{attendeeEmails}");
                    PrintLine();
                }
            }

            while (true)
            {
                PrintLine();
                int i = 0;
                foreach (var element in MenuOptions["active events submenu"])
                {
                    Console.WriteLine($"{++i} - {element}");
                }

                switch (UserChoice("a number to navigate the menu"))
                {
                    case 1:  
                        CheckAttendance(tableOfEvents);
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

                                               
        static void CheckAttendance(Hashtable tableOfEvents) 
        {
            Console.Write("Enter ID of event you want to check attendences for: ");
            string eventIDtoCheck = Console.ReadLine();
            if (!tableOfEvents.ContainsKey(eventIDtoCheck))
            {
                Console.Write($"No event with ID {eventIDtoCheck}\nPress any key to continue.");
                Console.ReadKey();
                return;
            }
            Event currentEvent = (Event)tableOfEvents[eventIDtoCheck];
            if (currentEvent.GetStatus() != "active")
            {
                Console.WriteLine("\nYou can only check attendances for active events.\nPress any key to continue.");
                Console.ReadKey();
                return;
            }

            Console.Write("\nEnter emails of everyone that is absent (separated by a whitespace): ");
            string AbsenteesEmails = Console.ReadLine(); 

            string wrongEmailList = currentEvent.SetAbsentees(AbsenteesEmails);
            if (wrongEmailList.Length > 0)    
            {
                Console.WriteLine($"\n-- These emails are not in the attendance list for this event:\n{wrongEmailList}");  
            }
            Console.Write("\nPress any key to continue.");
            Console.ReadKey();
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
                        Console.WriteLine("-- No paticipants");
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
                PrintLine();
                int i = 0;
                foreach (var element in MenuOptions["upcoming events submenu"])
                {
                    Console.WriteLine($"{++i} - {element}");
                }

                switch (UserChoice("a number to navigate the menu"))
                {
                    case 1:  
                        DeleteEvent(tableOfEvents,people);
                        break;

                    case 2:  
                        RemoveParticipants(tableOfEvents, people);
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

                if (!UserConfirmation("delete this event"))
                {
                    return;
                }

                foreach (string email in eventToDelete.EmailList.Keys)  // brisati podatke o prisutnosti na tom eventu
                {
                    people[email].Attendance.Remove(eventIDtoDelete);
                }
                tableOfEvents.Remove(eventIDtoDelete);  // izbrisat event
                Console.WriteLine("Deletion successful.\nPress any key to continue.");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("ID doesn't match any event.\nPress any key to continue.");
            Console.ReadKey();
            return;
        }


        static void RemoveParticipants(Hashtable tableOfEvents, Dictionary<string, Person> people)
        {
            Console.Write("\nEnter ID of event you want to remove people from: ");
            string eventIDtoEdit = Console.ReadLine();
            if (tableOfEvents.ContainsKey(eventIDtoEdit))
            {
                Event eventToEdit = (Event)tableOfEvents[eventIDtoEdit];

                Console.Write("\nEnter emails of people you want to remove from this event (separated by a whitespace): ");
                string input = Console.ReadLine();

                if (!UserConfirmation("remove these participants from the event"))
                {
                    return;
                }

                string[] emailsToRemove = input.Split(' ');
                string wrongInput = "";
                foreach (var email in emailsToRemove)
                {
                    if (!eventToEdit.EmailList.ContainsKey(email.Trim()))
                    {
                        wrongInput += $"{email}, ";
                        continue;
                    }
                    eventToEdit.EmailList.Remove(email);  
                    people[email].Attendance.Remove(eventIDtoEdit);
                }
                if (wrongInput.Length > 0)
                {
                    Console.WriteLine($"\nEmails that are not on the event participant list: {wrongInput}");
                }
                Console.WriteLine("Successful.\nPress any key to continue.");
                Console.ReadKey();
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
                        Console.WriteLine("-- No paticipants");
                        continue;
                    }
                    string[] atendeeEmails = element.PrintAttendances(true).Split(' ');
                    foreach (var email in atendeeEmails)
                    {
                        Console.WriteLine($"{email}\n");
                    }
                    string absentAtendeeEmails = element.PrintAttendances(false);
                    Console.WriteLine($"Absent participants:\n{absentAtendeeEmails}");
                }
            }

            while (true)
            {
                PrintLine();
                int i = 0;
                foreach (var element in MenuOptions["past events submenu"])
                {
                    Console.WriteLine($"{++i} - {element}");
                }

                switch (UserChoice("a number to navigate the menu"))
                {
                    case 1: 
                        MainMenu(MenuOptions, tableOfEvents, people);
                        break;

                    default:
                        break;
                }
            }
        }

                                                        
        static void CreateEvent(Hashtable tableOfEvents, Dictionary<string, Person> people)
        {
            Console.Write("Enter the name of event: ");
            string eventName = Console.ReadLine();

            Console.Write("\nEnter the location of the event: ");
            string eventLocation = Console.ReadLine();

            DateTime startingDate;
            while (true)
            {
                Console.Write("\nEnter the starting date of the event (YYYY-MM-DD): ");
                string[] dateInput = Console.ReadLine().Split('-');
                if (int.TryParse(dateInput[0], out int year) && int.TryParse(dateInput[1], out int month) && int.TryParse(dateInput[2], out int day))
                {
                    startingDate = new DateTime(year, month, day);
                    if (startingDate < DateTime.Now)
                    {
                        Console.WriteLine("You can only create an upcoming event.\n");
                        continue;
                    }
                    break;
                }
                Console.WriteLine("Incorrect date input.");
            }

            DateTime endingDate;
            while (true)
            {
                Console.Write("\nEnter the ending date of the event (YYYY-MM-DD): ");
                string[] dateInput2 = Console.ReadLine().Split('-');
                if (int.TryParse(dateInput2[0], out int year) && int.TryParse(dateInput2[1], out int month) && int.TryParse(dateInput2[2], out int day))
                {
                    endingDate = new DateTime(year, month, day);
                    if (endingDate < startingDate)
                    {
                        Console.WriteLine("Event can't end before it begins.");
                        continue;
                    }
                    break;
                }
                Console.WriteLine("Incorrect date input.");
            }

            Event newEvent = new Event(eventName, eventLocation, startingDate, endingDate);
            string newEventId = newEvent.Id.ToString();

            while (true)
            {
                Console.Write("\nEnter emails of attendees (one string, separated by a whitespace): ");
                string[] attendees = Console.ReadLine().Split(' ');
                string unavailablePeople = "";

                foreach (var person in attendees)
                {
                    if (people.ContainsKey(person))
                    {
                        bool unavailable = false;
                        foreach (var eventAttendance in people[person].Attendance)
                        {
                            
                            Event temp = (Event)tableOfEvents[eventAttendance.Key];
                            if ((startingDate < temp.DateTimeOfBeginning && startingDate > temp.DateTimeOfEnding) ||  // novi se preklapa sa pocetkom postojeceg
                                (startingDate > temp.DateTimeOfBeginning && endingDate < temp.DateTimeOfEnding) ||   // novi je unutar postojeceg
                                (startingDate < temp.DateTimeOfEnding && endingDate > temp.DateTimeOfEnding))       // novi pocinje prije zavrsetka postojeceg
                            {
                                unavailablePeople += $"{person}, ";
                                unavailable = true;
                                break;
                            }
                        }
                        if (unavailable)
                        {
                            continue;
                        }
                        people[person].SetAttendance(newEventId, true);
                        newEvent.EmailList.Add(person, true);
                    }
                    else
                    {
                        Console.WriteLine($"** {person} is not in the list of emails.");
                    }
                }

                if (unavailablePeople.Length > 0)
                {
                    Console.WriteLine($"\n-- Unavailable: {unavailablePeople}");
                }

                if (newEvent.EmailList.Count > 0)
                {
                    break;
                }
                Console.WriteLine("Cannot create an event without participants.");
            }

            if (UserConfirmation("create new event"))
            {
                tableOfEvents.Add(newEventId, newEvent);
                Console.Write("\nEvent created successfully.\nPress any key to continue.");
                Console.ReadKey();
            }
        }

        
        static bool UserConfirmation(string typeOfConfirmation)
        {
            Console.Write($"\nAre you sure you want to {typeOfConfirmation}? (y/n) ");
            if (string.Compare(Console.ReadLine().ToLower(), "y") == 0)
            {
                return true;
            }
            return false;
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
                new Event("Blood Incantation concert", "Tvornica Kulture, Zagreb", new DateTime(2023, 2, 11, 21, 45, 00), new DateTime(2023, 2, 12, 3, 00, 00)),
                new Event("Discrete math and combinatorics midterm", "PMFST amfiteatar B0-2", new DateTime(2022, 11, 26, 11, 00, 00), new DateTime(2022, 11, 27, 13, 00, 00)),
                new Event("Study session", "Sveučilišna knjižnica Split", new DateTime(2022, 11, 23, 14, 30, 00), new DateTime(2022, 11, 23, 21, 25, 00)) 
            };

            Hashtable tableOfEvents = new Hashtable();
            foreach (var item in eventList)
            {
                tableOfEvents.Add(item.Id.ToString(), item);
            }

            Dictionary<string, Person> people = new Dictionary<string, Person>()
            {
                {"hideyokids@gmail.com", new Person("Anakin", "Skywalker", "hideyokids@gmail.com") },
                { "reckless1111@gmail.com", new Person("Leroy", "Jenkins", "reckless1111@gmail.com") },
                { "hiryeong@gmail.com", new Person("Ryeong", "Hi", "hiryeong@gmail.com") },
                { "birdboxgirl@hotmail.com", new Person("Sandra", "Bulock", "birdboxgirl@hotmail.com")}
            };

            people["hideyokids@gmail.com"].SetAttendance(eventList[0].Id.ToString(), true);
            eventList[4].EmailList.Add("hideyokids@gmail.com", true);
            eventList[3].EmailList.Add("hiryeong@gmail.com", true);


            Dictionary<string, List<string>> menuOptions = new Dictionary<string, List<string>>()
            {
                {"main menu", new List<string> { "Active events", "Upcoming events", "Past events", "Create a new event", "Exit"} },
                {"active events submenu", new List<string> { "Check attendance", "Return to main menu"} },
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
