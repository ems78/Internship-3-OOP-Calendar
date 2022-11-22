using System;
using System.Collections.Generic;

namespace MailCalendar.classes
{
    internal class Event
    {
        public Guid Id { get; }
        public string Name { get; private set; }
        public string Location { get; private set; }
        public DateTime DateTimeOfBeginning { get; private set; }
        public DateTime DateTimeOfEnding { get; private set; }
        public Dictionary<string, string> EmailList { get; private set; }
            

        public Event(string name, string location, DateTime dateTimeOfBeginning, DateTime dateTimeOfEnding)
        {
            Id = Guid.NewGuid();
            Name = name;
            Location = location;
            DateTimeOfBeginning = dateTimeOfBeginning;
            DateTimeOfEnding = dateTimeOfEnding;
            EmailList = new Dictionary<string, string>();
        }


        public bool AddParticipantEmail(string newEmail, string nameSurname)
        {
            if (EmailList.ContainsKey(newEmail))
            {
                return false;
            }
            EmailList.Add(newEmail, nameSurname);
            return true;
        }


        public string EndsIn()
        {
            TimeSpan remainingTime = (DateTimeOfEnding - DateTime.Now);
            if (remainingTime.TotalHours > 24)
            {
                double endsInDays = remainingTime.TotalDays;
                return String.Format("{0:.##} days", endsInDays);
            }
            return string.Format("{0:.##} hours", remainingTime.TotalHours);
        }
    }
}
