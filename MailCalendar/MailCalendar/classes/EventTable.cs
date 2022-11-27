
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MailCalendar.classes
{
    internal class EventTable : Hashtable
    {
        public EventTable(List<Event> eventList)
        {
            foreach (var item in eventList)
            {
                this.Add(item.Id.ToString(), item); ;
            }
        }

        public void PrintEvents(string type)
        {
            switch (type)
            {
                case "active":
                    foreach (Event storedEvent in this.Values)
                    {
                        //Event storedEvent = (Event)element;
                        if (storedEvent.GetStatus() == "active")
                        {
                            Console.WriteLine($"ID: {storedEvent.Id}\nName: {storedEvent.Name}\nLocation: {storedEvent.Location} Ends in: {storedEvent.GetDuration("active")}\n\nParticipans:");
                            if (storedEvent.PrintAttendances(true) == "")
                            {
                                Console.WriteLine("-- No paticipants\n");
                                PrintHelper.PrintLine();
                                continue;
                            }
                            string attendeeEmails = storedEvent.PrintAttendances(true);
                            Console.WriteLine($"{attendeeEmails}");
                            PrintHelper.PrintLine();
                        }
                    }
                    break;

                case "upcoming":
                    foreach (Event storedEvent in this.Values)
                    {
                        if (storedEvent.GetStatus() == "upcoming")
                        {
                            Console.WriteLine($"\nID: {storedEvent.Id}\nName: {storedEvent.Name}\nLocation: {storedEvent.Location} \nEnds in: {storedEvent.GetDuration("upcoming")}\n\nParticipans:");
                            if (storedEvent.EmailList.Count is 0)
                            {
                                Console.WriteLine("-- No paticipants");
                                PrintHelper.PrintLine();
                                continue;
                            }
                            string attendeeEmails = storedEvent.PrintAttendances(true);
                            Console.WriteLine(attendeeEmails);
                            PrintHelper.PrintLine();
                        }
                    }
                    break;

                case "past":
                    foreach (Event storedEvent in this.Values)
                    {
                        if (storedEvent.GetStatus() == "past")
                        {
                            Console.WriteLine($"\nID: {storedEvent.Id}\nName: {storedEvent.Name}\nLocation: {storedEvent.Location}\nEnded: {storedEvent.GetDuration("past")}\n\nParticipans:");
                            if (storedEvent.EmailList.Count is 0)
                            {
                                Console.WriteLine("-- No paticipants");
                                PrintHelper.PrintLine();
                                continue;
                            }
                            string attendee = storedEvent.PrintAttendances(true);
                            Console.WriteLine(attendee);

                            string absentAtendeeEmails = storedEvent.PrintAttendances(false);
                            Console.WriteLine($"\nAbsent participants:\n{absentAtendeeEmails}");
                            PrintHelper.PrintLine();
                        }
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
