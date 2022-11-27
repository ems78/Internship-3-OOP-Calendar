using MailCalendar.classes;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MailCalendar
{
    public class Program
    {
        static void MainMenu(Dictionary<string, List<string>> MenuOptions, EventTable tableOfEvents, Dictionary<string, Person> people)
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
                        PrintHelper.PrintLine();
                        ActiveEvents(MenuOptions, tableOfEvents, people);
                        break;

                    case 2:  // print upcoming evetns
                        Console.Clear();
                        Console.WriteLine("\tUpcoming events");
                        PrintHelper.PrintLine();
                        UpcomingEvents(MenuOptions, tableOfEvents, people);
                        break;

                    case 3:  // print past events
                        Console.Clear();
                        Console.WriteLine("\tPast events");
                        PrintHelper.PrintLine();
                        PastEvents(MenuOptions, tableOfEvents, people);
                        break;

                    case 4:  // create new event
                        Console.Clear();
                        Console.WriteLine("\tCreate a new event");
                        PrintHelper.PrintLine();
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

        
        static void ActiveEvents(Dictionary<string, List<string>> MenuOptions, EventTable tableOfEvents, Dictionary<string, Person> people)
        {
            while (true)
            {
                tableOfEvents.PrintEvents("active");
                PrintHelper.PrintLine();
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
                PrintHelper.MessageAboutAction($"\nNo event with ID {eventIDtoCheck}.");
                return;
            }

            Event currentEvent = (Event)tableOfEvents[eventIDtoCheck];
            if (currentEvent.GetStatus() != "active")
            {
                PrintHelper.MessageAboutAction("\nYou can only check attendances for active events.");
                return;
            }

            Console.Write("\nEnter emails of everyone that is absent (separated by a whitespace): ");
            string AbsenteesEmails = Console.ReadLine(); 

            string wrongEmailList = currentEvent.SetAbsentees(AbsenteesEmails);
            if (wrongEmailList.Length > 0)    
            {
                Console.WriteLine($"\n-- These emails are not in the attendance list for this event:\n{wrongEmailList}");  
            }
            PrintHelper.MessageAboutAction("\nDone.");
        }


        static void UpcomingEvents(Dictionary<string, List<string>> MenuOptions, EventTable tableOfEvents, Dictionary<string, Person> people)
        {
            while (true)
            {
                tableOfEvents.PrintEvents("upcoming");
                PrintHelper.PrintLine();
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


        static void DeleteEvent(EventTable tableOfEvents, Dictionary<string, Person> people)
        {
            Console.Write("\nEnter ID of event you want to delete: ");
            string eventIDtoDelete = Console.ReadLine();

            if (!tableOfEvents.ContainsKey(eventIDtoDelete))
            {
                PrintHelper.MessageAboutAction("\nID doesn't match any event.");
                return;
            }

            Event eventToDelete = (Event)tableOfEvents[eventIDtoDelete];
            
            if (eventToDelete.GetStatus() != "upcoming")
            {
                PrintHelper.MessageAboutAction("\nYou can only delete an upcoming event.");
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
            PrintHelper.MessageAboutAction("\nDeletion successful.");
            Console.Clear();
            return;
        }


        static void RemoveParticipants(EventTable tableOfEvents, Dictionary<string, Person> people)
        {
            Console.Write("\nEnter ID of event you want to remove people from: ");
            string eventIDtoEdit = Console.ReadLine();

            if (!tableOfEvents.ContainsKey(eventIDtoEdit))
            {
                PrintHelper.MessageAboutAction("\nID doesn't match any event.");
                return;
            }

            Event eventToEdit = (Event)tableOfEvents[eventIDtoEdit];
            Console.Write("\nEnter emails of people you want to remove from this event (separated by a whitespace): ");
            string input = Console.ReadLine();

            if (input.Length == 0 || !UserConfirmation("remove these participants from the event"))
            {
                return;
            }

            string wrongInput = eventToEdit.RemoveParticipants(eventToEdit, input, people);
            if (wrongInput.Length > 0)
            {
                Console.WriteLine($"\nEmails that are not on the event participant list: {wrongInput}");
            }
            PrintHelper.MessageAboutAction("\nSuccess.");
            return;
        }


        static void PastEvents(Dictionary<string, List<string>> MenuOptions, EventTable tableOfEvents, Dictionary<string, Person> people)
        {
            tableOfEvents.PrintEvents("past");

            while (true)
            {
                PrintHelper.PrintLine();
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
                if (dateInput.Length < 3)
                {
                    Console.WriteLine("Incorrect date input.");
                    continue;
                }
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
                Console.WriteLine("Incorrect date format.");
            }

            DateTime endingDate;
            while (true)
            {
                Console.Write("\nEnter the ending date of the event (YYYY-MM-DD): ");
                string[] dateInput = Console.ReadLine().Split('-');
                if (dateInput.Length < 3)
                {
                    Console.WriteLine("Incorrect date input.");
                    continue;
                }
                if (int.TryParse(dateInput[0], out int year) && int.TryParse(dateInput[1], out int month) && int.TryParse(dateInput[2], out int day))
                {
                    endingDate = new DateTime(year, month, day);
                    if (endingDate < startingDate)
                    {
                        Console.WriteLine("Event can't end before it begins.");
                        continue;
                    }
                    break;
                }
                Console.WriteLine("Incorrect date format.");
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
                PrintHelper.MessageAboutAction("\nEvent created successfully.");
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
                new Event("Graffiti jam", "Koteks", new DateTime(2022, 11, 27, 14, 30, 00), new DateTime(2022, 12, 11, 00, 00, 00)),
                new Event("Blood Incantation concert", "Tvornica Kulture, Zagreb", new DateTime(2023, 2, 11, 21, 45, 00), new DateTime(2023, 2, 12, 3, 00, 00)),
                new Event("Discrete math and combinatorics midterm", "PMFST amfiteatar B0-2", new DateTime(2022, 11, 26, 11, 00, 00), new DateTime(2022, 11, 26, 13, 00, 00)),
                new Event("Study session", "Sveučilišna knjižnica Split", new DateTime(2022, 11, 27, 18, 20, 00), new DateTime(2022, 11, 27, 21, 00, 00)) 
            };

            EventTable tableOfEvents = new EventTable(eventList);

            Dictionary<string, Person> people = new Dictionary<string, Person>()
            {
                {"hideyokids@gmail.com", new Person("Anakin", "Skywalker", "hideyokids@gmail.com") },
                { "reckless1111@gmail.com", new Person("Leroy", "Jenkins", "reckless1111@gmail.com") },
                { "hiryeong@gmail.com", new Person("Ryeong", "Hi", "hiryeong@gmail.com") },
                { "birdboxgirl@hotmail.com", new Person("Sandra", "Bulock", "birdboxgirl@hotmail.com")},
                { "aaaa44455@gmail.com", new Person("Andre", "Andreas", "aaaa44455@gmail.com")},
                { "galadriex@gmail.com", new Person("Alex", "Gala", "galadriex@gmail.com")},
                { "gigachad@hotmail.com", new Person("Kalei", "Renay", "gigachad@hotmail.com")},
                { "fakemail@gmail.com", new Person("Fake", "Name", "fakemail@gmail.com")},
                { "orange@gmail.com", new Person("Ori", "Ange", "orange@gmail.com")},
                { "whiterunhold@gmail.com", new Person("Balgruuf", "the Greater", "whiterunhold@gmail.com")}
            };

            people["hideyokids@gmail.com"].SetAttendance(eventList[0].Id.ToString(), true);
            eventList[0].AddParticipantEmail(new List<Person>() {people["hideyokids@gmail.com"], people["whiterunhold@gmail.com"] });
            eventList[1].AddParticipantEmail(new List<Person>() {people["fakemail@gmail.com"], people["orange@gmail.com"] });
            eventList[2].AddParticipantEmail(new List<Person>() {people["gigachad@hotmail.com"], people["whiterunhold@gmail.com"], people["aaaa44455@gmail.com"] });
            eventList[3].AddParticipantEmail(new List<Person>() {people["reckless1111@gmail.com"], people["hideyokids@gmail.com"] });
            eventList[4].AddParticipantEmail(new List<Person>() {people["galadriex@gmail.com"], people["whiterunhold@gmail.com"], people["hiryeong@gmail.com"] });

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
    }
}
