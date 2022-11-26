using System;
using System.Collections.Generic;
using System.Text;

namespace MailCalendar.classes
{
    internal class Event
    {
        public Guid Id { get; }
        public string Name { get; private set; }
        public string Location { get; private set; }
        public DateTime DateTimeOfBeginning { get; private set; }
        public DateTime DateTimeOfEnding { get; private set; }
        //public Dictionary<string, string> EmailList { get; private set; }
        public Dictionary<string, bool> EmailList { get; private set; }
            

        public Event(string name, string location, DateTime dateTimeOfBeginning, DateTime dateTimeOfEnding)
        {
            Id = Guid.NewGuid();
            Name = name;
            Location = location;
            DateTimeOfBeginning = dateTimeOfBeginning;
            DateTimeOfEnding = dateTimeOfEnding;
            EmailList = new Dictionary<string,  bool>();
        }


        public bool AddParticipantEmail(string newEmail, string nameSurname)
        {
            if (EmailList.ContainsKey(newEmail))
            {
                return false;
            }
            EmailList.Add(newEmail, true);
            return true;
        }


        public string SetAbsentees(string emails)
        {
            string wrongInput = "";
            foreach (var person in emails.Split(' '))
            {
                if (EmailList.ContainsKey(person))
                {
                    EmailList[person] = false;
                    continue;
                }
                wrongInput += $"{person}, ";
            }
            return wrongInput;
        }


        public string GetDuration(string eventStatus)
        {
            switch (eventStatus)
            {
                case "active":
                    TimeSpan remainingTime = (DateTimeOfEnding - DateTime.Now);
                    return string.Format("{0:.#} hours", remainingTime.TotalHours);

                case "upcoming":
                    TimeSpan daysUntilBeginning = (DateTimeOfBeginning - DateTime.Now);
                    TimeSpan durationInHours = (DateTimeOfEnding - DateTimeOfBeginning);
                    return string.Format($"{daysUntilBeginning.TotalDays:.} days\nDuration: {durationInHours.TotalHours:.#} hours");

                case "past":
                    TimeSpan daysSinceEnding = (DateTime.Now - DateTimeOfEnding);
                    TimeSpan durationInHours2 = (DateTimeOfEnding - DateTimeOfBeginning);
                    return string.Format($"{daysSinceEnding.TotalDays:.} days ago\nDuration: {durationInHours2.TotalHours:.#} hours");

                default:
                    return "incorrect argument";
            }
        }

        public string GetStatus()
        {
            if (DateTimeOfBeginning < DateTime.Now && DateTimeOfEnding > DateTime.Now)
                return "active";

            else if (DateTimeOfBeginning > DateTime.Now)
                return "upcoming";

            return "past";
        }

        public string PrintAttendances(bool attended)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (attended)
            {
                foreach (var email in EmailList)
                {
                    if (email.Value)
                    {
                        stringBuilder.Append($"{email.Key}, ");
                    }                    
                }
                return stringBuilder.ToString();
            }
            foreach (var email in EmailList)
            {
                if (!email.Value)
                {
                    stringBuilder.Append($"{email.Key}, ");
                }                
            }
            return stringBuilder.ToString();
        }
    }
}
